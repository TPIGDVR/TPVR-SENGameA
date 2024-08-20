using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCheck : MonoBehaviour
{
    GameObject go;
    [SerializeField]
    bool IsRight;

    private void Awake()
    {
        if (IsRight)
        {
            EventSystem.player.AddListener(PlayerEvents.GRAB_DOWN_RIGHT, Retrieve);
        }
        else
        {
            EventSystem.player.AddListener(PlayerEvents.GRAB_DOWN_LEFT, Retrieve);
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        go = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        go = null;
    }

    private GameObject Retrieve()
    {
        return go;
    }


   
}



