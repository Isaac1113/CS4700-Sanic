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
    public float slipSpeedThreshold = 3f;
    public float controlLockTimer = 0.5f;
    float lastLockTime = 0f;
    float currentSpeed = 0f;
    public float gravityForce = 0.21875f;
    public float jumpLimit = 4f;
    private float currDownVelocity = 0f;
    bool isGrounded = false;
    bool wasGroundedLastFrame = false;
    bool isJumping = false;
    bool isBall = false;
    bool isSanic = true;
    float lastJumpTime = 0f;
    public HudManager hud;
    public float damageTimer = 1.5f;
    public float lastDamageTime = 0f;
    Vector3 movementVector = Vector3.zero;
    Rigidbody rb;
    SphereCollider col;
    [SerializeField] GameObject sanicGraphics;
    [SerializeField] GameObject ballGraphics;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();

        hud.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") < 0 && !isBall)
        {
            TurnToBall();
        }
        if (Input.GetAxis("Vertical") > 0 && isBall)
        {
            TurnToSanic();
        }
        
        if(Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
        if(Input.GetButtonUp("Jump") && isJumping)
        {
            if (movementVector.y > jumpLimit)
            {
                movementVector.y = jumpLimit;
            }
        }
    }

    private void FixedUpdate() {
        if (Time.time - lastJumpTime > jumpTime)
        {
            if (isGrounded || movementVector.y <= 0)
            {
                isGrounded = CheckGrounded();
            }
            RotationHandler();
        }
        SlipCheck();
        GravityHandler();
        HorizontalMovement();
        setVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Coin")
        {
            // Increase Score
            GameManager.instance.IncreaseScore(1);

            // Refresh the HUD
            hud.Refresh();

            // Destroy the coin
            Destroy(other.gameObject);
        }
        else if(other.gameObject.tag == "Spike")
        {
            if (Time.time - lastDamageTime > damageTimer)
            {
                lastDamageTime = Time.time;
                int currentRings = GameManager.instance.rings;
                Debug.Log(currentRings);
                if(currentRings == 0)
                {
                    // Game Over here
                    Debug.Log("Game Over on Enter");
                }
                else
                {
                    GameManager.instance.LoseAllRings();
                    hud.Refresh();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastDamageTime > damageTimer)
        {
            if(other.gameObject.tag == "Spike")
            {
                Debug.Log("Game Over on Stay");
            }
        }
    }

    private void RotationHandler()
    {
        float sizeY = col.radius * 2;
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
        if(isSanic)
        {
            TurnToBall();
        }
        LeaveGround(jumpSpeed);
        isJumping = true;
    }

    public void LeaveGround(float jumpPower)
    {
        isGrounded = false;
        wasGroundedLastFrame = false;
        lastJumpTime = Time.time;
        movementVector = currentSpeed * transform.right;
        movementVector += transform.up * jumpPower;
        transform.rotation = Quaternion.identity;
        currentSpeed = 0;
        // Debug.Log(movementVector);
    }
    
    public void TouchGround()
    {
        if (isJumping)
        {
            TurnToSanic();
        }
        isJumping = false;
        Vector3 rotatedMoveVector = transform.InverseTransformDirection(movementVector);
        currentSpeed = rotatedMoveVector.x;
    }

    private void setVelocity() 
    {
        if(isGrounded)
        {
            movementVector = transform.right * currentSpeed;
        }
        else // In the air here:
        {
            movementVector += Vector3.up * currDownVelocity;
            if(movementVector.x < 0)
            {
                if(currentSpeed > 0)
                {
                    movementVector += Vector3.right * (currentSpeed / airDamper);
                }
                else
                {
                    movementVector += Mathf.Abs(movementVector.x) < topSpeed ? Vector3.right * (currentSpeed / airDamper) : Vector3.zero;
                }
            }
            else if(movementVector.x > 0)
            {
                if(currentSpeed < 0)
                {
                    movementVector += Vector3.right * (currentSpeed / airDamper);
                }
                else
                {
                    movementVector += Mathf.Abs(movementVector.x) < topSpeed ? Vector3.right * (currentSpeed / airDamper) : Vector3.zero;
                }
            }
        }
        if (Mathf.Abs(currentSpeed) > 0f && Mathf.Abs(currentSpeed) < 0.5f && isBall && isGrounded)
        {
            TurnToSanic();
        }
        rb.velocity = movementVector;
    }

    private void TurnToBall()
    {
        frictionSpeed = frictionSpeed / 2;
        topSpeed = topSpeed * 2;
        isBall = true;
        isSanic = false;
        sanicGraphics.SetActive(false);
        ballGraphics.SetActive(true);
    }

    private void TurnToSanic()
    {
        frictionSpeed = frictionSpeed * 2;
        topSpeed = topSpeed / 2;
        isBall = false;
        isSanic = true;
        sanicGraphics.SetActive(true);
        ballGraphics.SetActive(false);
    }

    private void SlipCheck()
    {
        if (!isGrounded)
        {
            return;
        }

        float sanicRotation = transform.rotation.eulerAngles.z;

        if (sanicRotation >= 45 && sanicRotation <= 315 && Mathf.Abs(currentSpeed) < slipSpeedThreshold)
        {
            if(isBall)
            {
                TurnToSanic();
            }
            Slip();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "tran.rot.eulerAngles.z" + transform.rotation.eulerAngles.z.ToString());
        GUI.Label(new Rect(10, 30, 100, 20), "rb.vel.x" + rb.velocity.x.ToString());
        GUI.Label(new Rect(10, 50, 100, 20), "currSpeed" + currentSpeed.ToString());
    }

    private void Slip()
    {
        Debug.Log("Entered Slip");
        lastLockTime = Time.time;
        LeaveGround(0);
    }

    bool CheckGrounded()
    {
        float sizeY = col.radius * 2;
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

        if (Time.time - lastLockTime < controlLockTimer)
        {
            hAxis = 0;
        }
        if (hAxis < 0) // Inputting to the left
        {
            if (currentSpeed > 0) // Moving to the right
            {
                if (isGrounded && isBall)
                {
                    TurnToSanic();
                }
                currentSpeed -= deceleration;
                if (currentSpeed <= 0)
                {
                    currentSpeed = -0.5f; // Emulate deceleration quirk
                }
            }
            else if (currentSpeed > -topSpeed) // if moving to the left
            {
                if(!(isBall && isGrounded))
                {
                    currentSpeed -= acceleration;
                }
                if (currentSpeed <= -topSpeed)
                {
                    currentSpeed = -topSpeed; // Impose top speed limit
                }
            }
        }

        if (hAxis > 0) // Inputting to the right
        {
            if (currentSpeed < 0) // If moving to the left
            {
                if (isGrounded && isBall)
                {
                    TurnToSanic();
                }
                currentSpeed += deceleration;
                if (currentSpeed >= 0)
                {
                    currentSpeed = 0.5f; // Emulate deceleration quirk
                }
            }
            else if (currentSpeed < topSpeed) // if moving to the right
            {
                if(!(isBall && isGrounded))
                {
                    currentSpeed += acceleration;
                }
                if (currentSpeed >= topSpeed)
                {
                    currentSpeed = topSpeed; // Impose top speed limit
                }
            }
        }

        if (hAxis == 0 || (isBall && isGrounded))
        {
            currentSpeed -= Mathf.Min(Mathf.Abs(currentSpeed), frictionSpeed) * Mathf.Sign(currentSpeed); // decelerate with friction
        }
    }
}
