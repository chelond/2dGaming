using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки патрулирования")]
    public Transform[] patrolPoints; // Точки патрулирования
    public float patrolWaitTime = 2f; // Время ожидания на точке

    [Header("Настройки преследования")]
    public float detectionRadius = 10f; // Радиус обнаружения игрока
    public float chaseRadius = 15f; // Радиус прекращения преследования
    public float rotationSpeed = 5f; // Скорость поворота к игроку

    [Header("Настройки атаки")]
    public float attackRange = 2f; // Дистанция атаки
    public float attackCooldown = 2f; // Перезарядка атаки
    public int attackDamage = 10; // Урон атаки

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

        // Начальное состояние
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        // Проверка обнаружения игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerDetected = distanceToPlayer <= detectionRadius;

        // Логика состояний
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

        // Обновление таймера атаки
        attackTimer -= Time.deltaTime;

        // Обновление анимаций
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
        // Остановка передвижения
        agent.isStopped = true;

        // Поворот к игроку
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Атака по таймеру
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        // Здесь должна быть логика нанесения урона
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        // if (playerHealth != null) playerHealth.TakeDamage(attackDamage);

        // Запуск анимации атаки
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

    // Визуализация зон в редакторе
    void OnDrawGizmosSelected()
    {
        // Зона обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Зона преследования
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Зона атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}