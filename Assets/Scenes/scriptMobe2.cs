using UnityEngine;

public class WanderingEnemy2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;          // Скорость перемещения
    public float minPauseTime = 0.5f;     // Минимальное время паузы
    public float maxPauseTime = 2f;       // Максимальное время паузы

    [Header("Movement Area")]
    public Vector2 areaCenter;            // Центр области перемещения
    public float areaWidth = 5f;          // Ширина области
    public float areaHeight = 3f;         // Высота области

    private Vector2 targetPosition;       // Текущая целевая позиция
    private bool isMoving = true;         // Флаг движения
    private float pauseTimer = 0f;        // Таймер паузы
    private SpriteRenderer spriteRenderer; // Для отражения спрайта

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GenerateNewTarget();
    }

    void Update()
    {
        if (isMoving)
        {
            // Перемещаемся к цели
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Поворачиваем спрайт по направлению движения
            if (targetPosition.x > transform.position.x)
                spriteRenderer.flipX = false;
            else if (targetPosition.x < transform.position.x)
                spriteRenderer.flipX = true;

            // Проверка достижения цели
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                StartPause();
            }
        }
        else
        {
            // Обработка паузы
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isMoving = true;
                GenerateNewTarget();
            }
        }
    }

    // Генерация новой цели в заданной области
    void GenerateNewTarget()
    {
        float randomX = Random.Range(areaCenter.x - areaWidth / 2, areaCenter.x + areaWidth / 2);
        float randomY = Random.Range(areaCenter.y - areaHeight / 2, areaCenter.y + areaHeight / 2);

        targetPosition = new Vector2(randomX, randomY);
    }

    // Начало паузы
    void StartPause()
    {
        isMoving = false;
        pauseTimer = Random.Range(minPauseTime, maxPauseTime);
    }

    // Визуализация области в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(areaCenter, new Vector3(areaWidth, areaHeight, 0.1f));

        // Границы области
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