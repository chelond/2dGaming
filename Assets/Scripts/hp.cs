using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class условия : MonoBehaviour
{
    private int hp = 100;
    private float timeVozdistvie = 0f;
    private bool ad = false;
    private bool shit = false;
    [SerializeField] private GameObject damageTextPrefab;
    
    void Start()
    {
        Debug.Log("персонаж создан его сздорровье " + hp);
    }

    public void TakeDamage(int damage)
    {
        if (shit)
        {
            damage /= 2; // как я понял он так сокрращает любой домаг на 2
            ShowDamageText("щит!" + damage, Color.blue);
        }
        else;
        {
            ShowDamageText("-" + damage, Color.red);
        }
        hp -= damage;
        Debug.Log($"полученно урона : {damage}. Здоровье : {hp}");

        if (hp <= 0)
        {
            Debug.Log("игррок погиб");
            hp = 0;
        }
    }
    public void Heal(int amount)
    {
        hp += amount;

        // Максимальное здоровье не больше 100
        if (hp > 100)
        {
            hp = 100;
        }

        Debug.Log($"Вылечено: {amount}. Здоровье: {hp}");
    }



    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Получить урон (20)"))
            TakeDamage(20);


    }
    private void ShowDamageText(string text, Color color)
    {
        if (damageTextPrefab == null) return;

        // Создаем текст над персонажем
        Vector3 position = transform.position + Vector3.up * 2f;
        GameObject textObj = Instantiate(damageTextPrefab, position, Quaternion.identity);

        // Устанавливаем текст и цвет
        Text damageText = textObj.GetComponentInChildren<Text>();
        if (damageText != null)
        {
            damageText.text = text;
            damageText.color = color;
        }

        // Автоуничтожение через 1 секунду
        Destroy(textObj, 1f);

        // Анимация (опционально)
        StartCoroutine(FloatText(textObj));
    }

    // Простая анимация всплывания текста
    private IEnumerator FloatText(GameObject textObj)
    {
        float duration = 0.8f;
        float elapsed = 0;
        Vector3 startPos = textObj.transform.position;

        while (elapsed < duration)
        {
            textObj.transform.position = startPos + Vector3.up * elapsed * 0.5f;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Остальной код (ApplyEffect, Update и т.д.) остается без изменений
    // ...
}

