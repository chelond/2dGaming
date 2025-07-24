using UnityEngine;
using UnityEngine.XR;

public class player : MonoBehaviour
{
    Rigidbody2D rd;
    public float speed;
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rb.velocity = Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
    }
}
