using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDisabler : MonoBehaviour
{
    [SerializeField] private Transform rightSpot;
    [SerializeField] private Transform leftSpot;
    [SerializeField] private MeshCollider rightLoop;
    [SerializeField] private MeshCollider leftLoop;

    private void OnTriggerEnter(Collider other)
    {
        if(transform.position == rightSpot.position)
        {
            leftLoop.enabled = false;
        }
        else
        {
            rightLoop.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameObject.SetActive(false);
    }
}
