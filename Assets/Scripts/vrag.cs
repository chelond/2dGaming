using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints; // ����� ��������������
    public float moveSpeed = 3f; // �������� ������������
    public float playerDetectionRange = 5f; // ��������� ����������� ������
    public float chaseSpeed = 5f; // �������� ������
    public float rotationSpeed = 10f; // �������� ��������

    private int currentPatrolIndex = 0;
    private Transform player;
    private bool isChasing = false;
    private Rigidbody2D rb;
    private Vector2 movementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ������ � ������ ����� ��������������
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
        }
    }

    void Update()
    {
        // �������� ���������� �� ������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= playerDetectionRange)
        {
            // �������� ��������� ������ (��� �����������)
            if (IsPlayerVisible())
            {
                isChasing = true;
                ChasePlayer();
                return;
            }
        }

        // ���� ����� �� ���������
        isChasing = false;
        Patrol();
    }

    private bool IsPlayerVisible()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer,
            distanceToPlayer
        );

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // �������� � ������� ����� ��������������
        Vector2 targetPosition = patrolPoints[currentPatrolIndex].position;
        movementDirection = (targetPosition - (Vector2)transform.position).normalized;

        // ��������
        rb.linearVelocity = movementDirection * moveSpeed;

        // ������� � ������� ��������
        if (movementDirection != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // �������� ���������� �����
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        movementDirection = directionToPlayer;

        // ��������
        rb.linearVelocity = directionToPlayer * chaseSpeed;

        // ������� � ������
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // ������������ � ���������
    void OnDrawGizmosSelected()
    {
        // ������ �����������
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        // ����� ���� ��������������
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                    if (i > 0 && patrolPoints[i - 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i - 1].position, patrolPoints[i].position);
                    }
                }
            }
            if (patrolPoints.Length > 1 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[0].position, patrolPoints[patrolPoints.Length - 1].position);
            }
        }
    }
}