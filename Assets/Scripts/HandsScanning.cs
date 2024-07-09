using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandsScanning : MonoBehaviour
{


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Scannable"))
        {
            Debug.Log("AAA.AAA.AAA.AAAAAA.AAAA.AAA");
        }
    }



}

