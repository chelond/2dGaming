using UnityEngine;

public class SimpleEnemyHealthBar : MonoBehaviour
{
    [SerializeField] private HealthSystem health;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    private Vector3 originalScale;
    private bool isVisible;

    private void Start()
    {
        originalScale = healthBar.localScale;
        healthBar.gameObject.SetActive(false);
        health.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        float healthPercent = (float)health.CurrentHealth / health.MaxHealth;
        healthBar.localScale = new Vector3(originalScale.x * healthPercent, originalScale.y, originalScale.z);

        if (!isVisible && healthPercent < 1f)
        {
            healthBar.gameObject.SetActive(true);
            isVisible = true;
        }

        if (health.IsDead)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (isVisible)
        {
            healthBar.position = transform.position + offset;
            healthBar.rotation = Camera.main.transform.rotation;
        }
    }
}