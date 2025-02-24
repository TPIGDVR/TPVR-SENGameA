using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPhonesInterable : Interactable
{
    protected override void OnEquip()
    {

        base.OnEquip();
        GameData.player.IsWearingHeadphones = true;
    }

    protected override void OnUnEquip()
    {
        base.Unequip();
        GameData.player.IsWearingHeadphones = false;
    }
}
