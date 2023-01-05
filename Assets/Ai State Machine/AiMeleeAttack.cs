using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMeleeAttack : IAiState
{
    private bool isCombo = false;
    public AiStateId GetId()
    {
        return AiStateId.MeleeAttack;
    }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        EnemyAnimatorManager enemyAnimatorManager = agent._enemyAnimatorManager;
        AttackTarget(agent);
        //HandleRotationToTarget(agent);
        if (agent.distanceFromPlayer > agent.maximumAttackRange)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeChase);
        }
        if (enemyManager.isPerformingAction && agent.canDoCombo == false)
        {
            
        }
        else if(enemyManager.isPerformingAction && agent.canDoCombo)
        {
            if (isCombo)
            {
                enemyAnimatorManager.PlayTargetAnimation(agent.currentAttack.actionAnimation, true);
                isCombo = false; 
            }
        }
    }

     #region Attacks

    private void AttackTarget(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        EnemyAnimatorManager enemyAnimatorManager = agent._enemyAnimatorManager;
        if (enemyManager.isPerformingAction && isCombo == false)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
        }

        if (agent.currentAttack != null)
        {
            if (agent.distanceFromPlayer < agent.currentAttack.minimumDistanceToAttack)
            {
                // agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
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
                            
                            agent._enemyAnimatorManager.anim.SetFloat("Vertical", 0 , 0.1f , Time.deltaTime);
                            enemyAnimatorManager.PlayTargetAnimation(agent.currentAttack.actionAnimation, true);
                            agent._enemyManager.isPerformingAction = true;
                            if (agent.currentAttack.canCombo)
                            {
                                agent.currentAttack = agent.currentAttack.comboAction;
                            }
                            else
                            {
                                agent.currentRecoveryTime = agent.currentAttack.recoveryTime;
                                agent.currentAttack = null;
                                agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance); 
                            }
                            
                        }
                    }
                }
            }
        }
    }

    //
    // private void GetNewAttack(AiAgent agent)
    // {
    //     int maxScore = 0;
    //     for (int i = 0; i < agent.enemyAttacks.Length; i++)
    //     {
    //         EnemyAttackAction enemyAttackAction = agent.enemyAttacks[i];
    //         if (agent.distanceFromPlayer <= enemyAttackAction.maximumDistanceToAttack
    //             && agent.distanceFromPlayer >= enemyAttackAction.minimumDistanceToAttack)
    //         {
    //             if (agent.angleFromPlayer <= enemyAttackAction.maximumAttackAngle
    //                 && agent.angleFromPlayer >= enemyAttackAction.minimumAttackAngle)
    //             {
    //                 maxScore += enemyAttackAction.attackScore;
    //             }
    //         }
    //     }
    // }

    #endregion

    // private void HandleRotationToTarget(AiAgent agent)
    // {
    //     EnemyManager enemyManager = agent._enemyManager;
    //     //Rotate Manually
    //     if (enemyManager.isPerformingAction)
    //     {
    //         Vector3 direction = agent.player.transform.position - agent.transform.position;
    //         direction.y = 0;
    //         direction.Normalize();
    //         if (direction == Vector3.zero)
    //         {
    //             direction = agent.transform.forward;
    //         }
    //         Quaternion targetRotation = Quaternion.LookRotation(direction);
    //         agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.rotationSpeed / Time.deltaTime );
    //     }
    //     //Rotate with pathfinding
    //     else
    //     {
    //         Vector3 relativeDirection = agent.transform.InverseTransformDirection(agent.navMeshAgent.desiredVelocity);
    //         Vector3 targetVelocity = agent._enemyRigidbody.velocity;
    //     
    //         agent.navMeshAgent.enabled = true;
    //         agent.navMeshAgent.SetDestination(agent.player.transform.position);
    //         agent._enemyRigidbody.velocity = targetVelocity;
    //         agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, agent.navMeshAgent.transform.rotation, agent.rotationSpeed / Time.deltaTime);
    //     }
    // }
    public void Exit(AiAgent agent)
    {
        
    }
}
