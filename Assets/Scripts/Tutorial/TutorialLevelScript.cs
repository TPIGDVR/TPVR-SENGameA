using Assets.Scripts.Automaton;
using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelScript : MonoBehaviour
{
    [SerializeField] Level_Door door;
    public static int kioskDownload = 0;
    [SerializeField] int numberOfDoorToOpenDoor = 4;

    [Header("dialog")]
    [SerializeField] DialogueLines[] lines;

    [Header("Automaton")]
    [SerializeField] AutomationBehaviourTutorial[] automatons;

    EventManager<DialogEvents> EM_Dialog = EventSystem.dialog;

    private void OnEnable()
    {
        EventSystem.tutorial.AddListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EventSystem.tutorial.AddListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);

    }

    private void OnDisable()
    {
        EventSystem.tutorial.RemoveListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EventSystem.tutorial.RemoveListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);

    }

    void IncrementKioskDownload()
    {
        EM_Dialog.TriggerEvent(DialogEvents.ADD_DIALOG, lines[kioskDownload]);
        kioskDownload++;
        if(kioskDownload >= numberOfDoorToOpenDoor)
        {
            door.LevelCleared();
        }
        else if(kioskDownload == 1)
        {

        }
    }

    void CallClosestAutomatonToDestination(Transform kiosk)
    {
        AutomationBehaviourTutorial closestA = null;
        float closestDist = float.MaxValue;
        foreach (var a in automatons)
        {
            var dist = Vector3.Distance(kiosk.position, a.transform.position);
            if (dist < closestDist)
            {
                closestA = a;
                closestDist = dist;
            }
        }

        closestA.targetDestination = kiosk.position;
        closestA.SwitchToTarget();
    }

}
