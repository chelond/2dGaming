using UnityEngine;

public class scriptMobe : MonoBehaviour
{
    public GameObject left;
    public GameObject up;
    public GameObject down;
    public GameObject rihht;
    public Rigidbody2D Rigidbody;


    public bool isRightDirection;
    public float speed;

    void Start()
    {

    }


    private void Update()
    {
        if (isRightDirection)
        {
            Rigidbody.linearVelocity = Vector2.right * speed;

        }
        else
        {
            Rigidbody.linearVelocity = Vector2.left * speed;

        }
    }
}


