using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent), typeof(Collider))]
public class Enemy : LivingEntity
{
    static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    {
        var randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);

        return hit.position;
    }
    private enum State
    {
        Patrol,
        Tracking,
        AttackBegin,
        Attacking
    }
    private State state;

    NavMeshAgent agent;
    Rigidbody rb;
    Animator animator;

    [SerializeField] float speed;
    [SerializeField, Range(0.01f, 2f)] float turnSmoothTime;
    float turnSmoothVelocity;

    [HideInInspector] public LivingEntity targetEntity;
    public LayerMask whatIsTarget; // 추적 대상 레이어

    bool hasTarget => targetEntity != null && !targetEntity.isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
    }
    public void Setup(float health)
    {
        this.maxHealth = health;
        this.health = health;
    }
    private void Start()
    {
        StartCoroutine(UpdatePath());
    }
    private void Update()
    {
        if (isDead) return;
        print("is stopped? " + agent.isStopped + ", State is : " + state);
        if (state == State.Tracking &&
            Vector3.Distance(targetEntity.transform.position, transform.position) <= agent.stoppingDistance)
        {
            BeginAttack();
        }
        animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
    }
    private void FixedUpdate()
    {
        if (isDead) return;

        if (state == State.AttackBegin || state == State.Attacking)
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
        print("Disable attack");
        state = State.Tracking;
        agent.isStopped = false;
    }
    public void Idling()
    {
        state = State.Patrol;
    }
    public override void Die()
    {
        base.Die();

        agent.enabled = false;

        animator.applyRootMotion = true;
        //animator.SetTrigger("Die");
        animator.SetBool("IsDead", true);

        GetComponent<Collider>().enabled = false;
        //
        print("Enemy Died!");
    }
    private IEnumerator UpdatePath()
    {
        while (!isDead)
        {
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    agent.speed = speed;
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
                    agent.speed = speed;
                }

                if (agent.remainingDistance <= 1f)
                {
                    var patrolPosition = GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                    agent.isStopped = false;
                }

                var colliders = Physics.OverlapSphere(transform.position, 100f, whatIsTarget);

                foreach(var collider in colliders)
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

        if (isDead) return;

        // 공격 중이 아니면 피격 애니메이션
        if (state != State.AttackBegin && state != State.Attacking)
        {
            DisableAttack();
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");
        }
        if (targetEntity == null) targetEntity = damageMessage.damager.GetComponent<LivingEntity>();
        //print("Enemy hit! hp is : " + this.health);
        //
    }
}
