using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMeleeCombatStance : IAiState
{
    private bool randomDestinationSet = false;
    private float verticalMovementValue = 0;
    private float horizontalMovementValue = 0;
    public AiStateId GetId()
    {
        return AiStateId.MeleeCombatStance;
    }

    public void Enter(AiAgent agent)
    {
        agent._gameManager.RegisterAttacker(agent);
    }

    public void Update(AiAgent agent)
    {
        //Apply Vertical movement
        //Apply Horizontal Movement

        if (agent._enemyManager.isPerformingAction)
        {
            //Reset Movement To zero
        }
        if (agent.distanceFromPlayer > agent.maximumAttackRange)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeChase);
        }

        if (!randomDestinationSet)
        {
            randomDestinationSet = true;
            DecideCirclingAction(agent._enemyAnimatorManager);
        }
        
        HandleRotationToTarget(agent);
        
        if (agent.currentRecoveryTime <= 0 && agent.currentAttack != null)
        {
            randomDestinationSet = false;
            agent.StateMachine.ChangeState(AiStateId.MeleeAttack);
        }
        else
        {
            GetNewAttack(agent);
        }
        
       
    }

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

    private void DecideCirclingAction(EnemyAnimatorManager animatorManager)
    {
        WalkAroundPlayer(animatorManager);
    }
    
    private void WalkAroundPlayer(EnemyAnimatorManager animatorManager)
    {
        verticalMovementValue = Random.Range(0, 1); // Only Forward For Melee

        if (verticalMovementValue <= 1 && verticalMovementValue > 0)
        {
            verticalMovementValue = 0.5f;
        }
        else if (verticalMovementValue >= -1 && verticalMovementValue < 0)
        {
            verticalMovementValue = -0.5f;
        }

        horizontalMovementValue = Random.Range(-1, 1);

        if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0)
        {
            horizontalMovementValue = 0.5f;
        }
        else if(horizontalMovementValue >= -1 && horizontalMovementValue < 0)
        {
            horizontalMovementValue = -0.5f;
        }
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
    
    public void Exit(AiAgent agent)
    {
        
    }
}
