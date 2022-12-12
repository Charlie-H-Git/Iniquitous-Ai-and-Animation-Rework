using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add any new states to this Enum
/// </summary>
public enum AiStateId
{
    MeleeChase,
    EngagePlayer,
    MeleeIdle,
    TauntState,
    AiFlankState
}

public interface IAiState
{
    AiStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}
