using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    GameObject go_L;
    GameObject go_R;

    public void GrabDown()
    {
        go_R = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_RIGHT);
        go_L = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_LEFT);
        go_R?.GetComponent<Interactable>()?.ShowMesh();
        go_L?.GetComponent<Interactable>()?.ShowMesh();
    }


    public void GrabUp_R()
    {
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
    }

    public void GrabUp_L()
    {

        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);
    }
}


