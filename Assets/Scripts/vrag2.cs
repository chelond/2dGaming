using UnityEngine;
using System.Collections;

public class AdvancedEnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 1.5f;
    public float acceleration = 8f;
    public float deceleration = 10f;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    [Range(0f, 360f)] public float detectionAngle = 90f;
    public float visionCheckInterval = 0.2f;
    public LayerMask obstacleLayers;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waypointThreshold = 0.5f;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 movementDirection;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isReturning = false;
    private Vector2 lastKnownPosition;
    private Coroutine visionCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
        }

        // �������� ������������� �������� ��������� ������
        visionCoroutine = StartCoroutine(PlayerVisionCheck());
    }

    void Update()
    {
        // ������� ������� � ������� ��������
        if (movementDirection != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            ChaseBehavior();
        }
        else if (isReturning)
        {
            ReturnToPatrolBehavior();
        }
        else
        {
            PatrolBehavior();
        }

        ApplyMovementPhysics();
    }

    IEnumerator PlayerVisionCheck()
    {
        while (true)
        {
            CheckForPlayer();
            yield return new WaitForSeconds(visionCheckInterval);
        }
    }

    private void CheckForPlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

        // �������� ���������� � ����
        if (distanceToPlayer <= detectionRadius && angleToPlayer <= detectionAngle * 0.5f)
        {
            // �������� �� ������� �����������
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayers);
            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                isChasing = true;
                isReturning = false;
                lastKnownPosition = player.position;
                return;
            }
        }

        // ���� ����� ��� �����, �� ������ �������
        if (isChasing)
        {
            isChasing = false;
            isReturning = true;
        }
    }

    private void ChaseBehavior()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // ��������� �� ������������ ����������
        if (distanceToPlayer <= stoppingDistance)
        {
            movementDirection = Vector2.zero;
            return;
        }

        movementDirection = directionToPlayer.normalized;
    }

    private void ReturnToPatrolBehavior()
    {
        // �������� ��������� ��������� �������?
        if (Vector2.Distance(transform.position, lastKnownPosition) < waypointThreshold)
        {
            isReturning = false;
            return;
        }

        movementDirection = (lastKnownPosition - (Vector2)transform.position).normalized;
    }

    private void PatrolBehavior()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 targetPosition = patrolPoints[currentPatrolIndex].position;
        movementDirection = (targetPosition - (Vector2)transform.position).normalized;

        // ������������ �� ��������� �����
        if (Vector2.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ApplyMovementPhysics()
    {
        float currentSpeed = isChasing ? chaseSpeed : patrolSpeed;
        Vector2 desiredVelocity = movementDirection * currentSpeed;

        // ������� ��������� ��������
        Vector2 velocityChange = desiredVelocity - rb.linearVelocity;
        velocityChange = Vector2.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

        rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);

        // ������������ ��� ���������� ��������
        if (movementDirection == Vector2.zero && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.AddForce(-rb.linearVelocity * deceleration * Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        // ������ �����������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ���� ������
        Vector2 rightDir = Quaternion.Euler(0, 0, detectionAngle * 0.5f) * transform.right;
        Vector2 leftDir = Quaternion.Euler(0, 0, -detectionAngle * 0.5f) * transform.right;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rightDir * detectionRadius);
        Gizmos.DrawRay(transform.position, leftDir * detectionRadius);

        // ����� ����
        Gizmos.color = Color.green;
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
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

    void OnDestroy()
    {
        if (visionCoroutine != null)
        {
            StopCoroutine(visionCoroutine);
        }
    }
}