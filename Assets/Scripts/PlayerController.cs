using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float runSpeed = 3f;
    public float jumpSpeed = 2f;
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
        float hAxis = Input.GetAxis("Horizontal");

        float direction = runSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(0f, 0f, direction * hAxis);

        Vector3 newPosition = transform.position + transform.TransformDirection(movement);

        float sizeY = col.bounds.size.y;
        Vector3 bottomPoint = transform.position + new Vector3(0f, -sizeY / 2 + 0.05f, 0f);
        Vector3 feetPoint = transform.position + transform.up * (-sizeY/2 + 0.05f);
        Debug.Log(feetPoint);

        RaycastHit hitDown;
        RaycastHit hitNormal;
        bool castDown = Physics.Raycast(bottomPoint, Vector3.down, out hitDown, 1f);
        bool castNormal = Physics.Raycast(feetPoint, -transform.up, out hitNormal, 1f);
        if (castNormal)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal.normal);
        }
        if (castDown)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitDown.normal);
        }

        rb.MovePosition(newPosition);
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }
}
