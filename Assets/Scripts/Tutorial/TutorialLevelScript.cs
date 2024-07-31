using Assets.Scripts.Automaton;
using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ScriptableObjectManager;
using Assets.Scripts.Player.Anxiety_Scripts;

public class TutorialLevelScript : MonoBehaviour,IScriptLoadQueuer
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

    [Header("player")]
    [SerializeField] PlayerAnxietyHandler anxietyHandler;

    [Header("Dialogue Triggers")]
    [SerializeField]
    Transform automaton_DialogueTrigger;
    [SerializeField]
    TMP_Text ObjectiveText;

    [Header("Last kiosk variables")]
    [SerializeField] float robotsSpeed= 7f;
    [SerializeField] float anxietySpeed = 7f;
    Tutorial_Kiosk lastKiosk;
    [SerializeField]
    Transform finalKioskTrigger;

    [Header("GAME OVER")]
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    TutorialGameOver gameOver;
    Vector3 initialPlayerPosition;


    //original variable
    [SerializeField] float originalRobotsSpeed;
    [SerializeField]  float originalAnxietySpeed;
    [SerializeField]  Vector3 originalTriggerPosition;
    [SerializeField]  Vector3 originalAutomatonPosition;
    [SerializeField] Tutorial_AutomatonBehaviour movedAutomaton;

    #region INITIALIZATION
    public void Initialize()
    {
        EventSubscribing();

        EM_Tut.TriggerEvent(TutorialEvents.INIT_TUTORIAL);
        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");
        //instantiate all dialogue scriptable object
        foreach (var l in lines)
        {
            AddIntoSOCollection(l);
        }

        originalAnxietySpeed = anxietyHandler.AnxietyIncreaseSpeed;
        originalRobotsSpeed = automatons[0].Agent.speed;

        GameData.ChangeTutorialStatus(true);
    }

    void EventSubscribing()
    {
        EM_Tut.AddListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EM_Tut.AddListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);
        EM_Tut.AddListener<Tutorial_Kiosk>(TutorialEvents.LAST_KIOSK, SetUpLastKiosk);
        EM_Tut.AddListener(TutorialEvents.CHASE_PLAYER, ChasePlayer);
        EM_Tut.AddListener(TutorialEvents.DEATH_SCREEN_FADED, poop);
        EM_Tut.AddListener(TutorialEvents.RES_SCREEN_FADED, TutorialRespawn);
    }

    private void OnDisable()
    {
        EM_Tut.RemoveListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
        EM_Tut.RemoveListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);
        EM_Tut.RemoveListener<Tutorial_Kiosk>(TutorialEvents.LAST_KIOSK, SetUpLastKiosk);
        EM_Tut.RemoveListener(TutorialEvents.CHASE_PLAYER, ChasePlayer);
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.AUTOMATONS + 1);
    }

    #endregion
    void IncrementKioskDownload()
    {
        //EM_Dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, (DialogueLines)RetrieveRuntimeScriptableObject(lines[kioskDownload]));
        kioskDownload++;
        if(kioskDownload >= numberOfKioskToOpenDoor)
        {
            Debug.Log("Tutorial Cleared");
            door.LevelCleared();
        }

        //update objective ui
        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");

        if(kioskDownload == 3)
        {
            EM_Tut.TriggerEvent(TutorialEvents.DETERMINE_LAST_KIOSK);
        }
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

    void SetUpLastKiosk(Tutorial_Kiosk targetKiosk)
    {
        lastKiosk = targetKiosk;
        originalTriggerPosition = finalKioskTrigger.position;
        finalKioskTrigger.position = targetKiosk.transform.position;

        anxietyHandler.AnxietyIncreaseSpeed = anxietySpeed;

        foreach(var a in automatons)
        {
            a.SetAgentSpeed(robotsSpeed);
        }
    }

    void ChasePlayer()
    {
        Tutorial_AutomatonBehaviour furtherestA = null;
        float furthestDist = float.MinValue;
        foreach (var a in automatons)
        {
            var dist = Vector3.Distance(lastKiosk.transform.position, a.transform.position);
            if (dist > furthestDist)
            {
                furtherestA = a;
                furthestDist = dist;
            }
        }
        originalAutomatonPosition = furtherestA.transform.position;
        furtherestA.targetDestination = lastKiosk.TargetDestination.position;
        furtherestA.SwitchToTarget();
        //Store the reference of the moved automaton so that it can be move back;
        movedAutomaton = furtherestA;
    }

    void poop()
    {
        StartCoroutine(TutorialDeath());
    }

    IEnumerator TutorialDeath()
    {
        //set player to death location
        initialPlayerPosition = playerTransform.position;
        yield return null;
        playerTransform.position = gameOver.deathPoint.position;

        //reset the scene to just before they scan kiosk 4
        ResetScene();
    }

    void ResetScene()
    {
        anxietyHandler.AnxietyIncreaseSpeed = originalAnxietySpeed;
        foreach (var a in automatons)
        {
            a.SetAgentSpeed(originalRobotsSpeed);
        }
        print($"reseting scene {originalAutomatonPosition}");
        movedAutomaton.ResetOriginalPostion();
        //movedAutomaton.transform.position = originalAutomatonPosition;
        //finalKioskTrigger.transform.position = originalTriggerPosition;

    }


    void TutorialRespawn()
    {
        playerTransform.position = initialPlayerPosition;
    }
}
