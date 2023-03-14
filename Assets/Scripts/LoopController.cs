using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopController : MonoBehaviour
{
    [SerializeField] private MeshCollider entryLoop;
    [SerializeField] private MeshCollider exitLoop;

    private void OnTriggerEnter(Collider other)
    {
        entryLoop.enabled = false;
        exitLoop.enabled = true;
    }
}
