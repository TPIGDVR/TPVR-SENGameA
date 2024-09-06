using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipDetection : MonoBehaviour
{
    [SerializeField]
    Transform p;
    [SerializeField]
    Transform defaultParent;
    bool itemDetection;
    public TMP_Text text;
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

        StartCoroutine(EQ(equipment));
    }
    IEnumerator EQ(GameObject equipment)
    {
        if (itemDetection)
        {
            
            if (equipment.CompareTag("Sunglasses"))
            {
                EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_ON, 0.85f);
            }
            else if (equipment.CompareTag("Headphones"))
            {
                GameData.player.IsWearingHeadphones = true;
            }

            equipment.transform.parent = p;
            yield return null;
            Quaternion rotate = Quaternion.identity;
            // equipment.transform.SetLocalPositionAndRotation(Vector3.zero,rotate);
            equipment.transform.localPosition = Vector3.zero;
            equipment.transform.localRotation = rotate;
            text.text = $"Equipped : {equipment.name}";
            print("resetting to zero");
            if (equipment != null)
            {
                equipment.GetComponent<Interactable>()?.Equip();
            }
        }
        else
        {
            equipment.transform.parent = defaultParent;

            if (equipment.CompareTag("Sunglasses"))
            {
                EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_OFF);
            }
            else if (equipment.CompareTag("Headphones"))
            {
                GameData.player.IsWearingHeadphones = false;
            }

            if (equipment != null)
            {
                equipment.GetComponent<Interactable>()?.Unequip();
            }

            text.text = $"Unequipped : {equipment.name}";
        }
    }
}
