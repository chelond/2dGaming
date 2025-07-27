using UnityEngine;

public class Gun9 : MonoBehaviour
{
    public GameObject bullet;
    public Transform BulletTransform;
    public float StartTimeFire;

    [Header("Hand Switching")]
    public Transform rightHandPosition;
    public Transform leftHandPosition;
    public float switchSmoothing = 5f;

    private float TimeFire;
    private Camera mainCamera;
    private bool isRightHand = true;
    private Vector3 originalScale;
    private Vector3 flippedScale;

    void Start()
    {
        TimeFire = StartTimeFire;
        mainCamera = Camera.main;

        // Сохраняем оригинальный и отраженный scale
        originalScale = transform.localScale;
        flippedScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
    }

    void Update()
    {
        UpdateWeaponPositionAndRotation();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (TimeFire <= 0)
            {
                ShootTowardsMouse();
                TimeFire = StartTimeFire;
            }
            else
            {
                TimeFire -= Time.deltaTime;
            }
        }
    }

    void UpdateWeaponPositionAndRotation()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Получаем позицию курсора
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Определяем сторону относительно игрока
        Vector3 playerToMouse = mousePosition - transform.parent.position;
        bool mouseOnRight = playerToMouse.x > 0;
        isRightHand = mouseOnRight;

        // Выбираем позицию для оружия
        Transform targetHand = mouseOnRight ? rightHandPosition : leftHandPosition;
        transform.position = Vector3.Lerp(transform.position, targetHand.position, switchSmoothing * Time.deltaTime);

        // Рассчитываем направление
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Применяем поворот
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Применяем отражение через scale
        transform.localScale = isRightHand ? originalScale : flippedScale;
    }

    void ShootTowardsMouse()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Рассчитываем направление всегда от точки выстрела к курсору
        Vector2 direction = (mousePosition - BulletTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Instantiate(bullet, BulletTransform.position, Quaternion.Euler(0, 0, angle));
    }
}