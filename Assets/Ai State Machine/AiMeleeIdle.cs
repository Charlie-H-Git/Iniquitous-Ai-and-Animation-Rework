using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AiMeleeIdle : IAiState
{
    
    public AiStateId GetId()
    {
        return AiStateId.MeleeIdle;
    }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        LineOfSight(agent);
        if (agent.canSee)
        {
            agent.StateMachine.ChangeState(AiStateId.MeleeChase);
        }
        
        
    }

    public void Exit(AiAgent agent)
    {
        
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
        if (agent.angleFromPlayer < agent.coneAngle && agent.angleFromPlayer > -agent.coneAngle)
        {
            if ( agent.distanceFromPlayer < agent.detectionDistance)
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
    
   
}
