using UnityEngine;
using System; // Не забудьте добавить для работы с Action

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject deathEffect;

    // Добавляем событие для отслеживания изменений здоровья
    public event Action OnHealthChanged;

    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged(); // Уведомляем при инициализации
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        NotifyHealthChanged();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        NotifyHealthChanged();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        NotifyHealthChanged();

        if (CompareTag("Player"))
        {
            Debug.Log("Player died!");
            // Здесь перезагрузка сцены или экран смерти
        }
        else if (CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    // Уведомляем подписчиков об изменении здоровья
    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }
}