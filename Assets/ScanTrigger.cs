using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTrigger : MonoBehaviour
{
    [SerializeField]
    Kiosk kiosk;


    private void OnTriggerEnter(Collider other)
    {
        kiosk.ScanStart();
    }


    private void OnTriggerExit(Collider other)
    {
        kiosk.ScanStop();
    }
}
