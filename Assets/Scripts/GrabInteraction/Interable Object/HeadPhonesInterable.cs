using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPhonesInterable : Interactable
{
    protected override void OnEquip()
    {
        EventSystem.player.TriggerEvent(PlayerEvents.HEADPHONES_ON);
        GameData.player.IsWearingHeadphones = true;
    }

    protected override void OnUnEquip()
    {
        EventSystem.player.TriggerEvent(PlayerEvents.HEADPHONES_OFF);
        GameData.player.IsWearingHeadphones = false;
    }
}
