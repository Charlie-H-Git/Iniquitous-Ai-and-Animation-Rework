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
        
    }

    public void Update(AiAgent agent)
    {
        agent.HandleMoveToPlayer();
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
