using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapLoopController : MonoBehaviour
{
    [SerializeField] private MeshCollider rightLoop;
    [SerializeField] private MeshCollider leftLoop;
    [SerializeField] private GameObject loopDisabler;

    // If loop is true, then the right loop was enabled and left loop disabled
    // If loop is false, then the left loop was enabled and right loop disabled
    bool loop;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        if(Mathf.Abs(otherRb.velocity.x) > 10f)
        {
            loop = rightLoop.enabled ? true : false;
            leftLoop.enabled = true;
            rightLoop.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(loop)
        {
            rightLoop.enabled = false;
        }
        else
        {
            leftLoop.enabled = false;
        }
    }
}
