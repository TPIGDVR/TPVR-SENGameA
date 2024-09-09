using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPhonesInterable : Interactable
{
    protected override void OnEquip()
    {
        GameData.player.IsWearingHeadphones = true;
    }

    protected override void OnUnEquip()
    {
        GameData.player.IsWearingHeadphones = false;
    }
}
