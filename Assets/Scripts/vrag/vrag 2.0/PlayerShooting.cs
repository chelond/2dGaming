using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;      // ����� ������ ����
    [SerializeField] private GameObject bulletPrefab; // ������ ����
    [SerializeField] private float fireRate = 0.2f;   // ����������������

    private float nextFireTime;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // ������� ����
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // ����� ����� �������� ���� ��������
        // AudioManager.Play("Shoot");
    }
}