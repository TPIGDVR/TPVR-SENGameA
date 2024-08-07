using Assets.Scripts.Automaton;
using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ScriptableObjectManager;
using Assets.Scripts.Player.Anxiety_Scripts;
using Automaton;

//public class TutorialLevelScript : MonoBehaviour,IScriptLoadQueuer
//{
//    [SerializeField] Level_Door door;
//    public static int kioskDownload = 0;
//    [SerializeField] int numberOfKioskToOpenDoor = 4;

//    [Header("dialog")]
//    [SerializeField] DialogueLines[] lines;

//    [Header("Automaton")]
//    [SerializeField] BaseAutomatonBehaviour[] automatons;
//    [SerializeField] BaseAutomatonBehaviour firstAutomaton;
//    EventManager<DialogEvents> EM_Dialog = EventSystem.dialog;
//    EventManager<PlayerEvents> EM_P = EventSystem.player;

//    [Header("player")]
//    [SerializeField] PlayerAnxietyHandler anxietyHandler;

//    [Header("Dialogue Triggers")]
//    [SerializeField]
//    Transform automaton_DialogueTrigger;
//    [SerializeField]
//    TMP_Text ObjectiveText;

//    [Header("Last kiosk variables")]
//    [SerializeField] float robotsSpeed= 7f;
//    [SerializeField] float anxietySpeed = 7f;
//    Kiosk lastKiosk;
//    [SerializeField]
//    Transform finalKioskTrigger;

//    [Header("GAME OVER")]
//    [SerializeField]
//    Transform playerTransform;
//    [SerializeField]
//    TutorialGameOver gameOver;
//    Vector3 initialPlayerPosition;


//    //original variable
//    float originalRobotsSpeed;
//     float originalAnxietySpeed;
//     Vector3 originalTriggerPosition;
//     Vector3 originalAutomatonPosition;
//    BaseAutomatonBehaviour movedAutomaton;

//    #region INITIALIZATION
//    public void Initialize()
//    {
//        EventSubscribing();

//        EM_Tut.TriggerEvent(TutorialEvents.INIT_TUTORIAL);
//        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");
//        //instantiate all dialogue scriptable object
//        foreach (var l in lines)
//        {
//            AddIntoSOCollection(l);
//        }
//        initialPlayerPosition = playerTransform.position;
//        originalAnxietySpeed = anxietyHandler.AnxietyIncreaseSpeed;
//        originalRobotsSpeed = automatons[0].Agent.speed;

//        firstAutomaton.ChangeState(BaseAutomatonBehaviour.AutomatonStates.IDLE);

//        GameData.ChangeTutorialStatus(true);
//    }

//    void EventSubscribing()
//    {
//        EM_Tut.AddListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
//        EM_Tut.AddListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);
//        EM_Tut.AddListener<Kiosk>(TutorialEvents.LAST_KIOSK, SetUpLastKiosk);
//        EM_Tut.AddListener(TutorialEvents.CHASE_PLAYER, ChasePlayer);
//        EM_Tut.AddListener(TutorialEvents.DEATH_SCREEN_FADED, poop);
//        EM_Tut.AddListener(TutorialEvents.RES_SCREEN_FADED, TutorialRespawn);
//        EM_Dialog.AddListener(DialogEvents.MOVE_FIRST_AUTOMATON, MoveFirstAutomaton);

//    }

//    private void OnDisable()
//    {
//        EM_Tut.RemoveListener(TutorialEvents.ACTIVATE_KIOSK, IncrementKioskDownload);
//        EM_Tut.RemoveListener<Transform>(TutorialEvents.FIRST_KIOSK, CallClosestAutomatonToDestination);
//        EM_Tut.RemoveListener<Kiosk>(TutorialEvents.LAST_KIOSK, SetUpLastKiosk);
//        EM_Tut.RemoveListener(TutorialEvents.CHASE_PLAYER, ChasePlayer);
//    }

//    private void Awake()
//    {
//        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.AUTOMATONS + 1);
//    }

//    #endregion
//    void IncrementKioskDownload()
//    {
//        //EM_Dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, (DialogueLines)RetrieveRuntimeScriptableObject(lines[kioskDownload]));
//        kioskDownload++;

//        initialPlayerPosition = playerTransform.position;

//        if(kioskDownload >= numberOfKioskToOpenDoor)
//        {
//            Debug.Log("Tutorial Cleared");
//            door.MakeDoorOpenable();
//        }

//        //update objective ui
//        EM_P.TriggerEvent<string>(PlayerEvents.OBJECTIVE_UPDATED, $"Kiosk Completed : {kioskDownload}/{numberOfKioskToOpenDoor}");

//        if(kioskDownload == 3)
//        {
//            EM_Tut.TriggerEvent(TutorialEvents.DETERMINE_LAST_KIOSK);
//        }
//    }

//    void CallClosestAutomatonToDestination(Transform kiosk)
//    {
//        print("running call auto");
//        BaseAutomatonBehaviour closestA = null;
//        float closestDist = float.MaxValue;
//        foreach (var a in automatons)
//        {
//            var dist = Vector3.Distance(kiosk.position, a.transform.position);
//            if (dist < closestDist)
//            {
//                closestA = a;
//                closestDist = dist;
//            }
//        }

//        closestA.ChangeToMoveTarget(kiosk.position);
//        //attach the dialogue trigger to this automaton
//        automaton_DialogueTrigger.SetParent(closestA.transform);
//        automaton_DialogueTrigger.localPosition = new(0, 0, 0);
//    }

//    [ContextMenu("finish tutorial")]
//    public void CompleteTutorial()
//    {
//        var em = EventSystem.dialog;
//        em.TriggerEvent(DialogEvents.ACTIVATE_HEARTRATE);
//        em.TriggerEvent(DialogEvents.ACTIVATE_NOISE_INDICATOR);
//        em.TriggerEvent(DialogEvents.ACTIVATE_OBJECTIVE);
//    }

//    void SetUpLastKiosk(Kiosk targetKiosk)
//    {
//        lastKiosk = targetKiosk;
//        originalTriggerPosition = finalKioskTrigger.position;
//        finalKioskTrigger.position = targetKiosk.transform.position;

//        anxietyHandler.AnxietyIncreaseSpeed = anxietySpeed;

//        foreach(var a in automatons)
//        {
//            //a.SetAgentSpeed(robotsSpeed);
//            a.Agent.speed = robotsSpeed;
//        }
//    }

//    void ChasePlayer()
//    {
//        BaseAutomatonBehaviour furtherestA = null;
//        float furthestDist = float.MinValue;
//        foreach (var a in automatons)
//        {
//            var dist = Vector3.Distance(lastKiosk.transform.position, a.transform.position);
//            if (dist > furthestDist)
//            {
//                furtherestA = a;
//                furthestDist = dist;
//            }
//        }
//        originalAutomatonPosition = furtherestA.transform.position;
//        furtherestA.ChangeToMoveTarget(lastKiosk.TargetDestination.position);
//        //Store the reference of the moved automaton so that it can be move back;
//        movedAutomaton = furtherestA;
//    }

//    void poop()
//    {
//        StartCoroutine(TutorialDeath());
//    }

//    IEnumerator TutorialDeath()
//    {

//        yield return null;
//        //set player to death location
//        playerTransform.position = gameOver.deathPoint.position;

//        //reset the scene to just before they scan kiosk 4
//        //ResetScene();
//    }

//    //void ResetScene()
//    //{
//    //    anxietyHandler.AnxietyIncreaseSpeed = originalAnxietySpeed;
//    //    foreach (var a in automatons)
//    //    {
//    //        a.Agent.speed = originalRobotsSpeed;
//    //    }
//    //    print($"reseting scene {originalAutomatonPosition}");
//    //    movedAutomaton.ResetOriginalPostion();
//    //    //movedAutomaton.transform.position = originalAutomatonPosition;
//    //    //finalKioskTrigger.transform.position = originalTriggerPosition;

//    //}

//    void MoveFirstAutomaton()
//    {
//        //firstAutomaton.SwitchToRoam();
//        firstAutomaton.ChangeToWalkToWayPoint();
//    }

//    void TutorialRespawn()
//    {
//        playerTransform.position = initialPlayerPosition;
//    }
//}
