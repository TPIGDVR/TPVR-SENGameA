using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isWearingSunglasses;
    EventManager<PostProcessEvents> ppem = EventSystem.postProcess;

    void WearSunglasses()
    {
        isWearingSunglasses = true;
        ppem.TriggerEvent<float>(PostProcessEvents.SUNGLASSES_ON, 1f);
    }

    void TakeOffSunglasses()
    {
        isWearingSunglasses = false;
    }

    private void Start()
    {
    }


}
