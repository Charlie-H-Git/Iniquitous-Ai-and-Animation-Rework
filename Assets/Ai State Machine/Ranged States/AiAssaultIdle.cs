using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAssaultIdle : IAiState
{
    public AiStateId GetId()
    {
        return AiStateId.AssaultIdle;
    }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        agent.targetPos = agent.player.transform.position;
        if (!agent.canSee)
        {
            LineOfSight(agent);
        }
        else
        {
            agent.StateMachine.ChangeState(AiStateId.AssaultChase);
        }
    }

    private void LineOfSight(AiAgent agent)
    {
        RaycastHit losHIT;
        Debug.DrawLine(agent.transform.position, agent.player.transform.position);
        if (Physics.Linecast(agent.transform.position, agent.player.transform.position, out losHIT, agent.combatMask))
        {
            agent.los = false;
        }
        else
        {
            agent.los = true;
        }
        //If the player is within the Agents cone of vision and is close enough Set CanSee to true
        if (agent.angleFromTarget < agent.coneAngle && agent.angleFromTarget > -agent.coneAngle)
        {
            if ( agent.distanceFromTarget < agent.detectionDistance)
            {
                agent.inCone = true;
            }
            else
            {
                agent.inCone = false;
            }
        }
        if (agent.inCone && agent.los)
        {
            agent.canSee = true;
        }
    }
    public void Exit(AiAgent agent)
    {
        
    }
}
