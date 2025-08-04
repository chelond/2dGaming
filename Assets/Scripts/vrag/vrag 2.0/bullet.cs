using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private int damage = 25;         // ���� �� ����
    [SerializeField] private float speed = 15f;       // �������� ������
    [SerializeField] private float lifetime = 2f;     // ����� �� ���������������
    [SerializeField] private LayerMask enemyLayer;    // ���� ������
    [SerializeField] private GameObject hitEffect;    // ������ ���������

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; // ���� ����� ������ �� ����� ��������

        // ��������������� ����� �����
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������, ��� ������ �� �����
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            // ������� ����
            HealthSystem enemyHealth = collision.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // ������� ������ ���������
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // ���������� ����
            Destroy(gameObject);
        }
        // ���������� ��� ��������� � �����������
        else if (!collision.isTrigger && !collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}