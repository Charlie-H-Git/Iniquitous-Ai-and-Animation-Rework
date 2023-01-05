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
    #region ======= State Machine Variables =======
    
    public AiStateId currentState;
    public AiStateMachine StateMachine;
    public AiStateId initialState;

    #endregion

    #region ========== Manager Variables ==========
    public EnemyManager _enemyManager;
    public EnemyAnimatorManager _enemyAnimatorManager;
    public GameManager _gameManager;
    #endregion
    
    public NavMeshAgent navMeshAgent;
    public GameObject player;
    public TMP_Text stateText;
    
    
    [Header("Idle State Variables")] 
    public float angleFromPlayer;
    public float distanceFromPlayer;
    public bool los;
    public bool inCone;
    public bool canSee;
    public float coneAngle;
    public float detectionDistance;
    public LayerMask combatMask;
    
    
    [Header("Movement Values")]
    private ResetAnimatorBool _resetAnimatorBool;
    public Rigidbody _enemyRigidbody;
    public float rotationSpeed = 15;
    
    [Header("Attack Variables")] 
    public float maximumAttackRange = 1.5f;
    public EnemyAttackAction[] enemyAttacks;
    public bool entryAttackBool;
    public float entryAttackCounter;
    public EnemyAttackAction[] entryAttack;
    public EnemyAttackAction currentAttack;
    public float currentRecoveryTime = 0;
    public bool canDoCombo;
    
    [Header("Combat Stance Variables")] 
    public float circleRadius;
    public Vector3 target;
    private void AssignStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiMeleeChase());
        StateMachine.RegisterState(new AiMeleeIdle());
        StateMachine.RegisterState(new AiMeleeAttack());
        StateMachine.RegisterState(new AiMeleeCombatStance());
    }
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _enemyRigidbody = GetComponent<Rigidbody>();
        _enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _enemyManager = GetComponent<EnemyManager>();
        // navMeshAgent.stoppingDistance = maximumAttackRange;
        AssignStates();
    }
    
    public void Start()
    {
        StateMachine.ChangeState(initialState);
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
    
    
    

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.collider.gameObject.CompareTag("Enemy"))
    //     {
    //         if (!_enemyManager.isPerformingAction)
    //         {
    //             navMeshAgent.enabled = true;
    //         }
    //     }
    // }

    
    void Update()
    {
        currentState = StateMachine.CurrentState;
        StateMachine.Update();
        Mathf.Clamp(currentRecoveryTime, 0, 10);
        TargetVectors();
        HandleRecoveryTimer();
        _enemyManager.isPerformingAction = _enemyAnimatorManager.anim.GetBool("isInteracting");
        canDoCombo = _enemyAnimatorManager.anim.GetBool("canDoCombo");

        if (entryAttackBool)
        {
            entryAttackCounter -= Time.deltaTime;
            if (entryAttackCounter <= 0)
            {
                entryAttackBool = false;
                entryAttackCounter = 30;
            }
        }
        
    }

    private void LateUpdate()
    {
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private Vector3 arcOffsetVector = new Vector3(0, 0.25f, 0); 
    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(circlePlayerPosition, new Vector3(1,1,1));
        if (navMeshAgent != null)
        {
            foreach (var corner in navMeshAgent.path.corners)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(corner,0.25f);
            } 
        }
        Handles.color = Color.red.WithAlphaMultiplied(0.2f);
        Handles.zTest = CompareFunction.LessEqual;
        Handles.DrawSolidArc(transform.position + arcOffsetVector,Vector3.up, transform.forward,coneAngle, detectionDistance);
        Handles.DrawSolidArc(transform.position + arcOffsetVector,Vector3.up, transform.forward,-coneAngle, detectionDistance);
    }
    
}
