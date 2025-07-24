using UnityEngine;
using UnityEngine.XR;
using System.Collections;


public class ggg : MonoBehaviour
{
    Rigidbody2D rb;
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
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
    }
}
