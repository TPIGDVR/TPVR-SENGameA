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
        //text.text = "right hand";
        if (go_R == null)
            return;
        text.text = $"right hand : {go_R.name?.ToString()}";
        go_R = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_RIGHT);
        go_R?.GetComponent<Interactable>()?.ShowMesh();
    }

    public void GrabDown_L()
    {
        //text.text = "left hand";
        if (go_L == null)
            return;
        text.text = $"left hand : {go_L.name?.ToString()}";
        go_L = EventSystem.player.TriggerEvent<GameObject>(PlayerEvents.GRAB_DOWN_LEFT);
        go_L?.GetComponent<Interactable>()?.ShowMesh();

    }

    public void GrabUp_R()
    {
        if (go_R == null)
            return;
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
        text.text = $"let go : right hand : {go_R.name}";
        // EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_RIGHT, go_R);
    }

    public void GrabUp_L()
    {
        if (go_L == null)
            return;
        EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);
        text.text = $"let go : left hand : {go_L.name}";
        // EventSystem.player.TriggerEvent(PlayerEvents.GRAB_UP_LEFT, go_L);
    }
}


