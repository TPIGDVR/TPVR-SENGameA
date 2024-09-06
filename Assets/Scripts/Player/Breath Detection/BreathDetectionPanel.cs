using BreathDetection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreathDetectionPanel : MonoBehaviour
{
    [Header("UI display Optional")]
    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject breathingInstructionPanel;
    [SerializeField] GameObject completedBreathingPanel;
    [SerializeField] TextMeshProUGUI elapseTimeText;
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] Slider breathingSlider;

    [Header("detector")]
    [SerializeField] BreathingDetection breathDetector;
    
    //we want the x btn in the game
    [SerializeField] OVRInput.Button prevButton = OVRInput.Button.One;
    [SerializeField] OVRInput.Button nxtBtn = OVRInput.Button.Three;

    private void Start()
    {
        EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_BREATHING, EnablePanel);
        instructionPanel.SetActive(true);
        breathingInstructionPanel.SetActive(false);
        completedBreathingPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void EnablePanel()
    {
        gameObject.SetActive(true);
        EventSystem.dialog.RemoveListener(DialogEvents.ACTIVATE_BREATHING, EnablePanel);
        StartCoroutine(StartIntroduction());
    }

    //public void StartBreathingPanel()
    //{
    //    instructionPanel.SetActive(false);
    //    breathingInstructionPanel.SetActive(true);
    //    StartCoroutine(StartBreathingPanel());   
    //}

    //called testing here.
    IEnumerator StartIntroduction()
    {
        int currentIndex = 0;
        int maxNumberOfPanel = instructionPanel.transform.childCount;

        //close all the panel except the first
        print("This coroutine man");
        ActivatePanel(0);
        print("this coroutine about to start");
        while (currentIndex < maxNumberOfPanel)
        {
            print("hello");
            if (OVRInput.GetDown(nxtBtn))
            {
                currentIndex++;
                if(currentIndex < maxNumberOfPanel)
                {
                    ActivatePanel(currentIndex);
                }
            }
            else if (OVRInput.GetDown(prevButton))
            {
                //ignore it if its 0
                if (currentIndex > 0)
                {
                    currentIndex--;
                    ActivatePanel(currentIndex);
                } 
            }
            //wait for the btn press
            yield return null;
        }

        instructionPanel.SetActive(false);

        StartCoroutine(StartBreathingPanel());

        void ActivatePanel(int index)
        {
            foreach (Transform panel in instructionPanel.transform)
            {
                panel.gameObject.SetActive(false);
            }
            instructionPanel.transform.GetChild(index).gameObject.SetActive(true);
        }
    }
    IEnumerator StartBreathingPanel()
    {
        breathingInstructionPanel.SetActive(true);
        breathDetector.StartTesting();
        while(breathDetector.breathingTestingState != BreathingTestingState.NONE)
        {
            elapseTimeText.text = $"{breathDetector.remainTimingForTesting} Time remaining";
            stateText.text = $"{breathDetector.breathingTestingState.ToString()}";
            breathingSlider.value = breathDetector.NormaliseVolumeForUI;
            yield return null;
        }

        //completed
        stateText.text = "COMPLETED";
        yield return new WaitForSeconds(0.5f);
        //gameObject.SetActive(false);
        //breathDetector.CanRun = true;

        breathingInstructionPanel.SetActive(false);
        StartCoroutine(StartCompletedPanel());

    }

    IEnumerator StartCompletedPanel()
    {
        completedBreathingPanel.SetActive(true);

        int currentIndex = 0;
        int maxNumberOfPanel = completedBreathingPanel.transform.childCount;

        //close all the panel except the first
        ActivatePanel(0);

        while (currentIndex < maxNumberOfPanel)
        {
            if (OVRInput.GetDown(nxtBtn))
            {
                currentIndex++;
                if (currentIndex < maxNumberOfPanel)
                {
                    ActivatePanel(currentIndex);
                }
            }
            else if (OVRInput.GetDown(prevButton))
            {
                //ignore it if its 0
                if (currentIndex == 0) continue;
                currentIndex--;
                ActivatePanel(currentIndex);
            }
            //wait for the btn press
            yield return null;
        }

        //once over. 
        breathDetector.CanRun = true;
        gameObject.SetActive(false);


        void ActivatePanel(int index)
        {
            foreach (Transform panel in completedBreathingPanel.transform)
            {
                panel.gameObject.SetActive(false);
            }
            completedBreathingPanel.transform.GetChild(index).gameObject.SetActive(true);
        }
    }
}
