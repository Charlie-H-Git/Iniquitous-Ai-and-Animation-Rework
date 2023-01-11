using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStateMachine
{
    //List of all available States from AiState Class
    public IAiState[] States;
    //Reference to AiAgent Class
    public AiAgent Agent;
    //Reference to currently active state
    public AiStateId CurrentState;

    //
    public AiStateMachine(AiAgent agent)
    {
        this.Agent = agent;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        States = new IAiState[numStates];
    }

    
    //Registers new state to AiState List
    public void RegisterState(IAiState state)
    {
        int index = (int) state.GetId();
        States[index] = state;
    }

    //Gets the state Id from States List
    public IAiState GetState(AiStateId stateId)
    {
        int index = (int) stateId;
        return States[index];
    }
    
    public void Update()
    {
        //Updates the current state Every Frame
        GetState(CurrentState)?.Update(Agent);
    }

    public void ChangeState(AiStateId newState)
    {
        GetState(CurrentState)?.Exit(Agent);
        CurrentState = newState;
        GetState(CurrentState)?.Enter(Agent);
    }
}
