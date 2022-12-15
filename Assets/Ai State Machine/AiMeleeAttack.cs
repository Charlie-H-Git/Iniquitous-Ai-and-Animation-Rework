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
        HandleRotationToTarget(agent);
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
                agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
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
    }
    

    #endregion

    private void HandleRotationToTarget(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        //Rotate Manually
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = agent.player.transform.position - agent.transform.position;
            direction.y = 0;
            direction.Normalize();
            if (direction == Vector3.zero)
            {
                direction = agent.transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.rotationSpeed / Time.deltaTime );
        }
        //Rotate with pathfinding
        else
        {
            Vector3 relativeDirection = agent.transform.InverseTransformDirection(agent.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = agent._enemyRigidbody.velocity;

            agent.navMeshAgent.enabled = true;
            agent.navMeshAgent.SetDestination(agent.player.transform.position);
            agent._enemyRigidbody.velocity = targetVelocity;
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, agent.navMeshAgent.transform.rotation, agent.rotationSpeed / Time.deltaTime);
        }
    }
    public void Exit(AiAgent agent)
    {
        
    }
}
