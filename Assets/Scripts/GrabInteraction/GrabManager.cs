using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    GameObject go_L;
    GameObject go_R;

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
        go_R = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_RIGHT);
        if (go_R == null)
        {
            text.text = "right hand Cannot find";
            return;
        }
        text.text = $"Gd_r right hand : {go_R.name?.ToString()}";
        go_R?.GetComponent<Interactable>()?.ShowMesh();
    }

    public void GrabDown_L()
    {
        //text.text = "left hand";
        go_L = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_LEFT);
        if (go_L == null)
        {
            text.text = "left hand Cannot find";
            return;
        }
            
        text.text = $"Gd_L left hand : {go_L.name?.ToString()}";
        go_L?.GetComponent<Interactable>()?.ShowMesh();

    }

    public void GrabUp_R()
    {
        print("GU_R");
        text.text = "GU_R";
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
        if (go_R == null)
            return;
        text.text = $"let go : right hand : {go_R.name}";
        // EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
    }

    public void GrabUp_L()
    {
        print("G_L");
        text.text = "GU_L";

        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);
        if (go_L == null)
            return;
        text.text = $"let go : left hand : {go_L.name}";
        // EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);
    }
}


