using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(NavMeshAgent))]
public class AiAgent : MonoBehaviour
{
    public AiStateId currentState;
    public float faceTowards;
    public NavMeshAgent navMeshAgent;
    public AiStateMachine StateMachine;
    public AiStateId intialState;
    public GameObject player;
    public TMP_Text stateText;
    

    [Header("Idle State Variables")] 
    public float angleFromPlayer;
    public float distanceFromPlayer;
    public bool los;
    public bool canSee;
    public bool inCone;
    public float coneAngle;
    public float detectionDistance;
    public LayerMask combatMask;
    
    private void AssignStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiMeleeChase());
        StateMachine.RegisterState(new AiMeleeIdle());
    }
    private void Awake()
    {
        
        AssignStates();
    }
    
    public void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StateMachine.ChangeState(intialState);
       
    }
    public void TargetVectors()
    {
        Vector3 targetDir = player.transform.position - transform.position;
        Vector3 forward = transform.forward;
        distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        angleFromPlayer =  Vector3.Angle(targetDir, forward);
    }

    public void HandleMoveToPlayer()
    {
        //Angle from player
        
        //If performing action
        //Set the speed Value to 0 by 0.1 over Time.deltatime
        //Deactivate Nav Mesh
        //Else
        //if distance from player is > stopping distance
        //Set Speed Value to 1 by 0.1 over Time.deltatime
        
        //navmesh local position = vector.0
        //navmesh local rotation = quaternion identity
    }

    private void HandleRotationToPlayer()
    {
        //if performing action Rotate Manually (Root Motion)
        // Get direction
        //Set direction y to 0
        //Normalize Direction
        //if direction is comparative to vector 0
        //direction = transform forward
        // new target rotation = quaternion look rotation(direction)
        //tranform rotation = quat slerp(trans.rotation, player.rotation, rotation speed)
        //else
        // RelativeDirection = transform.inverseTransformDirection(navmeshagent.desiredVelocity)
        // navmeshagent = enabled
        //navmeshSetDestination(player.transform.poition)
    }

    // public void OnObjectSpawn()
    // {
    //     _navMeshAgent.enabled = true;
    //     player = GameObject.FindGameObjectWithTag("Player");
    //     playercontroller = FindObjectOfType<CharacterController>();
    //     _playerHealth = FindObjectOfType<PlayerHealth>();
    //     if (isMelee || isBoss)
    //     {
    //         _meleeFist = FindObjectOfType<MeleeFist>();
    //     }
    //     
    //     StateMachine.ChangeState(intialState);
    //     taunt = false;
    // }
    // private void FlankPlayer()
    // {
    //     if (isMelee && StateMachine.currentState == AiStateId.AiFlankState)
    //     {
    //         if (engagementCounter.FlankPlayer.Contains(gameObject))
    //         {
    //             int i = engagementCounter.FlankPlayer.IndexOf(gameObject);
    //             int FlankCount = engagementCounter.FlankPlayer.Count;
    //             FlankPosition = new Vector3(
    //                 player.transform.position.x + FlankRadius * Mathf.Cos(2 * Mathf.PI * i / FlankCount),
    //                 player.transform.position.y,
    //                 player.transform.position.z + FlankRadius * Mathf.Sin(2 * Mathf.PI * i / FlankCount));
    //
    //
    //         }
    //     }
    // }
    void Update()
    {
        currentState = StateMachine.CurrentState;
        StateMachine.Update();
        TargetVectors();
    }
    private void OnDrawGizmos()
    {
        foreach (var corner in navMeshAgent.path.corners)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(corner,0.5f);
        }
        
        Handles.color = Color.red.WithAlphaMultiplied(0.2f);
        Handles.zTest = CompareFunction.LessEqual;
        Handles.DrawSolidArc(transform.position,Vector3.up, transform.forward,coneAngle, detectionDistance);
        Handles.DrawSolidArc(transform.position,Vector3.up, transform.forward,-coneAngle, detectionDistance);
    }
    
}
