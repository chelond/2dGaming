using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 30f;
    public float velocityPower = 0.9f;
    [Range(0f, 0.5f)] public float inputDeadzone = 0.1f;

    [Header("Roll/Dash")]
    public float rollForce = 15f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1f;
    public bool rollInvincibility = true;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 20f;
    public float staminaRegenDelay = 1f;
    public float runStaminaCost = 10f;
    public float rollStaminaCost = 25f;

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Effects")]
    public ParticleSystem rollParticle;
    public AudioClip rollSound;
    public AudioClip damageSound;
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private Vector3 initialScale;
    private float lastRollTime;
    private float lastActionTime;
    private bool isRolling = false;
    private bool isRunning = false;
    private float rollTimer = 0f;
    private Vector2 rollDirection;
    private float currentStamina;
    private float currentHealth;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb.freezeRotation = true;
        initialScale = transform.localScale;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        lastMoveDirection = Vector2.down;
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        HandleStamina();
        HandleAnimations();

        // Handle roll
        if (Input.GetKeyDown(KeyCode.Space) && CanRoll())
        {
            StartRoll();
        }

        // Update roll timer
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                EndRoll();
            }
        }
    }

    void FixedUpdate()
    {
        if (isRolling || isDead) return;

        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector2 targetVelocity = moveInput * currentSpeed;
        Vector2 velocityDifference = targetVelocity - rb.linearVelocity;
        float accelerationRate = (moveInput.magnitude > inputDeadzone) ? acceleration : deceleration;
        Vector2 movement = Vector2.ClampMagnitude(velocityDifference * accelerationRate, currentSpeed) * Time.fixedDeltaTime;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);

        if (rb.linearVelocity.magnitude > currentSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
    }

    void HandleInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.magnitude < inputDeadzone)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = moveInput.normalized;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > inputDeadzone && currentStamina > 0;

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }
    }

    void HandleStamina()
    {
        if (isRunning)
        {
            currentStamina = Mathf.Max(0, currentStamina - Time.deltaTime * runStaminaCost);
            lastActionTime = Time.time;
        }
        else if (Time.time >= lastActionTime + staminaRegenDelay)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + Time.deltaTime * staminaRegenRate);
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        float directionX = lastMoveDirection.x;
        float directionY = lastMoveDirection.y;
        animator.SetFloat("DirectionX", directionX);
        animator.SetFloat("DirectionY", directionY);
        animator.SetFloat("Speed", moveInput.magnitude);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsRolling", isRolling);
        animator.SetBool("IsDead", isDead);

        FlipCharacter();
    }

    bool CanRoll()
    {
        return !isRolling && !isDead && Time.time >= lastRollTime + rollCooldown && currentStamina >= rollStaminaCost;
    }

    void StartRoll()
    {
        isRolling = true;
        lastRollTime = Time.time;
        lastActionTime = Time.time;
        rollTimer = rollDuration;
        currentStamina = Mathf.Max(0, currentStamina - rollStaminaCost);

        rollDirection = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection.normalized;
        rb.linearVelocity = rollDirection * rollForce;

        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }

        if (rollParticle != null)
        {
            rollParticle.Play();
        }
        if (audioSource != null && rollSound != null)
        {
            audioSource.PlayOneShot(rollSound);
        }
    }

    void EndRoll()
    {
        isRolling = false;
        rollTimer = 0f;
        rb.linearVelocity = Vector2.zero;

        if (rollParticle != null)
        {
            rollParticle.Stop();
        }
    }

    void FlipCharacter()
    {
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y) && moveInput.x != 0)
        {
            transform.localScale = new Vector3(
                moveInput.x > 0 ? Mathf.Abs(initialScale.x) : -Mathf.Abs(initialScale.x),
                initialScale.y,
                initialScale.z
            );
        }
        else if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || (isRolling && rollInvincibility)) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Player died!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsDead", true);
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Дополнительная логика (например, деактивация коллайдера)
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public bool IsRolling()
    {
        return isRolling;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsDead()
    {
        return isDead;
    }
}