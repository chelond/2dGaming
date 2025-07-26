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
    public int attackDamage = 25;
    public int maxHealth = 100;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private float lastAttackTime;
    private Animator animator;
    private Vector3 initialScale;
    private int currentDirection = 2;
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        lastMoveDirection = Vector2.down;
        initialScale = transform.localScale;
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Получаем ввод для движения
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Сохраняем последнее направление движения
        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }

        // Определяем направление для анимации
        int newDirection = GetDirectionIndex(lastMoveDirection);

        // Передаем данные в аниматор
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
            
            // Принудительно обновляем направление
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                animator.SetInteger("Direction", currentDirection);
                
                // Принудительно переключаем анимацию
                if (moveInput.magnitude > 0.1f)
                {
                    switch (currentDirection)
                    {
                        case 0: // side
                            animator.Play("run_side");
                            break;
                        case 1: // up
                            animator.Play("run_up");
                            break;
                        case 2: // down
                            animator.Play("run_down");
                            break;
                    }
                }
                else
                {
                    switch (currentDirection)
                    {
                        case 0: // side
                            animator.Play("idle_side");
                            break;
                        case 1: // up
                            animator.Play("idle_up");
                            break;
                        case 2: // down
                            animator.Play("idle_down");
                            break;
                    }
                }
            }
        }

        // Поворачиваем персонажа только при движении влево/вправо
        FlipCharacter();

        // Атака
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
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

    // Определяем индекс направления для анимации
    int GetDirectionIndex(Vector2 direction)
    {
        // Определяем основное направление по наибольшей компоненте
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return 0; // side (влево/вправо)
        }
        else if (direction.y > 0)
        {
            return 1; // up
        }
        else
        {
            return 2; // down
        }
    }

    // Поворачиваем персонажа только при движении влево/вправо
    void FlipCharacter()
    {
        // Поворачиваем только если движение преимущественно горизонтальное
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y) && moveInput.x != 0)
        {
            if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            }
        }
        // При движении вверх/вниз возвращаем к нормальному scale
        else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Проверяем, есть ли враги в зоне атаки
        if (attackPoint != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                WanderingEnemy2D enemyScript = enemy.GetComponent<WanderingEnemy2D>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(attackDamage);
                    Debug.Log($"Player attacks {enemy.name} for {attackDamage} damage!");
                }
            }
        }

        // Запускаем анимацию атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player takes {damage} damage! Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player defeated!");
        // Здесь можно добавить логику смерти игрока
        // Например, перезапуск уровня, показ экрана смерти и т.д.
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}