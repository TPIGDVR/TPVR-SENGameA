using Assets.Scripts.Player.Anxiety_Scripts;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isWearingSunglasses;

    //AnxietyHandler anxietyHandler;
    PlayerAnxietyHandler anxietyHandler;
    NoiseProximityHandler noiseProximityHandler;

    //references to ui
    [SerializeField]
    GameObject Heart,Objective;

    //room / objective variables
    Room currentRoom;

    private void Awake()
    {
        InitializePlayer();
    }

    #region Initialization
    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;
    EventManager<DialogEvents> em_d = EventSystem.dialog;
    EventManager<LevelEvents> em_l = EventSystem.level;

    void InitializePlayer()
    {
        EventSubscribing();
        GetReferenceToComponents();
    }

    void EventSubscribing()
    {
        em_t.AddListener(TutorialEvents.INIT_TUTORIAL, DeactivateAllMechanic);

        //dialogue events
        em_d.AddListener(DialogEvents.ACTIVATE_HEARTRATE, ActivateHeartRateMechanic);
        em_d.AddListener(DialogEvents.ACTIVATE_OBJECTIVE, ActivateObjectiveMechanic);

        //level events
        em_l.AddListener<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, ProgressObjective);
        em_l.AddListener<Room>(LevelEvents.ENTER_NEW_ROOM, SwitchCurrentRoom);
    }

    void EventUnsubscribing()
    {
        em_t.RemoveListener(TutorialEvents.INIT_TUTORIAL, DeactivateAllMechanic);
        em_d.RemoveListener(DialogEvents.ACTIVATE_HEARTRATE, ActivateHeartRateMechanic);
        em_d.RemoveListener(DialogEvents.ACTIVATE_OBJECTIVE, ActivateObjectiveMechanic);
        em_l.RemoveListener<ObjectiveName>(LevelEvents.OBJECTIVE_COMPLETE, ProgressObjective);
    }

    void GetReferenceToComponents()
    {
        anxietyHandler = GetComponent<PlayerAnxietyHandler>();
        noiseProximityHandler = GetComponent<NoiseProximityHandler>();
    }

    #endregion

    #region UNUSED FOR NOW
    void WearSunglasses()
    {
        isWearingSunglasses = true;
        em_p.TriggerEvent<float>(PlayerEvents.SUNGLASSES_ON, 1f);
    }

    void TakeOffSunglasses()
    {
        isWearingSunglasses = false;
    }
    #endregion
    //Mechanics :
    //Noise Range Indicator
    //Heartrate
    //Level objective

    void ActivateHeartRateMechanic()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.FUTURISTIC_PANEL_OPEN);
        anxietyHandler.CanRun = true;
        Heart.SetActive(true);
    }

    void ActivateObjectiveMechanic()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.FUTURISTIC_PANEL_OPEN);
        Objective.SetActive(true);
    }

    void DeactivateAllMechanic()
    {
        //heart rate: deactivate the ui + anxiety build up
        //disable noise range indicator
        //disable objective indicator
        anxietyHandler.CanRun = false;
        Heart.SetActive(false);
        Objective.SetActive(false);
    }

    void ProgressObjective(ObjectiveName name)
    {
        currentRoom.ProgressObjective(name);
    }

    void SwitchCurrentRoom(Room newRoom)
    {
        if (newRoom == null) throw new System.Exception("THE NEW ROOM IS NULL");

        currentRoom = newRoom;
    }
}
