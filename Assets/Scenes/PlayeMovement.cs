using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private float moveSpeed = 5f; // Скорость движения
    private Rigidbody2D rb; // Для физики движения

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Получаем ввод с клавиатуры (горизонтальное движение)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Двигаем персонажа
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Проверяем, движется ли персонаж
        bool isWalking = moveInput != 0;
        animator.SetBool("isWalking", isWalking);

        // Поворачиваем спрайт в сторону движения
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Спрайт смотрит вправо
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Спрайт смотрит влево
        }
    }
}