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
        em_Controller.AddListener(DialogEvents.STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS);
        em_Controller.AddListener(DialogEvents.HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS);
    }
}
