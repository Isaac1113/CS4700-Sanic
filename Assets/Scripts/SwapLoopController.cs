using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapLoopController : MonoBehaviour
{
    [SerializeField] private MeshCollider rightLoop;
    [SerializeField] private MeshCollider leftLoop;
    [SerializeField] private GameObject loopDisabler;

    private void OnTriggerEnter(Collider other)
    {
        leftLoop.enabled = true;
        rightLoop.enabled = true;

        loopDisabler.SetActive(true);
    }
}
