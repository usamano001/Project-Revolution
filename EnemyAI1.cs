using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour
{
    public float patrolRange = 10f;
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public float patrolWaitTime = 2f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float retreatRange = 20f;
    public float retreatSpeed = 3f;

    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;
    private Vector3 patrolPoint;
    private bool isPatrolling = false;
    private bool isRetreating = false;
    private float patrolWaitTimer = 0f;
    private float lastAttackTime = 0f;
    private bool isActivated = true;

    void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null) Debug.LogError("Player reference is missing.");
        if (agent == null) Debug.LogError("NavMeshAgent component is missing.");
        if (animator == null) Debug.LogError("Animator component is missing.");
    }

    void Start()
    {
        ActivateAI();
    }

    void Update()
    {
        if (!isActivated || player == null || agent == null) return;

        HandleAIState();
        SyncAnimatorWithMovement();
    }

    public void ActivateAI()
    {
        agent.isStopped = false;
        Debug.Log($"{gameObject.name} AI activated.");
    }

    private void HandleAIState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown && !isRetreating)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
        else if (distanceToPlayer <= chaseRange && !isRetreating)
        {
            ChasePlayer();
        }
        else if (!isRetreating)
        {
            Patrol();
        }

        if (isRetreating)
        {
            Retreat();
            if (distanceToPlayer > retreatRange)
            {
                isRetreating = false;
            }
        }
    }

    private void SyncAnimatorWithMovement()
    {
        float velocity = agent.velocity.magnitude;

        // If moving, determine the animation state
        if (velocity > 0.1f)
        {
            if (agent.speed == patrolSpeed) // Patrolling
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isRetreating", false);
                Debug.Log("Patrolling - Walking");
            }
            else if (agent.speed == chaseSpeed) // Chasing
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                animator.SetBool("isRetreating", false);
                Debug.Log("Chasing - Running");
            }
        }
        else // Idle
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isRetreating", false);
            Debug.Log("Idle - No movement");
        }

        // If retreating, set the isRetreating parameter to true
        if (isRetreating)
        {
            animator.SetBool("isRetreating", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            Debug.Log("Retreating");
        }
    }

    private void Patrol()
    {
        if (!isPatrolling)
        {
            isPatrolling = true;
            SetNewPatrolPoint();
        }

        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoint);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                SetNewPatrolPoint();
            }
        }
    }

    private void ChasePlayer()
    {
        isPatrolling = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void Retreat()
    {
        Vector3 retreatDirection = transform.position - player.position;
        Vector3 retreatPoint = transform.position + retreatDirection.normalized * retreatRange;

        agent.speed = retreatSpeed;
        agent.SetDestination(retreatPoint);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("isAttacking");
    }

    private void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
        }
    }
}
