using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private HealthSystem healthSystem;
    private Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player == null || healthSystem.CurrentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // ������� �������� ��������
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(rb.linearVelocity.x),
                1,
                1
            );
        }
    }

    private void AttackPlayer()
    {
        // ������� ���� ��� ��������
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            attackRange,
            playerLayer
        );

        if (playerCollider != null)
        {
            HealthSystem playerHealth = playerCollider.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}