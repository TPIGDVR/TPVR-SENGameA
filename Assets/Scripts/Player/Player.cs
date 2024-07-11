using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isWearingSunglasses;
    EventManager<PlayerEvents> em_p = EventSystem.player;

    void WearSunglasses()
    {
        isWearingSunglasses = true;
        em_p.TriggerEvent<float>(PlayerEvents.SUNGLASSES_ON, 1f);
    }

    void TakeOffSunglasses()
    {
        isWearingSunglasses = false;
    }

    private void Start()
    {
    }


}
