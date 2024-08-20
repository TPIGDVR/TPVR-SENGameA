using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDetection : MonoBehaviour
{
    bool itemDetection;
    [SerializeField]
    GameObject sunglasses;
    [SerializeField]
    GameObject headphones;

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

    }

}
