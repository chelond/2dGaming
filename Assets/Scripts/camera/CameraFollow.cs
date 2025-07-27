using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;         // Игрок
    public Vector3 offset = new Vector3(0, 0, -10); // Смещение по Z для 2D

    [Header("Movement Settings")]
    [Range(0.1f, 10f)]
    public float smoothSpeed = 5f;   // Плавность следования (в FixedUpdate)
    [Range(0.1f, 1f)]
    public float smoothInterpolation = 0.5f; // Интерполяция между кадрами

    [Header("Boundary Settings")]
    public Vector2 minBounds = new Vector2(-10, -5);  // Минимальные границы
    public Vector2 maxBounds = new Vector2(10, 5);    // Максимальные границы

    [Header("Zoom Settings")]
    [Range(5f, 20f)]
    public float zoom = 10f;       // Текущий уровень масштабирования
    public float zoomSpeed = 2f;   // Скорость изменения масштабирования
    public float minZoom = 5f;     // Минимальный уровень масштабирования
    public float maxZoom = 15f;    // Максимальный уровень масштабирования

    private Camera cam;
    private Vector3 velocity = Vector3.zero; // Для плавной интерполяции
    private Vector3 targetPosition;          // Целевая позиция для физического обновления

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found on this GameObject!");
        }
        UpdateCameraSize();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is missing! Camera will not follow.");
            return;
        }

        // Вычисление целевой позиции
        targetPosition = target.position + offset;
        targetPosition.z = transform.position.z; // Сохраняем Z для 2D

        // Ограничение позиции камеры
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    void LateUpdate()
    {
        // Плавная интерполяция позиции камеры
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothInterpolation / smoothSpeed,
                Mathf.Infinity,
                Time.deltaTime
            );
        }

        // Обработка масштабирования
        HandleZoom();
        UpdateCameraSize();
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scrollInput * zoomSpeed * Time.deltaTime;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    void UpdateCameraSize()
    {
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, zoomSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target == null)
        {
            Debug.LogWarning("New target is null!");
        }
    }

    public bool HasTarget()
    {
        return target != null;
    }
}