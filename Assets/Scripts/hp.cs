using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ������� : MonoBehaviour
{
    private int hp = 100;
    private float timeVozdistvie = 0f;
    private bool ad = false;
    private bool shit = false;
    [SerializeField] private GameObject damageTextPrefab;
    
    void Start()
    {
        Debug.Log("�������� ������ ��� ���������� " + hp);
    }

    public void TakeDamage(int damage)
    {
        if (shit)
        {
            damage /= 2; // ��� � ����� �� ��� ���������� ����� ����� �� 2
            ShowDamageText("���!" + damage, Color.blue);
        }
        else;
        {
            ShowDamageText("-" + damage, Color.red);
        }
        hp -= damage;
        Debug.Log($"��������� ����� : {damage}. �������� : {hp}");

        if (hp <= 0)
        {
            Debug.Log("������ �����");
            hp = 0;
        }
    }
    public void Heal(int amount)
    {
        hp += amount;

        // ������������ �������� �� ������ 100
        if (hp > 100)
        {
            hp = 100;
        }

        Debug.Log($"��������: {amount}. ��������: {hp}");
    }



    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "�������� ���� (20)"))
            TakeDamage(20);


    }
    private void ShowDamageText(string text, Color color)
    {
        if (damageTextPrefab == null) return;

        // ������� ����� ��� ����������
        Vector3 position = transform.position + Vector3.up * 2f;
        GameObject textObj = Instantiate(damageTextPrefab, position, Quaternion.identity);

        // ������������� ����� � ����
        Text damageText = textObj.GetComponentInChildren<Text>();
        if (damageText != null)
        {
            damageText.text = text;
            damageText.color = color;
        }

        // ��������������� ����� 1 �������
        Destroy(textObj, 1f);

        // �������� (�����������)
        StartCoroutine(FloatText(textObj));
    }

    // ������� �������� ���������� ������
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

    // ��������� ��� (ApplyEffect, Update � �.�.) �������� ��� ���������
    // ...
}

