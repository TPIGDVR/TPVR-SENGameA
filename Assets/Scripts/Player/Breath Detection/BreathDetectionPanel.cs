using BreathDetection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreathDetectionPanel : MonoBehaviour
{
    [Header("UI display Optional")]
    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject breathingInstructionPanel;
    [SerializeField] TextMeshProUGUI elapseTimeText;
    [SerializeField] TextMeshProUGUI stateText;

    [Header("detector")]
    [SerializeField] BreathingDetection breathDetector;
    //we want the x btn in the game
    [SerializeField] OVRInput.Button nxtBtn = OVRInput.Button.Three;

    private void Start()
    {
        EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_BREATHING, EnablePanel);
        instructionPanel.SetActive(true);
        breathingInstructionPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void EnablePanel()
    {
        gameObject.SetActive(true);
        EventSystem.dialog.RemoveListener(DialogEvents.ACTIVATE_BREATHING, EnablePanel);
        StartCoroutine(WaitForNxtBtnPress());
    }

    public void StartBreathingPanel()
    {
        instructionPanel.SetActive(false);
        breathingInstructionPanel.SetActive(true);
        StartCoroutine(UpdateBreathingPanel());   
    }

    IEnumerator UpdateBreathingPanel()
    {
        breathDetector.StartTesting();
        while(breathDetector.breathingTestingState != BreathingTestingState.NONE)
        {
            elapseTimeText.text = $"{breathDetector.remainTimingForTesting} Time remaining";
            stateText.text = $"{breathDetector.breathingTestingState.ToString()}";
            yield return null;
        }

        //completed
        stateText.text = "COMPLETED";
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        breathDetector.CanRun = true;
    }
    IEnumerator WaitForNxtBtnPress()
    {
        while(true)
        {
            if (OVRInput.GetDown(nxtBtn))
            {
                break;
            }
            //wait for the btn press
            yield return null;
        }
        StartBreathingPanel();
    }
}
