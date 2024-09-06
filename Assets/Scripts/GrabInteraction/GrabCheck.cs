using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GrabCheck : MonoBehaviour
{
    //public GameObject go;
    [SerializeField]
    bool IsRight;
    [SerializeField] float searchRadius = 0.05f;
    //[SerializeField] int mask = LayerMask.GetMask("Interactables");
    [SerializeField] LayerMask interactableMask;
    private void Start()
    {
        SimpleDebugingScript.Instance.DebugLine($"subscribe");
        if (IsRight)
        {
            EventSystem.player.AddListener(PlayerEvents.GRAB_DOWN_RIGHT, Retrieve);
        }
        else
        {
            EventSystem.player.AddListener(PlayerEvents.GRAB_DOWN_LEFT, Retrieve);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    go = other.gameObject;
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    go = other.gameObject;
    //}


    //private void OnTriggerExit(Collider other)
    //{
    //    go = null;
    //}

    private GameObject Retrieve()
    {
        var collider = Physics.OverlapSphere(transform.position, searchRadius, interactableMask);
        print($"this is running {collider?.Length}");
        SimpleDebugingScript.Instance.DebugLine($"{name} Trying to retrieveing");

        SimpleDebugingScript.Instance.DebugLine($"this is running {collider?.Length}");
        if (collider.Length == 0) return null;
        //get the first one.
        else return collider[0].gameObject;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    //iouytr

}



