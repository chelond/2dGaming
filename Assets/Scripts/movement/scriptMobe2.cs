using UnityEngine;

public class WanderingEnemy2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f; // Новое поле для скорости преследования
    public float minPauseTime = 0.5f;
    public float maxPauseTime = 2f;

    [Header("Movement Area")]
    public Vector2 areaCenter;
    public float areaWidth = 5f;
    public float areaHeight = 3f;

    [Header("Combat")]
    public float detectionRadius = 3f;    // Радиус обнаружения игрока
    public float attackRadius = 1.5f;     // Радиус атаки
    public float attackCooldown = 1f;     // Задержка между атаками
    public int damage = 10;               // Урон моба
    public int maxHealth = 50;            // Максимальное здоровье моба

    private Vector2 targetPosition;
    private bool isMoving = true;
    private float pauseTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private float lastAttackTime;
    private int currentHealth;
    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        GenerateNewTarget();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        // Проверяем расстояние до игрока
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRadius && Time.time >= lastAttackTime + attackCooldown)
            {
                // Атакуем игрока
                AttackPlayer();
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                // Преследуем игрока
                ChasePlayer();
            }
            else
            {
                // Обычное патрулирование
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (isMoving)
        {
            // Движение к цели с обычной скоростью
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Поворачиваем спрайт по направлению движения
            if (targetPosition.x > transform.position.x)
                spriteRenderer.flipX = false;
            else if (targetPosition.x < transform.position.x)
                spriteRenderer.flipX = true;

            // Проверяем достижение цели
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                StartPause();
            }
        }
        else
        {
            // Обработка паузы
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isMoving = true;
                GenerateNewTarget();
            }
        }
    }

    void ChasePlayer()
    {
        // Движение к игроку с увеличенной скоростью
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime // Используем chaseSpeed
        );

        // Поворачиваем спрайт к игроку
        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    void AttackPlayer()
    {
        lastAttackTime = Time.time;
        
        // Наносим урон игроку
        TopDownPlayerController playerController = player.GetComponent<TopDownPlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damage);
        }
        
        Debug.Log($"Enemy attacks player for {damage} damage!");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy takes {damage} damage! Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy defeated!");
        
        // Можно добавить анимацию смерти, эффекты и т.д.
        // Destroy(gameObject, 1f); // Уничтожаем через 1 секунду
        
        // Или просто скрываем объект
        gameObject.SetActive(false);
    }

    void GenerateNewTarget()
    {
        float randomX = Random.Range(areaCenter.x - areaWidth / 2, areaCenter.x + areaWidth / 2);
        float randomY = Random.Range(areaCenter.y - areaHeight / 2, areaCenter.y + areaHeight / 2);

        targetPosition = new Vector2(randomX, randomY);
    }

    void StartPause()
    {
        isMoving = false;
        pauseTimer = Random.Range(minPauseTime, maxPauseTime);
    }

    void OnDrawGizmosSelected()
    {
        // Зона патрулирования
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(areaCenter, new Vector3(areaWidth, areaHeight, 0.1f));

        // Границы зоны
        Gizmos.color = Color.red;
        Vector2 topLeft = new Vector2(areaCenter.x - areaWidth / 2, areaCenter.y + areaHeight / 2);
        Vector2 topRight = new Vector2(areaCenter.x + areaWidth / 2, areaCenter.y + areaHeight / 2);
        Vector2 bottomLeft = new Vector2(areaCenter.x - areaWidth / 2, areaCenter.y - areaHeight / 2);
        Vector2 bottomRight = new Vector2(areaCenter.x + areaWidth / 2, areaCenter.y - areaHeight / 2);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Радиус обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Радиус атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}