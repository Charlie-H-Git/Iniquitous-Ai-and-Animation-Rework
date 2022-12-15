using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMeleeAttack : IAiState
{
    public AiStateId GetId()
    {
        return AiStateId.MeleeAttack;
    }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        AttackTarget(agent);
        // if (agent._enemyManager.isPerformingAction)
        // {
        //     //If performing an action set the movement to 0
        //    agent._enemyAnimatorManager.anim.SetFloat("Blend", 0 ,0.1f, Time.deltaTime);
        //     agent.navMeshAgent.enabled = false;
        // }
        if (agent.distanceFromPlayer > agent.maximumAttackRange)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeChase);
        }
    }

     #region Attacks

    private void AttackTarget(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        EnemyAnimatorManager enemyAnimatorManager = agent._enemyAnimatorManager;
        if (enemyManager.isPerformingAction)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
        }

        if (agent.currentAttack != null)
        {
            if (agent.distanceFromPlayer < agent.currentAttack.minimumDistanceToAttack)
            {
                GetNewAttack(agent);
            }
            else
            {
                if (agent.distanceFromPlayer < agent.currentAttack.maximumDistanceToAttack)
                {
                    if (agent.angleFromPlayer <= agent.currentAttack.maximumAttackAngle 
                        && agent.angleFromPlayer >= agent.currentAttack.minimumAttackAngle)
                    {
                        if (agent.currentRecoveryTime <= 0 && agent._enemyManager.isPerformingAction == false)
                        {
                            
                            agent._enemyAnimatorManager.anim.SetFloat("Blend", 0 , 0.1f , Time.deltaTime);
                            enemyAnimatorManager.PlayTargetAnimation(agent.currentAttack.actionAnimation, true);
                            agent._enemyManager.isPerformingAction = true;
                            agent.currentRecoveryTime = agent.currentAttack.recoveryTime;
                            agent.currentAttack = null;
                            agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
                        }
                    }
                }
            }
        }
        else
        {
            GetNewAttack(agent);
        }
        //agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
    }
    private void GetNewAttack(AiAgent agent)
    {
        int maxScore = 0;
        for (int i = 0; i < agent.enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = agent.enemyAttacks[i];
            if (agent.distanceFromPlayer <= enemyAttackAction.maximumDistanceToAttack 
                && agent.distanceFromPlayer >= enemyAttackAction.minimumDistanceToAttack)
            {
                if (agent.angleFromPlayer <= enemyAttackAction.maximumAttackAngle 
                    && agent.angleFromPlayer >= enemyAttackAction.minimumAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore);
        int tempScore = 0;
        for (int i = 0; i < agent.enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = agent.enemyAttacks[i];
            if (agent.distanceFromPlayer <= enemyAttackAction.maximumDistanceToAttack 
                && agent.distanceFromPlayer >= enemyAttackAction.minimumDistanceToAttack)
            {
                if (agent.angleFromPlayer <= enemyAttackAction.maximumAttackAngle 
                    && agent.angleFromPlayer >= enemyAttackAction.minimumAttackAngle)
                {
                    if (agent.currentAttack != null) 
                        return;
                    tempScore += enemyAttackAction.attackScore;

                    if (tempScore > randomValue)
                    {
                        agent.currentAttack = enemyAttackAction;
                    }
                }
            }
        }
    }

    #endregion

    public void Exit(AiAgent agent)
    {
        
    }
}
