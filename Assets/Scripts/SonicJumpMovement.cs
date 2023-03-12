using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicJumpMovement : MonoBehaviour
{
    public float jumpForce = 6.5f;
    public float gravityForce = 0.21875f;
    bool pressedJump = false;
    public float jumpCooldown = 0.01f;
    float lastJumpTime = 0;
    Rigidbody rb;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        float sizeY = col.bounds.size.y;
        Vector3 feetPoint = transform.position + transform.up * (-sizeY/2 + 0.05f);

        RaycastHit hitNormal;
        float groundAngleNormal = 0f;
        bool castNormal = Physics.Raycast(feetPoint, -transform.up, out hitNormal, 1f);
        if (castNormal)
        {
            groundAngleNormal = Vector3.Angle(hitNormal.normal, Vector3.forward) - 90;
        }
       
        Debug.Log("groundAngleNormal" + groundAngleNormal);

        float jAxis = Input.GetAxis("Jump");
        float xSpeed = rb.velocity.z;
        float ySpeed = rb.velocity.y;
        
        bool isGrounded = CheckGrounded();

        if (jAxis > 0f ) //&& Time.time - lastJumpTime > jumpCooldown)
        {
            // lastJumpTime = Time.time;


            if(!pressedJump && isGrounded)
            {
                pressedJump = true;

                xSpeed -= jumpForce * Mathf.Sin(groundAngleNormal * Mathf.Deg2Rad);
                ySpeed += jumpForce * Mathf.Cos(groundAngleNormal * Mathf.Deg2Rad);
                rb.velocity = new Vector3(0f, ySpeed, xSpeed);
            }
            else
            {
                pressedJump = false;
            }
        }

    }

    bool CheckGrounded()
    {
        float sizeY = col.bounds.size.y;
        Vector3 feetPoint = transform.position + transform.up * (-sizeY/2 + 0.05f);

        bool castNormal = Physics.Raycast(feetPoint, -transform.up, 1f);
        return castNormal;
    }
}
