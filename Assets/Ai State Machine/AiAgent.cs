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

public class AiAgent : MonoBehaviour
{
    public AiStateId currentState;
    public float faceTowards;
    public NavMeshAgent navMeshAgent;
    public AiStateMachine StateMachine;
    public AiStateId intialState;
    public GameObject player;
    public TMP_Text stateText;
    private EnemyManager _enemyManager;
    private EnemyAnimatorManager _enemyAnimatorManager;
    public Rigidbody _enemyRigidbody;
    [Header("Idle State Variables")] 
    public float angleFromPlayer;
    public float distanceFromPlayer;
    public bool los;
    public bool canSee;
    public bool inCone;
    public float coneAngle;
    public float detectionDistance;
    public LayerMask combatMask;

    [Header("Chase Values")] 
    public float stoppingDistance = 1f;

    public float rotationSpeed = 15;
    private void AssignStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiMeleeChase());
        StateMachine.RegisterState(new AiMeleeIdle());
    }
    private void Awake()
    {
        _enemyRigidbody = GetComponent<Rigidbody>();
        _enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _enemyManager = GetComponent<EnemyManager>();
        AssignStates();
    }
    
    public void Start()
    {
        StateMachine.ChangeState(intialState);
        navMeshAgent.enabled = false;
        _enemyRigidbody.isKinematic = false;
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

        if (_enemyManager.isPerformingAction)
        {
            //If performing an action set the movement to 0
            _enemyAnimatorManager.anim.SetFloat("Blend", 0 ,0.1f, Time.deltaTime);
            navMeshAgent.enabled = false;
        }
        else
        {
            //if we are not performing an action
            // if the distance from player is greater than stopping distance
            if (distanceFromPlayer > stoppingDistance)
            {
                //Set the movement to 1 over time
                _enemyAnimatorManager.anim.SetFloat("Blend", 1 ,0.1f, Time.deltaTime);
                
            }else if (distanceFromPlayer <= stoppingDistance)
            {
                _enemyAnimatorManager.anim.SetFloat("Blend", 0 ,0.1f, Time.deltaTime);
            }
        }
        
        HandleRotationToPlayer();
        //Set navmesh local transform values to zero
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
        
    }

    private void HandleRotationToPlayer()
    {
        //Rotate Manually
        if (_enemyManager.isPerformingAction)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / Time.deltaTime );
        }
        //Rotate with pathfinding
        else
        {
           Vector3 relativeDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
           Vector3 targetVelocity = _enemyRigidbody.velocity;

           navMeshAgent.enabled = true;
           navMeshAgent.SetDestination(player.transform.position);
           _enemyRigidbody.velocity = targetVelocity;
           transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
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
    
    private Vector3 arcOffsetVector = new Vector3(0, 0.25f, 0); 
    private void OnDrawGizmos()
    {
        if (navMeshAgent != null)
        {
            foreach (var corner in navMeshAgent.path.corners)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(corner,0.1f);
            } 
        }
        Handles.color = Color.red.WithAlphaMultiplied(0.2f);
        Handles.zTest = CompareFunction.LessEqual;
        Handles.DrawSolidArc(transform.position + arcOffsetVector,Vector3.up, transform.forward,coneAngle, detectionDistance);
        Handles.DrawSolidArc(transform.position + arcOffsetVector,Vector3.up, transform.forward,-coneAngle, detectionDistance);
    }
    
}
