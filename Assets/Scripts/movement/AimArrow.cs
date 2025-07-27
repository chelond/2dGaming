using UnityEngine;

public class AimArrow : MonoBehaviour
{
    public Transform player; // Сюда перетащите игрока в инспекторе
    public float offset = 0.5f; // расстояние от центра игрока до стрелки

    void Update()
    {
        if (player == null) return;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - player.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Смещаем стрелку вперед от игрока
        transform.position = player.position + offset * (Vector3)direction;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
