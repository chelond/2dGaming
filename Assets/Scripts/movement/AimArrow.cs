using UnityEngine;

public class AimArrow : MonoBehaviour
{
    public Transform player; // Сюда перетащите игрока в инспекторе
    public float offset = 0.5f; // расстояние от центра игрока до стрелки
    public Vector2 spriteOffset = Vector2.zero; // смещение относительно центра спрайта игрока

    void Update()
    {
        if (player == null) return;
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z - player.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = (mouseWorld - player.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Смещаем стрелку вперед от игрока с учетом смещения спрайта
        Vector3 targetPosition = player.position + (Vector3)spriteOffset + offset * (Vector3)direction;
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
