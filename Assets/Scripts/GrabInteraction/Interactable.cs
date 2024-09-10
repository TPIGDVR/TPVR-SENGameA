using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isGrab = false;
    public float equipingDistance = 1f;
    public GameObject mesh;
    public Rigidbody rb;
    [SerializeField] EquipDetection currentEquipDetection;

    //to be called by the eqipment detection
    public void Equip()
    {
        mesh.SetActive(false);
        rb.isKinematic = true;
        OnEquip();
    }

    public void Unequip()
    {
        mesh.SetActive(true);
        rb.isKinematic = false;
        OnUnEquip();
    }

    public void Grab()
    {
        isGrab = true;
        rb.isKinematic = true;
        mesh.SetActive(true);
    }

    public void UnGrab()
    {
        //try to find the equipment detection
        var colliders = Physics.OverlapSphere(transform.position, equipingDistance);
        foreach(var collider in colliders)
        {
            if(collider.tag == "Player Head")
            {
                //after finding it, equip it and then stop it
                currentEquipDetection = collider.GetComponent<EquipDetection>();
                currentEquipDetection.Equip(this);
                return;
            }
        }

        //uneqip the object from the current equip detection
        if (currentEquipDetection != null) 
        {
            currentEquipDetection.UnequipCurrentEquipment();
            currentEquipDetection = null;
        }

        //else then
        isGrab = false;
        rb.isKinematic = false;
        mesh.SetActive(true);
    }

    protected virtual void OnEquip()
    {

    }

    protected virtual void OnUnEquip() 
    { 
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, equipingDistance);
    }

}
