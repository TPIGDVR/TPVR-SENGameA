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
        text.text = $"item detected {other.name}";
        itemDetection = true;
    }

    private void OnTriggerExit(Collider other)
    {
        text.text = $"item exit detected {other.name}";

        itemDetection = false;
    }


    private void Equip(GameObject equipment)
    {
        //print($"Equiping object {equipment.name}");
        SimpleDebugingScript.Instance.DebugLine($"Equiping object {equipment.name}");
        if (equipment == null)
        {
            SimpleDebugingScript.Instance.DebugLine($"RUnning null");
            return;
        }

        StartCoroutine(EQ(equipment));
    }
    IEnumerator EQ(GameObject equipment)
    {
        SimpleDebugingScript.Instance.DebugLine($"Runnign coroutine {equipment}");

        if (itemDetection)
        {
            print($"thing being equip {equipment.name}");
            SimpleDebugingScript.Instance.DebugLine($"thing being equip {equipment.name}");

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
            SimpleDebugingScript.Instance.DebugLine($"Completed equiping");
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
