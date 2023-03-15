using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float pickuptimer = 0.7f;
    public bool canPickup = false;

    private void Start()
    {
        Invoke("EnablePickup", pickuptimer);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, angle, Space.World);
    }
    public void EnablePickup()
    {
        canPickup = true;
    }
}
