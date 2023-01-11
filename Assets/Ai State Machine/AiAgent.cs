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

public enum Architype
{
    Melee,
    Assault,
    Tank
}

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(EnemyManager))]
public class AiAgent : MonoBehaviour
{
    #region ======= State Machine Variables =======
    
    public AiStateId currentState;
    public AiStateMachine StateMachine;
    public AiStateId initialState;
    public Architype architype;
    #endregion

    #region ========== Manager Variables ==========
    public EnemyManager _enemyManager;
    public EnemyAnimatorManager _enemyAnimatorManager;
    public GameManager _gameManager;
    public RigWeightHandler rigWeightHandler;
    #endregion
    
    public NavMeshAgent navMeshAgent;
    public GameObject player;
    public Vector3 targetPos;
    public bool chasingCombatPoint;
    public TMP_Text stateText;
    public EnemyHealth enemyHealth;
    
    [Header("Idle State Variables")] 
    public float angleFromTarget;
    public float distanceFromTarget;
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
    public float MaximumAimingRange;
    [Header("Combat Stance Variables")] 
    public float circleRadius;
    public Vector3 target;

    [Header("Ranged Retreat Variables")] 
    public LayerMask hidableLayers;
    public EnemyCoverHandler enemyCoverHandler;
    [Range(-1, 1)] [Tooltip("Lower is a better hiding spot")]
    public float hideSensitivity = 0;
    [Range(0,10)]
    public float MinPlayerDistance = 5;
    
    private void AssignStates()
    {
        switch (architype)
        {
            case Architype.Melee:
                RegisterMeleeStates();
                
                break;
            case Architype.Assault:
                RegisterAssaultStates();
                break;
            case Architype.Tank:
                //Do Thing
                break;
        }
    }

    private void RegisterMeleeStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiMeleeIdle());
        StateMachine.RegisterState(new AiMeleeChase());
        StateMachine.RegisterState(new AiMeleeAttack());
        StateMachine.RegisterState(new AiMeleeCombatStance());
    }

    private void RegisterAssaultStates()
    {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiAssaultIdle());
        StateMachine.RegisterState(new AiAssaultChase());
        StateMachine.RegisterState(new AiAssaultRetreat());
        StateMachine.RegisterState(new AiAssaultCombatStance());
    }
    private void Awake()
    {
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        rigWeightHandler = gameObject.GetComponent<RigWeightHandler>();
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
    public void TargetVectors(Vector3 targetPos, Vector3 origin, Vector3 originForward)
    {
        Vector3 targetDir = targetPos - origin;
        Vector3 forward = originForward;
        distanceFromTarget = Vector3.Distance(targetPos, origin);
        angleFromTarget =  Vector3.Angle(targetDir, forward);
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

    void resetCombatPoint()
    {
        if (chasingCombatPoint)
        {
            if (distanceFromTarget <= 1)
            {
                targetPos = player.transform.position;
                maximumAttackRange = 5;
                chasingCombatPoint = false;
            }
        }
    }

    
    void HealthHandler()
    {
        
        if (enemyHealth.health <= 0)
        {
            //DEAD
        }
    }
    
    void Update()
    {
        //print("Cover Line Of Sight = " + enemyCoverHandler.CheckLineOfSight(player.transform));
        currentState = StateMachine.CurrentState;
        StateMachine.Update();
        TargetVectors(targetPos, transform.position, transform.forward);
        HandleRecoveryTimer();
        _enemyManager.isPerformingAction = _enemyAnimatorManager.anim.GetBool("isInteracting");
        
        switch (architype)
        {
            case Architype.Melee:
                canDoCombo = _enemyAnimatorManager.anim.GetBool("canDoCombo");
                Mathf.Clamp(currentRecoveryTime, 0, 10);
                if (entryAttackBool)
                {
                    entryAttackCounter -= Time.deltaTime;
                    if (entryAttackCounter <= 0)
                    {
                        entryAttackBool = false;
                        entryAttackCounter = 30;
                    }
                }
                
                break;
            case Architype.Assault:
                //Do Thing
                break;
            case Architype.Tank:
                //Do Thing
                break;
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
