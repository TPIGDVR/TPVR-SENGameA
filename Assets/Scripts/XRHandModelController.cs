using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRHandModelController : MonoBehaviour
{
    [SerializeField]
    Material materialNormal;
    [SerializeField]
    Material materialHighlight;
    ControllerReference leftHand;
    ControllerReference rightHand;
    EventManager<DialogEvents> em_Controller = EventSystem.dialog;

    [Header("Hand controller")]
    [SerializeField] GameObject LeftHandControllerParent;
    [SerializeField] GameObject RightHandControllerParent;
    IEnumerator Start()
    {
        while (true)
        {
            leftHand = LeftHandControllerParent.GetComponentInChildren<ControllerReference>();
            rightHand = RightHandControllerParent.GetComponentInChildren<ControllerReference>();
            if(leftHand && rightHand)
            {
                break;
            }
            yield return null;
        }
        em_Controller.AddListener(DialogEvents.STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS, AButton_Off);
        em_Controller.AddListener(DialogEvents.HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS, AButton_On);

        //for hologram
        EventSystem.player.AddListener(PlayerEvents.START_PLAYING_HOLOGRAM, HideController);
        EventSystem.player.AddListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, ShowController);
    }

    void HideController()
    {
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
    }

    void ShowController()
    {
        leftHand.gameObject.SetActive(true);
        rightHand.gameObject.SetActive(true);
    }

    void AButton_Off()
    {
        //right hand primary button is A
        rightHand.primaryButton.GetComponent<MeshRenderer>().material = materialNormal;
        //change the thumbstick material to hightlight
        leftHand.thumbstick_reference.GetComponent<MeshRenderer>().material = materialHighlight;
        rightHand.thumbstick_reference.GetComponent<MeshRenderer>().material = materialHighlight;
    }

    void AButton_On()
    {
        //hightlight the A button
        rightHand.primaryButton.GetComponent<MeshRenderer>().material = materialHighlight;
        //change the thumbstick material to hightlight
        leftHand.thumbstick_reference.GetComponent<MeshRenderer>().material = materialNormal;
        leftHand.thumbstick_reference.GetComponent<MeshRenderer>().material = materialNormal;
    }
}
