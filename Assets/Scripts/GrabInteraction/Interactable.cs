using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    float elapseTime = 0f;
    [SerializeField] float equipingDuration = 5f;


    public float equipingDistance = 1f;
    public GameObject mesh;
    public Rigidbody rb;
    [SerializeField] private GameObject descriptionUI;
    [SerializeField] EquipDetection currentEquipDetection;

    //to be called by the eqipment detection

    Coroutine cooldown;

    public void Equip()
    {
        mesh.SetActive(false);
        rb.isKinematic = true;
        OnEquip();

        //restart the cooldown
        cooldown = StartCoroutine(StartCoolDown());
    }

    public void Unequip()
    {
        if (elapseTime >= equipingDuration)
        {
            //hide the item
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            mesh.SetActive(true);
            rb.isKinematic = false;
            currentEquipDetection = null;
            //make sure to stop the cooldown
            StopCoroutine(cooldown);
        }
        OnUnEquip();
    }

    public void Grab()
    {
        rb.isKinematic = true;
        mesh.SetActive(true);
        RemoveEquipmentDetection();
    }

    private void RemoveEquipmentDetection()
    {
        //unequip the object from the current equip detection
        if (currentEquipDetection != null) 
        {
            print("remove current equip detection from list");
            currentEquipDetection.UnequipCurrentEquipment();
            currentEquipDetection = null;
        }
    }

    public void UnGrab()
    {
        //try to find the equipment detection
        var colliders = Physics.OverlapSphere(transform.position, equipingDistance);
        print($"Colliders found with count {colliders.Length}");
        foreach(var collider in colliders)
        {
            print($"{collider.name} with tag {collider.tag} {collider.gameObject.name}");
            if(collider.tag == "Player Head")
            {
                //after finding it, equip it and then stop it
                if (collider.TryGetComponent<EquipDetection>(out var rt_currentEquipDetection))
                {
                    currentEquipDetection = rt_currentEquipDetection;
                    currentEquipDetection.Equip(this);
                    return;
                }
            }
        }
        print("unequiping");

        //else then
        rb.isKinematic = false;
        mesh.SetActive(true);
    }


    public void OnSet()
    {
        descriptionUI.gameObject.SetActive(true);
    }
    
    public void OnUnSet()
    {
        descriptionUI.gameObject.SetActive(false);
    }

    protected virtual void OnEquip()
    {
    }

    protected virtual void OnUnEquip() 
    { 
    }
    IEnumerator StartCoolDown()
    {
        while (elapseTime < equipingDuration)
        {
            yield return null;
            elapseTime += Time.deltaTime;
            print("counting");
        }

        //force to be remove from player
        RemoveEquipmentDetection();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, equipingDistance);
    }

}
