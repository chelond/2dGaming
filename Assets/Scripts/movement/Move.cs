using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Flip();
    }

    void FixedUpdate()
    {
       

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
    void Flip()
    {
        if(Input.GetAxis("Horizontal") > 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        if(Input.GetAxis("Horizontal") < 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}

