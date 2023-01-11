using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAssaultRetreat : IAiState
{
    private Coroutine movementCoroutine;
    private Collider[] Colliders = new Collider[10];
    private bool LineOfSight = true;
    private AiAgent _agent;
    
    public AiStateId GetId()
    {
        return AiStateId.AssaultRetreat;
    }

    public void Enter(AiAgent agent)
    {
        _agent = agent;
        agent.navMeshAgent.enabled = true;
        agent.enemyCoverHandler.OnGainSight += HandleGainSight;
        agent.enemyCoverHandler.OnLoseSight += HandleLoseSight;
    }

    private void HandleGainSight(Transform target, AiAgent agent)
    {
        if (!LineOfSight)
        {
            LineOfSight = true;
        }
    }

    private void HandleLoseSight(Transform target, AiAgent agent)
    {
        if (LineOfSight)
        {
            
        }
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
        
        if (agent.distanceFromTarget <= 0.5)
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0 ,0.1f, Time.deltaTime);
            enemyManager.isPerformingAction = true;
        }
        else
        {
            enemyManager.isPerformingAction = false;
        }
        
    }

    private void HandleRotationToTarget(AiAgent agent)
    {
        EnemyManager enemyManager = agent._enemyManager;
        //Rotate Manually
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = agent.targetPos - agent.transform.position;
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
           agent.navMeshAgent.SetDestination(agent.targetPos);
           agent._enemyRigidbody.velocity = targetVelocity;
           agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, agent.navMeshAgent.transform.rotation, agent.rotationSpeed / Time.deltaTime);
        }
    }

    #endregion

    private void Hide(AiAgent agent)
    {
        EnemyAnimatorManager enemyAnimatorManager = agent._enemyAnimatorManager;
        
         Debug.Log("LineOfSight = " + LineOfSight);
         
        if (LineOfSight)
        {
            //Remove all colliders at the top of the list
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }
            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, agent.enemyCoverHandler.Collider.radius,
                Colliders, agent.hidableLayers);
            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(Colliders[i].transform.position, agent.player.transform.position) < agent.MinPlayerDistance)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;
            System.Array.Sort(Colliders, ColliderArraySortComparer);
            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, 2f,
                        agent.navMeshAgent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.navMeshAgent.areaMask))
                    {
                        Debug.Log($"Unable to find edge close to {hit.position}" +
                                  "please check your baked nav mesh and scene area");
                    }

                    if (Vector3.Dot(hit.normal, (agent.player.transform.position - hit.position).normalized) < agent.hideSensitivity)
                    {
                        //Move agent to this position
                        agent.targetPos = hit.position;
                        HandleMoveToTarget(agent);
                        break;
                    }
                    else
                    {
                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (agent.player.transform.position - hit.position).normalized * 2,
                                out NavMeshHit hit2, 2f, agent.navMeshAgent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.navMeshAgent.areaMask))
                            {
                                Debug.Log($"Unable to find edge close to {hit2.position} (SECOND ATTEMPT)" +
                                          "please check your baked nav mesh and scene area");
                            }

                            if (Vector3.Dot(hit2.normal, (agent.player.transform.position - hit2.position).normalized) <
                                agent.hideSensitivity)
                            {
                                agent.targetPos = hit.position;
                                HandleMoveToTarget(agent);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log($"Unable to find NavMesh near Object {Colliders[i].name} at {Colliders[i].transform.position}");
                }
            }
        }
        else
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0 ,0.1f, Time.deltaTime);
            agent.rigWeightHandler.LerpWeight(agent.rigWeightHandler.Rigs[0],0,0.2f);
        }
    }

    private int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null & B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null & B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(_agent.transform.position, A.transform.position)
                .CompareTo(Vector3.Distance(_agent.transform.position, B.transform.position));
        }
    }
    
    public void Update(AiAgent agent)
    {
        Hide(agent);
        
        switch (agent.enemyHealth.health)
        {
            case <= 0:
                //TODO: DEAD
                break;
            case >= 70:
                agent.StateMachine.ChangeState(AiStateId.AssaultChase);
                break;
        }
        //If health falls below 75 start walking backwards
        //Start searching for nearby objects on obstacles
            //Turn on search sphere trigger
        //If health falls below 50 find cover point
        //or if there is an obstacle with certain range
        //When in cover start healing by 1 / 2s to a maximum of 80%
        //when health reach 80%
        //Return to chase mode
       
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
