
using UnityEngine;

public class AiFlankState : IAiState
{
    private Vector3 _rightFlank;
    private Vector3 _leftFlank;
    private Vector3 _rearFlank;


    public AiStateId GetId()
    {
        throw new System.NotImplementedException();
    }

    public void Enter(AiAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public void Update(AiAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(AiAgent agent)
    {
        throw new System.NotImplementedException();
    }


    private float _directionValue;
    void NormalizeDirectionValue(AiAgent agent)
    {
        float faceTowards = Vector3.SignedAngle(agent.player.transform.position, agent.transform.forward, Vector3.up);
        float oldRange = 180 - -180;
        float newRange = 1 - -1;
        _directionValue = faceTowards * newRange / oldRange;
    }
    
    
    
        //TODO research goal driven behaviours
        //TODO: Programming Game AI by Example Mat buckland BOOK!!
        //TODO: https://www.yworks.com/yed-live/
        //TODO Model The State Machine in a flow chart to understand the structure better
   
}
