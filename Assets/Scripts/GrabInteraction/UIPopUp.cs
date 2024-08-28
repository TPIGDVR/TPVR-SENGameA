using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [SerializeField]GameObject instructionToolTip;

    private void OnTriggerEnter(Collider other)
    {
        print($"game object enter {other.gameObject.name}");
        if(other.tag == "Hand")
        {
            instructionToolTip.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print($"game object leave {other.gameObject.name}");
        if (other.tag == "Hand")
            instructionToolTip.SetActive(false);
    }
}
