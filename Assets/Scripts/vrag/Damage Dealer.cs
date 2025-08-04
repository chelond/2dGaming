using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем наличие нужного слоя
        if (((1 << collision.gameObject.layer) & targetLayers) == 0)
            return;

        HealthSystem health = collision.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}