using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunglassesInterable : Interactable
{
    protected override void OnEquip()
    {
        EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_ON, 0.85f);
    }

    protected override void OnUnEquip()
    {
        EventSystem.player.TriggerEvent(PlayerEvents.SUNGLASSES_OFF);
    }
}
