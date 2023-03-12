using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicController : MonoBehaviour
{
    public float acceleration = 0.046875f;
    public float deceleration = 0.5f;
    public float frictionSpeed = 0.046875f;
    public float topSpeed = 6;
    public float airDamper = 3f;
    public float jumpSpeed = 2f;
    // The minimum amount of time that isGrounded is kept false after a jump
    public float jumpTime;
    float currentSpeed = 0f;
    public float gravityForce = 0.21875f;
    private float currDownVelocity = 0f;
    bool inLoop = false;
    bool isGrounded = false;
    bool wasGroundedLastFrame = false;
    float lastJumpTime = 0f;
    Vector3 movementVector = Vector3.zero;
    Rigidbody rb;
    CapsuleCollider col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    private void FixedUpdate() {
        if (Time.time - lastJumpTime > jumpTime)
        {
            RotationHandler();
            if (isGrounded || movementVector.y <= 0)
            {
                isGrounded = CheckGrounded();
            }
        }
        GravityHandler();
        HorizontalMovement();
        setVelocity();
    }

    private void RotationHandler()
    {
        float sizeY = col.height;
        Vector3 bottomPoint = transform.position + new Vector3(0f, -sizeY / 2 + 0.05f, 0f);
        Vector3 feetPoint = transform.position + transform.up * (-sizeY/2 + 0.05f);

        RaycastHit hitNormal;
        bool castNormal = Physics.Raycast(feetPoint, -transform.up, out hitNormal, 1f);
        if (castNormal)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal.normal);
        }
    }

    private void Jump()
    {
        isGrounded = false;
        wasGroundedLastFrame = false;
        LeaveGround(jumpSpeed);
        lastJumpTime = Time.time;
    }

    public void LeaveGround(float jumpPower)
    {
        movementVector = currentSpeed * transform.right;
        movementVector += transform.up * jumpPower;
        transform.rotation = Quaternion.identity;
        currentSpeed = 0;
    }
    
    public void TouchGround()
    {
        Vector3 rotatedMoveVector = transform.InverseTransformDirection(movementVector);
        currentSpeed = rotatedMoveVector.x;
    }

    private void setVelocity() 
    {
        if(isGrounded)
        {
            movementVector = transform.right * currentSpeed;
        }
        else
        {
            movementVector += Vector3.up * currDownVelocity + Vector3.right * (currentSpeed / airDamper);
        }
        rb.velocity = movementVector;
    }

    bool CheckGrounded()
    {
        float sizeY = col.height;
        Vector3 feetPoint = transform.position + transform.up * (-sizeY/2 + 0.15f);

        RaycastHit hit;
        bool castNormal = Physics.Raycast(feetPoint, -transform.up, out hit, .3f);
        if(castNormal)
        {
            transform.position = hit.point + transform.up * sizeY/2;
        }
        if(wasGroundedLastFrame && !castNormal)
        {
            LeaveGround(0);
        }
        else if (!wasGroundedLastFrame && castNormal)
        {
            TouchGround();
        }
        wasGroundedLastFrame = castNormal;
        Debug.DrawRay(feetPoint, -transform.up * .3f, Color.red, 0f, true);
        return castNormal;
    }

    private void GravityHandler()
    {
        if(!isGrounded)
        {
            currDownVelocity -= gravityForce;
        }
        else 
        {
            currDownVelocity = 0;
        }
    }

    void HorizontalMovement()
    {
        float hAxis = Input.GetAxis("Horizontal");

        if (hAxis < 0)
        {
            if (currentSpeed > 0) // Moving to the right
            {
                currentSpeed -= deceleration;
                if (currentSpeed <= 0)
                {
                    currentSpeed = -0.5f; // Emulate deceleration quirk
                }
            }
            else if (currentSpeed > -topSpeed) // if moving to the left
            {
                currentSpeed -= acceleration;
                if (currentSpeed <= -topSpeed)
                {
                    currentSpeed = -topSpeed; // Impose top speed limit
                }
            }
        }

        if (hAxis > 0)
        {
            if (currentSpeed < 0) // If moving to the left
            {
                currentSpeed += deceleration;
                if (currentSpeed >= 0)
                {
                    currentSpeed = 0.5f; // Emulate deceleration quirk
                }
            }
            else if (currentSpeed < topSpeed) // if moving to the right
            {
                currentSpeed += acceleration;
                if (currentSpeed >= topSpeed)
                {
                    currentSpeed = topSpeed; // Impose top speed limit
                }
            }
        }

        if (hAxis == 0)
        {
            currentSpeed -= Mathf.Min(Mathf.Abs(currentSpeed), frictionSpeed) * Mathf.Sign(currentSpeed); // decelerate with friction
        }
    }
}
