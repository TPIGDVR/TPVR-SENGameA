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

        if(go_R != null)
        {
            go_R.GetComponent<Interactable>().mesh.SetActive(false);
        }

        if(go_L != null)
        {
            go_L.GetComponent<Interactable>().mesh.SetActive(false);
        }
    }


    public void GrabUp()
    {
        
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);

        if (go_R != null)
        {
            go_R.GetComponent<Interactable>().mesh.SetActive(true);
        }

        if (go_L != null)
        {
            go_L.GetComponent<Interactable>().mesh.SetActive(true);
        }
    }
}


