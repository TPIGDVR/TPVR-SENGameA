using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    Interactable go_L;
    Interactable go_R;

    public TMP_Text text;

    void Update(){
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            GrabDown_L();
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            GrabDown_R();
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            GrabUp_L();
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            GrabUp_R();
        }
    }

    public void GrabDown_R()
    {
        try
        {
            if (EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_RIGHT)
                .TryGetComponent<Interactable>(out go_R))
            {
                print($"Grabbed {go_R.transform.name}");
                go_R.Grab();
            }
        }
        catch
        {
            print("no gameobject");
        }
    }

    public void GrabDown_L()
    {
        try
        {
            if (EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_LEFT)
                .TryGetComponent<Interactable>(out go_L))
            {
                print($"Grabbed {go_L.transform.name}");
                go_L.Grab();
            }
        }
        catch
        {
            print("no gameobject");
        }
    }

    public void GrabUp_R()
    {
        if(go_R != null)
        {
            go_R.UnGrab();
            go_R = null;
        }
    }

    public void GrabUp_L()
    {
        if (go_L != null)
        {
            go_L.UnGrab();
            go_L = null;
        }
    }
}


