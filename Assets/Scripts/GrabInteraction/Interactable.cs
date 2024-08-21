using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject mesh;
    public Rigidbody rb;

    public void Equip(){
        //mesh.SetActive(false);
        rb.isKinematic = true;
    }

    public void Unequip()
    {
        //mesh.SetActive(true);
        rb.isKinematic = false;
    }
}
