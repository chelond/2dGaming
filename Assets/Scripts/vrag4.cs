using UnityEngine;

public class vrag4 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float minPatrolDistance = 3f;
    public float maxPatrolDistance = 7f;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public float visionAngle = 90f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 patrolTarget;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            isChasing = true;
        }
        else if (isChasing)
        {
            // ��������� �������� ����� ��������� � ��������������
            Invoke("StopChasing", 1f);
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool CanSeePlayer()
    {
        Collider2D playerInRadius = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerLayer
        );

        if (playerInRadius != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector2.Angle(transform.up, directionToPlayer);

            // �������� ���� ������
            if (angleToPlayer < visionAngle / 2)
            {
                // �������� �� �����������
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    directionToPlayer,
                    detectionRadius,
                    obstacleLayer
                );

                if (hit.collider == null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        // ������� � ������� ������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Patrol()
    {
        // �������� � ���� ��������������
        Vector2 direction = (patrolTarget - rb.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        // ������� � ����������� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        // �������� ���������� ����
        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
        {
            SetNewPatrolTarget();
        }
    }

    void SetNewPatrolTarget()
    {
        // ��������� ��������� ����� � �������
        patrolTarget = rb.position + Random.insideUnitCircle.normalized *
            Random.Range(minPatrolDistance, maxPatrolDistance);
    }

    void StopChasing()
    {
        isChasing = false;
        SetNewPatrolTarget();
    }

    void OnDrawGizmosSelected()
    {
        // ������������ ������� �����������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ������������ ���� ������
        Vector3 leftRay = Quaternion.Euler(0, 0, visionAngle / 2) * transform.up * detectionRadius;
        Vector3 rightRay = Quaternion.Euler(0, 0, -visionAngle / 2) * transform.up * detectionRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftRay);
        Gizmos.DrawRay(transform.position, rightRay);

        // ������������ ������� ����
        if (Application.isPlaying)
        {
            Gizmos.color = isChasing ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, patrolTarget);
        }
    }
}