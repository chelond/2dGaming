using UnityEngine;

public class vrag5 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;
    public Transform player;

    [Header("Attack")]
    public int damage = 10;
    public float attackRate = 1f;
    private float nextAttackTime;
    public float attackRange = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // �������� � ������
        if (distance > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // ������� �������
            if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);

            if (animator != null) animator.SetBool("IsMoving", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsMoving", false);

            // �����
            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // ������ �������� �����
        if (animator != null) animator.SetTrigger("Attack");

        // ��������� �����
        
        
    }

    // ������������ ��� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}