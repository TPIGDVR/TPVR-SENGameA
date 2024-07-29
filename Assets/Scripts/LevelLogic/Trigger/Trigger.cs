using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    public abstract void ActivateTrigger();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ActivateTrigger();
    }
}
