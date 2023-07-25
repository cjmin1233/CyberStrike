using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    //static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    //{
    //    var randomPos = Random.insideUnitSphere * distance + center;

    //    NavMeshHit hit;

    //    NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);

    //    return hit.position;
    //}
    private enum State
    {
        Patrol,
        Tracking,
        AttackBegin,
        Attacking
    }
    private State state;
    NavMeshAgent agent;
    //Rigidbody rb;
    Animator animator;

    [SerializeField] private float moveSpeed;
    private float moveSpeedMultiplier = 1f;

    [SerializeField, Range(0.01f, 2f)] float turnSmoothTime;
    float turnSmoothVelocity;

    public LivingEntity targetEntity;
    public LayerMask whatIsTarget; // 추적 대상 레이어

    bool hasTarget => targetEntity != null && !targetEntity.isDead;

    public float damageMultiplier = 1f;

    [SerializeField] private EnemyType WhatIsType;

    private EnemyAnimatonEvent enemyAnimatonEvent;
    private Coroutine updatePath;
    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyAnimatonEvent = GetComponentInChildren<EnemyAnimatonEvent>();

        enemyAnimatonEvent.onIdling.AddListener(Idling);
        enemyAnimatonEvent.onEnableAttack.AddListener(EnableAttack);
        enemyAnimatonEvent.onDisableAttack.AddListener(DisableAttack);
        enemyAnimatonEvent.onDieFinish.AddListener(DieFinish);
    }
    protected override void OnEnable()
    {
        health = maxHealth;
        enemyAnimatonEvent.onIdling.RemoveAllListeners();
        enemyAnimatonEvent.onEnableAttack.RemoveAllListeners();
        enemyAnimatonEvent.onDisableAttack.RemoveAllListeners();
        enemyAnimatonEvent.onDieFinish.RemoveAllListeners();

        enemyAnimatonEvent.onIdling.AddListener(Idling);
        enemyAnimatonEvent.onEnableAttack.AddListener(EnableAttack);
        enemyAnimatonEvent.onDisableAttack.AddListener(DisableAttack);
        enemyAnimatonEvent.onDieFinish.AddListener(DieFinish);

        agent.enabled = true;
        SetAgentSpeed(moveSpeed);
        updatePath = StartCoroutine(UpdatePath());

        targetEntity = PlayerControllerFPS.Instance.GetComponent<PlayerHealth>();
    }
    public void Setup(float difficulty)
    {
        isDead = false;
        state = State.Patrol;
        maxHealth = originMaxHealth * difficulty;
        damageMultiplier = difficulty;
        moveSpeedMultiplier = difficulty;

        animator.applyRootMotion = false;
        animator.SetBool("IsDead", false);

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (var collider in childColliders)
        {
            collider.enabled = true;
        }
    }
    private void SetAgentSpeed(float value)
    {
        agent.speed = value * moveSpeedMultiplier;
        agent.isStopped = false;
        animator.SetFloat("MoveSpeed", agent.speed);
        animator.SetFloat("Speed", moveSpeedMultiplier);
    }
    //private void Start()
    //{
    //    updatePath = StartCoroutine(UpdatePath());
    //}
    private void Update()
    {
        if (isDead) return;
        if (hasTarget && state == State.Tracking && Vector3.Distance(targetEntity.transform.position, transform.position) <= agent.stoppingDistance)
        {
            BeginAttack();
        }
    }
    private void FixedUpdate()
    {
        if (isDead) return;

        if ((state == State.AttackBegin || state == State.Attacking) && hasTarget)
        {
            var lookRotation =
                Quaternion.LookRotation(targetEntity.transform.position - transform.position, Vector3.up);
            var targetAngleY = lookRotation.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                ref turnSmoothVelocity, turnSmoothTime);
        }

        if (state == State.Attacking)
        {
            //var direction = transform.forward;
            //var deltaDistance = agent.velocity.magnitude * Time.fixedDeltaTime;
            // 공격
        }
    }

    public void BeginAttack()
    {
        state = State.AttackBegin;

        agent.isStopped = true;
        animator.SetTrigger("Attack");
    }
    public void EnableAttack()
    {
        state = State.Attacking;
    }
    public void DisableAttack()
    {
        state = State.Tracking;
        if(agent.enabled) agent.isStopped = false;
    }
    public void Idling()
    {
        state = State.Patrol;
        print("enemy idling");
    }
    public override void Die()
    {
        base.Die();
        GameManager.Instance.AddScore(maxHealth);

        agent.enabled = false;

        animator.applyRootMotion = true;
        animator.SetBool("IsDead", true);

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (var collider in childColliders)
        {
            collider.enabled = false;
        }
    }
    private IEnumerator UpdatePath()
    {
        while (!isDead)
        {
            print(agent.isStopped);
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    //agent.speed = moveSpeed;
                    SetAgentSpeed(moveSpeed);
                }

                agent.SetDestination(targetEntity.transform.position);
                agent.isStopped = false;
            }
            else
            {
                if (targetEntity != null) targetEntity = null;

                if (state != State.Patrol)
                {
                    state = State.Patrol;
                    //agent.speed = moveSpeed;
                    SetAgentSpeed(moveSpeed);
                }

                if (agent.remainingDistance <= 1f)
                {
                    var patrolPosition = NavMeshUtility.GetRandomPointOnNavmesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                    agent.isStopped = false;
                }

                var colliders = Physics.OverlapSphere(transform.position, 100f, whatIsTarget);

                foreach (var collider in colliders)
                {
                    var livingEntity = collider.GetComponent<LivingEntity>();

                    if (livingEntity != null && !livingEntity.isDead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    public override void TakeDamage(DamageMessage damageMessage)
    {
        base.TakeDamage(damageMessage);

        UiManager.Instance.HitImageUpdate(damageMessage.damageType);
        //print("damage type is : "+damageMessage.damageType);
        if (isDead) return;

        // 공격 중이 아니면 피격 애니메이션
        if (state != State.AttackBegin && state != State.Attacking)
        {
            DisableAttack();
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");
        }
        
        if (targetEntity == null) targetEntity = damageMessage.damager.GetComponent<LivingEntity>();
    }
    public void DieFinish()
    {
        StopCoroutine(updatePath);
        EnemySpawner.Instance.Add2Pool((int)WhatIsType, gameObject);
    }
}
