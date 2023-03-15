using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    public bool isDead = false;
    private Vector3 cameraOffset;
    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            transform.position = player.position + cameraOffset;
        }
    }
}
