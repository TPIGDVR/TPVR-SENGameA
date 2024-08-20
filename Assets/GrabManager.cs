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
        Debug.Log($"Left : {go_L} Right : {go_R}");
        
    }


    public void GrabUp()
    {

    }
}


