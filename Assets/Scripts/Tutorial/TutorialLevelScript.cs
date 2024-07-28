using Assets.Scripts.Automaton;
using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ScriptableObjectManager;

public class TutorialLevelScript : MonoBehaviour
{
    [SerializeField] Level_Door door;
    public static int kioskDownload = 0;
    [SerializeField] int numberOfKioskToOpenDoor = 4;

    [Header("dialog")]
    [SerializeField] DialogueLines[] lines;

    [Header("Automaton")]
    [SerializeField] Tutorial_AutomatonBehaviour[] automatons;

    EventManager<DialogEvents> EM_Dialog = EventSystem.dialog;
    EventManager<TutorialEvents> EM_Tut = EventSystem.tutorial;
    EventManager<PlayerEvents> EM_P = EventSystem.player;

    [Header("Dialogue Triggers")]
    [SerializeField]
    Transform automaton_DialogueTrigger;

    [SerializeField]
    TMP_Text ObjectiveText;

    private void OnEnable()
    {
        EM_Tut.AddListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EM_Tut.AddListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);
    }

    private void OnDisable()
    {
        EM_Tut.RemoveListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EM_Tut.RemoveListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);

    }

    private void Start()
    {
        InitializeTutorial();
    }

    void InitializeTutorial()
    {
        EM_Tut.TriggerEvent(TutorialEvents.INIT_TUTORIAL);
        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");
        print("RUnning this scipt");
        //instantiate all dialogue scriptable object
        foreach (var l in lines)
        {
            print("adding lines");
            AddIntoSOCollection(l);
        }
    }

    void IncrementKioskDownload()
    {
        EM_Dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, (DialogueLines)RetrieveRuntimeScriptableObject(lines[kioskDownload]));
        kioskDownload++;
        if(kioskDownload >= numberOfKioskToOpenDoor)
        {
            Debug.Log("Tutorial Cleared");
            door.LevelCleared();
        }

        //update objective ui
        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");
    }

    void CallClosestAutomatonToDestination(Transform kiosk)
    {
        Tutorial_AutomatonBehaviour closestA = null;
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

        //attach the dialogue trigger to this automaton
        automaton_DialogueTrigger.SetParent(closestA.transform);
        automaton_DialogueTrigger.localPosition = new(0, 0, 0);
    }

    [ContextMenu("finish tutorial")]
    public void CompleteTutorial()
    {
        var em = EventSystem.dialog;
        em.TriggerEvent(DialogEvents.ACTIVATE_HEARTRATE);
        em.TriggerEvent(DialogEvents.ACTIVATE_NOISE_INDICATOR);
        em.TriggerEvent(DialogEvents.ACTIVATE_OBJECTIVE);
    }
}
