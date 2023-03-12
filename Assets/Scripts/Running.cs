using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : MonoBehaviour
{
    public float acceleration = 0.046875f;
    public float deceleration = 0.5f;
    public float frictionSpeed = 0.046875f;
    public float topSpeed = 6;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float groundSpeed = rb.velocity.z;

        if (hAxis < 0)
        {
            if (groundSpeed > 0) // Moving to the right
            {
                groundSpeed -= deceleration;
                rb.velocity = new Vector3(0f, 0f, groundSpeed);
                // rb.MovePosition(new Vector3(0f, 0f, groundSpeed));
                if (groundSpeed <= 0)
                {
                    groundSpeed = -0.5f; // Emulate deceleration quirk
                    rb.velocity = new Vector3(0f, 0f, groundSpeed);
                }
            }
            else if (groundSpeed > -topSpeed) // if moving to the left
            {
                groundSpeed -= acceleration;
                rb.velocity = new Vector3(0f, 0f, groundSpeed);
                if (groundSpeed <= -topSpeed)
                {
                    groundSpeed = -topSpeed; // Impose top speed limit
                    rb.velocity = new Vector3(0f, 0f, groundSpeed);
                }
            }
        }

        if (hAxis > 0)
        {
            if (groundSpeed < 0) // If moving to the left
            {
                groundSpeed += deceleration;
                rb.velocity = new Vector3(0f, 0f, groundSpeed);
                if (groundSpeed >= 0)
                {
                    groundSpeed = 0.5f; // Emulate deceleration quirk
                    rb.velocity = new Vector3(0f, 0f, groundSpeed);
                }
            }
            else if (groundSpeed < topSpeed) // if moving to the left
            {
                groundSpeed += acceleration;
                rb.velocity = new Vector3(0f, 0f, groundSpeed);
                if (groundSpeed >= topSpeed)
                {
                    groundSpeed = topSpeed; // Impose top speed limit
                    rb.velocity = new Vector3(0f, 0f, groundSpeed);
                }
            }
        }

        if (hAxis == 0)
        {
            groundSpeed -= Mathf.Min(Mathf.Abs(groundSpeed), frictionSpeed) * Mathf.Sign(groundSpeed); // decelerate with friction
            rb.velocity = new Vector3(0f, 0f, groundSpeed);
        }
    }
}
