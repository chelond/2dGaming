using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("��������� ��������������")]
    public Transform[] patrolPoints; // ����� ��������������
    public float patrolWaitTime = 2f; // ����� �������� �� �����

    [Header("��������� �������������")]
    public float detectionRadius = 10f; // ������ ����������� ������
    public float chaseRadius = 15f; // ������ ����������� �������������
    public float rotationSpeed = 5f; // �������� �������� � ������

    [Header("��������� �����")]
    public float attackRange = 2f; // ��������� �����
    public float attackCooldown = 2f; // ����������� �����
    public int attackDamage = 10; // ���� �����

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    private int currentPatrolIndex;
    private float patrolTimer;
    private float attackTimer;
    private bool isPlayerDetected;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ��������� ���������
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        // �������� ����������� ������
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerDetected = distanceToPlayer <= detectionRadius;

        // ������ ���������
        if (isPlayerDetected)
        {
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer <= chaseRadius)
            {
                ChasePlayer();
            }
            else
            {
                ResumePatrol();
            }
        }
        else
        {
            Patrol();
        }

        // ���������� ������� �����
        attackTimer -= Time.deltaTime;

        // ���������� ��������
        UpdateAnimations();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                MoveToNextPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        // ��������� ������������
        agent.isStopped = true;

        // ������� � ������
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // ����� �� �������
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        // ����� ������ ���� ������ ��������� �����
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        // if (playerHealth != null) playerHealth.TakeDamage(attackDamage);

        // ������ �������� �����
        if (animator != null) animator.SetTrigger("Attack");
    }

    void ResumePatrol()
    {
        agent.isStopped = false;
        MoveToNextPatrolPoint();
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);
            animator.SetBool("IsAttacking", attackTimer > attackCooldown - 0.5f);
        }
    }

    // ������������ ��� � ���������
    void OnDrawGizmosSelected()
    {
        // ���� �����������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ���� �������������
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // ���� �����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}