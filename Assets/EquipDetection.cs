using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDetection : MonoBehaviour
{
    [SerializeField]
    Transform p;
    [SerializeField]
    Transform defaultParent;
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

            currentGO = equipment;

            if (equipment.CompareTag("Sunglasses"))
            {
                EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_ON,0.85f);
            }
            else if(equipment.CompareTag("Headphones"))
            {
                GameData.player.IsWearingHeadphones = true;
            }

            currentGO.transform.parent = p;
            Quaternion rotate = Quaternion.Euler(0, 0, 0);
            currentGO.transform.SetLocalPositionAndRotation(Vector3.zero,rotate);
        }
        else
        {
            currentGO.transform.parent = defaultParent;

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
