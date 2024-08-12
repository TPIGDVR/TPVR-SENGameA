using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightControllerComponents : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Material materialNormal;
    [SerializeField]
    Material materialHighlight;

    [SerializeField]
    GameObject thumbstick_left;
    [SerializeField]
    GameObject thumbstick_right;
    [SerializeField]
    GameObject button_a;

    EventManager<DialogEvents> em_Controller = EventSystem.dialog;

    public void Initialize()
    {
        em_Controller.AddListener(DialogEvents.STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS, AButton_Off);
        em_Controller.AddListener(DialogEvents.HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS, AButton_On);
    }

    void AButton_Off()
    {
        button_a.GetComponent<MeshRenderer>().material = materialNormal;
        thumbstick_left.GetComponent<MeshRenderer>().material = materialHighlight;
        thumbstick_right.GetComponent<MeshRenderer>().material = materialHighlight;
    }

    void AButton_On()
    {
        button_a.GetComponent<MeshRenderer>().material = materialHighlight;
        thumbstick_left.GetComponent<MeshRenderer>().material = materialNormal;
        thumbstick_right.GetComponent<MeshRenderer>().material = materialNormal;
    }
}
