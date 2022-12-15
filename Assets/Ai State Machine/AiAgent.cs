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
using Random = UnityEngine.Random;

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
    public EnemyManager _enemyManager;
    public EnemyAnimatorManager _enemyAnimatorManager;
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
    public float currentRecoveryTime = 0;
    [Header("Chase Values")] 
    public float stoppingDistance = 1f;

    private ResetAnimatorBool _resetAnimatorBool;
    public float rotationSpeed = 15;

    [Header("Attack Variables")] 
    public EnemyAttackAction[] enemyAttacks;

    public EnemyAttackAction currentAttack;
    private void AssignStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiMeleeChase());
        StateMachine.RegisterState(new AiMeleeIdle());
        StateMachine.RegisterState(new AiMeleeAttack());
    }
    private void Awake()
    {
        _enemyRigidbody = GetComponent<Rigidbody>();
        _enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _enemyManager = GetComponent<EnemyManager>();
        navMeshAgent.stoppingDistance = stoppingDistance;
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

    private void HandleRecoveryTimer()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (_enemyManager.isPerformingAction)
        {
            if (currentRecoveryTime <= 0)
            {
                _enemyManager.isPerformingAction = false;
            }
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
        HandleRecoveryTimer();
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
