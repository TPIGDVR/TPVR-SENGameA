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
        ppem.TriggerEvent(PostProcessEvents.SUNGLASSES_ON);
    }
}
