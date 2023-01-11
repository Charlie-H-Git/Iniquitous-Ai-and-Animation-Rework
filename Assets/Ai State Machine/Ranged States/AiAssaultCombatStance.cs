using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AiAssaultCombatStance : IAiState
{
    
    private float verticalMovementValue = 0;
    private float horizontalMovementValue = 0;
    private Vector3 combatPosition;
    private bool changePosition = true;
    private float changeCountdown = 1f;
    
    public AiStateId GetId()
    {
        return AiStateId.AssaultCombatStance;
    }

    public void Enter(AiAgent agent)
    {
        //agent._enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        combatPosition = agent.transform.position;
    }

    public void Update(AiAgent agent)
    {
        agent._enemyAnimatorManager.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
        agent._enemyAnimatorManager.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);

        if (agent._enemyManager.isPerformingAction)
        {
            agent._enemyAnimatorManager.anim.SetFloat("Vertical", 0);
            agent._enemyAnimatorManager.anim.SetFloat("Horizontal", 0);
        }
        
        switch (agent.enemyHealth.health)
        {
            case <= 0:
                //TODO: DEAD
                break;
            case <= 50:
                agent.StateMachine.ChangeState(AiStateId.AssaultRetreat);
                break;
        }
        
        TacticalMovement(agent);
    }
    
    

    void TacticalMovement(AiAgent agent)
    {
        float distanceFromPoint = Vector3.Distance(agent.transform.position, combatPosition);
        
        HandleRotationToTarget(agent);
        HandleAimingLerp(agent);

        if (agent.distanceFromTarget <= agent.maximumAttackRange)
        {
            WalkAroundPoint(distanceFromPoint, agent);
        }

        changeCountdown -= Time.deltaTime;
        if (changeCountdown <= 0)
        {
            changePosition = true;
            changeCountdown = 1f;
        }
        //TODO change target to combat position
        if (distanceFromPoint >= 2.5f)
        {
            // agent.chasingCombatPoint = true;
            // agent.targetPos = combatPosition;
            // agent.maximumAttackRange = 0;
            agent._enemyAnimatorManager.anim.SetFloat("Vertical", 0);
            agent._enemyAnimatorManager.anim.SetFloat("Horizontal", 0);
            agent.StateMachine.ChangeState(AiStateId.AssaultChase);
        }
    }
    
    void HandleAimingLerp(AiAgent agent)
    {
        float distance = Vector3.Distance(agent.player.transform.position, agent.gameObject.transform.position);
        if (distance <= agent.MaximumAimingRange)
        {
            agent.rigWeightHandler.LerpWeight(agent.rigWeightHandler.Rigs[0], 1 , 0.05f);
            //Start shooting
        }
        else
        {
            //Lower Gun
            //Stop Shooting
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

    private void WalkAroundPoint(float distanceFromPoint, AiAgent agent)
    {
        agent.navMeshAgent.enabled = false;
        

        if (changePosition)
        {
            verticalMovementValue = Random.Range(-1, 1); // Only Forward For Melee

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

            changePosition = false;
        }
    }

    
    public void Exit(AiAgent agent)
    {
        agent._enemyAnimatorManager.anim.SetFloat("Vertical", 0);
        agent._enemyAnimatorManager.anim.SetFloat("Horizontal", 0);
    }
}
