using UnityEngine;

public class gun : MonoBehaviour
{
    public GameObject bullet;
    public Transform BulletTransform;

    public float StartTimeFire;
    private float TimeFire;
    void Start()
    {
        TimeFire = StartTimeFire;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (TimeFire <= 0)
            {
                Instantiate(bullet, BulletTransform.position, transform.rotation);
                TimeFire = StartTimeFire;



            }
            else
            {
                TimeFire -= Time.deltaTime;
            }

        }
    }
}
