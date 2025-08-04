using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;      // Точка вылета пуль
    [SerializeField] private GameObject bulletPrefab; // Префаб пули
    [SerializeField] private float fireRate = 0.2f;   // Скорострельность

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

        // Создаем пулю
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Здесь можно добавить звук выстрела
        // AudioManager.Play("Shoot");
    }
}