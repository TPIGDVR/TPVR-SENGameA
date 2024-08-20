using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDetection : MonoBehaviour
{
    [SerializeField]
    Transform p;

    bool itemDetection;

    GameObject currentGO;

    private void Awake()
    {
        EventSystem.player.AddListener<GameObject>(PlayerEvents.GRAB_UP_LEFT, Equip);
        EventSystem.player.AddListener<GameObject>(PlayerEvents.GRAB_UP_RIGHT, Equip);
    }

    private void OnTriggerEnter(Collider other)
    {
        itemDetection = true;
    }

    private void OnTriggerExit(Collider other)
    {
        itemDetection = false;
    }


    private void Equip(GameObject equipment)
    {
        if (equipment == null)
        {
            return;
        }


        if (itemDetection)
        {

            Debug.Log("Triggered");
            currentGO = equipment;


            if (equipment.CompareTag("Sunglasses"))
            {
                print("sunglasses on");
                EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_ON,0.85f);
            }
            else if(equipment.CompareTag("Headphones"))
            {
                GameData.player.IsWearingHeadphones = true;
            }

            currentGO.transform.parent = p;
            currentGO.transform.localPosition = Vector3.zero;
        }

        else
        {
            currentGO.transform.parent = null;

            if (equipment.CompareTag("Sunglasses"))
            {
                EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_OFF);
            }
            else if (equipment.CompareTag("Headphones"))
            {
                GameData.player.IsWearingHeadphones = false;
            }
        }
    }

}
