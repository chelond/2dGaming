using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 30f;
    public float velocityPower = 0.9f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector3 initialScale;
    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        rb.freezeRotation = true; // Не вращаться при падении
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Проверка на землю
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Физика прыжка: быстрое падение и короткий прыжок
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        Flip();
    }

    void FixedUpdate()
    {
        // Плавное ускорение и замедление
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velocityPower) * Mathf.Sign(speedDiff);

        rb.AddForce(Vector2.right * movement);

        // Ограничение максимальной скорости
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
            rb.linearVelocity = new Vector2(moveSpeed * Mathf.Sign(rb.linearVelocity.x), rb.linearVelocity.y);
    }

    void Flip()
    {
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }
}

