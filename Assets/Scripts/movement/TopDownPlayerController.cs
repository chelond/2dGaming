using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 30f;
    public float velocityPower = 0.9f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private float lastAttackTime;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        lastMoveDirection = Vector2.down; // Начальное направление
    }

    void Update()
    {
        // Получаем ввод для движения
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized; // Нормализуем для равномерной скорости по диагонали

        // Сохраняем последнее направление движения для атаки
        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }

        // Атака
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }

        // Передаем данные в аниматор
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }
    }

    void FixedUpdate()
    {
        // Плавное движение с ускорением и замедлением
        Vector2 targetVelocity = moveInput * moveSpeed;
        Vector2 velocityDifference = targetVelocity - rb.linearVelocity;
        float accelerationRate = (moveInput.magnitude > 0.01f) ? acceleration : deceleration;
        Vector2 movement = Mathf.Pow(Mathf.Abs(velocityDifference.magnitude) * accelerationRate, velocityPower) * velocityDifference.normalized;

        rb.AddForce(movement);

        // Ограничение максимальной скорости
        if (rb.linearVelocity.magnitude > moveSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Проверяем, есть ли враги в зоне атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Здесь можно добавить логику урона
            Debug.Log($"Hit {enemy.name}");
            
            // Пример: если у врага есть компонент Enemy
            // enemy.GetComponent<Enemy>()?.TakeDamage(damage);
        }

        // Запускаем анимацию атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // Визуализация зоны атаки в редакторе
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}