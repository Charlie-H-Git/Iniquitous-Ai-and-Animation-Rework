using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AiMeleeChase : IAiState
{
    public AiStateId GetId()
    {
        return AiStateId.MeleeChase;
    }

    public void Enter(AiAgent agent)
    { 
        Debug.Log("Entered Melee Chase");
        agent.navMeshAgent.enabled = true;
        agent._gameManager.DeRegisterAttacker(agent);
    }

    public void Update(AiAgent agent)
    {
        HandleMoveToTarget(agent);
    }

    #region Handle Moving To Target Position

    private void HandleMoveToTarget(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        EnemyAnimatorManager enemyAnimatorManager = agent._enemyAnimatorManager;
        if (enemyManager.isPerformingAction)
            return;
        
        //if we are not performing an action
        // if the distance from player is greater than stopping distance
        if (agent.distanceFromTarget > agent.maximumAttackRange)
        {
            //Set the movement to 1 over time
            enemyAnimatorManager.anim.SetFloat("Vertical", 1 ,0.1f, Time.deltaTime);
                
        }
        HandleRotationToTarget(agent);

        Transform navMeshAgentTransform = agent.navMeshAgent.transform;
        //Set navmesh local transform values to zero
        
        if (agent.distanceFromTarget <= agent.maximumAttackRange)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeCombatStance);
        }
        else
        {
            return;
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

    #endregion
    
    
    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.enabled = false;
    }
}
