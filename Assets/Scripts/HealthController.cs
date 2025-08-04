using TMPro;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);

    [Header("References")]
    [SerializeField] private TextMeshPro healthText;

    private int currentHealth;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        UpdateHealthDisplay();

        // јвтоматическое создание текста если не задан
        if (healthText == null)
        {
            CreateHealthText();
        }
    }

    void LateUpdate()
    {
        if (healthText != null && mainCamera != null)
        {
            // ѕозиционирование и поворот текста
            healthText.transform.position = transform.position + healthBarOffset;
            healthText.transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthDisplay();

        if (currentHealth <= 0) Die();
    }

    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            UpdateTextColor();
        }
    }

    private void UpdateTextColor()
    {
        float ratio = (float)currentHealth / maxHealth;
        healthText.color = ratio > 0.5f ? Color.green :
                          ratio > 0.25f ? Color.yellow : Color.red;
    }

    private void CreateHealthText()
    {
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = healthBarOffset;

        healthText = textObj.AddComponent<TextMeshPro>();
        healthText.alignment = TextAlignmentOptions.Center;
        healthText.fontSize = 4;
        healthText.sortingOrder = 100;
    }

    private void Die()
    {
        if (healthText != null) Destroy(healthText.gameObject);
        Destroy(gameObject);
    }
}