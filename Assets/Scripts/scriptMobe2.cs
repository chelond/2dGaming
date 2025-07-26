using UnityEngine;

public class WanderingEnemy2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;          // �������� �����������
    public float minPauseTime = 0.5f;     // ����������� ����� �����
    public float maxPauseTime = 2f;       // ������������ ����� �����

    [Header("Movement Area")]
    public Vector2 areaCenter;            // ����� ������� �����������
    public float areaWidth = 5f;          // ������ �������
    public float areaHeight = 3f;         // ������ �������

    private Vector2 targetPosition;       // ������� ������� �������
    private bool isMoving = true;         // ���� ��������
    private float pauseTimer = 0f;        // ������ �����
    private SpriteRenderer spriteRenderer; // ��� ��������� �������

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GenerateNewTarget();
    }

    void Update()
    {
        if (isMoving)
        {
            // ������������ � ����
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // ������������ ������ �� ����������� ��������
            if (targetPosition.x > transform.position.x)
                spriteRenderer.flipX = false;
            else if (targetPosition.x < transform.position.x)
                spriteRenderer.flipX = true;

            // �������� ���������� ����
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                StartPause();
            }
        }
        else
        {
            // ��������� �����
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isMoving = true;
                GenerateNewTarget();
            }
        }
    }

    // ��������� ����� ���� � �������� �������
    void GenerateNewTarget()
    {
        float randomX = Random.Range(areaCenter.x - areaWidth / 2, areaCenter.x + areaWidth / 2);
        float randomY = Random.Range(areaCenter.y - areaHeight / 2, areaCenter.y + areaHeight / 2);

        targetPosition = new Vector2(randomX, randomY);
    }

    // ������ �����
    void StartPause()
    {
        isMoving = false;
        pauseTimer = Random.Range(minPauseTime, maxPauseTime);
    }

    // ������������ ������� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(areaCenter, new Vector3(areaWidth, areaHeight, 0.1f));

        // ������� �������
        Gizmos.color = Color.red;
        Vector2 topLeft = new Vector2(areaCenter.x - areaWidth / 2, areaCenter.y + areaHeight / 2);
        Vector2 topRight = new Vector2(areaCenter.x + areaWidth / 2, areaCenter.y + areaHeight / 2);
        Vector2 bottomLeft = new Vector2(areaCenter.x - areaWidth / 2, areaCenter.y - areaHeight / 2);
        Vector2 bottomRight = new Vector2(areaCenter.x + areaWidth / 2, areaCenter.y - areaHeight / 2);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}