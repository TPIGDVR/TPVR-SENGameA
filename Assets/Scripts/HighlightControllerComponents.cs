using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightControllerComponents : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Material materialNormal;
    [SerializeField]
    Material materialHighlight;

    public GameObject thumbstick_left;
    public GameObject thumbstick_right;
    public GameObject button_a;

    EventManager<DialogEvents> em_Controller = EventSystem.dialog;

    //public void Initialize()
    //{
    //    //thumbstick_left = GameObject.Find("Controller Tutorial Left(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
    //    //thumbstick_right = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
    //    //button_a = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().button_a_reference;
    //    //em_Controller.AddListener(DialogEvents.STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS, AButton_Off);
    //    //em_Controller.AddListener(DialogEvents.HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS, AButton_On);
    //}

    //private void Awake()
    //{
    //    //ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.SYSTEM);
    //    //thumbstick_left = GameObject.Find("Controller Tutorial Left(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
    //    //thumbstick_right = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
    //    //button_a = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().button_a_reference;
    //    //em_Controller.AddListener(DialogEvents.STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS, AButton_Off);
    //    //em_Controller.AddListener(DialogEvents.HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS, AButton_On);
    //}

    // THOM PLEASE SAVE US!!!!!!!

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        thumbstick_left = GameObject.Find("Controller Tutorial Left(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
        thumbstick_right = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().thumbstick_reference;
        button_a = GameObject.Find("Controller Tutorial Right(Clone)").GetComponent<ControllerReference>().button_a_reference;
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
