using Assets.Scripts.Player.Anxiety_Scripts;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IScriptLoadQueuer
{
    bool isWearingSunglasses;

    PlayerAnxietyHandler anxietyHandler; //handles noise + light + breathing
    PlayerVFX vfx;
    PlayerObjectiveHandler objectiveHandler;

    //references to ui
    [SerializeField]
    GameObject Heart,Objective,Dialogue;

    [SerializeField]
    Transform playerTransform;

    //room / objective variables
    Room currentRoom;

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.PLAYER);
    }

    private void Update()
    {
        anxietyHandler.CalculateAnxiety();
    }

    #region Initialization
    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;
    EventManager<DialogEvents> em_d = EventSystem.dialog;
    EventManager<LevelEvents> em_l = EventSystem.level;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    public void Initialize()
    {
        EventSubscribing();
        GetReferenceToComponents();
        anxietyHandler.InitializePlayerAnxiety();
        objectiveHandler.InitializeObjectiveHandler();
        GameData.player = this;
    }

    void EventSubscribing()
    {
        //tutorial events
        em_t.AddListener(TutorialEvents.INIT_TUTORIAL, DeactivateAllMechanic);
        em_t.AddListener(TutorialEvents.TUTORIAL_DEATH, TutorialDeath);
        em_t.AddListener(TutorialEvents.RESTART, TutorialRespawn);

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
        objectiveHandler = GetComponent<PlayerObjectiveHandler>();
        vfx = GetComponent<PlayerVFX>();
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

    void ProgressObjective(ObjectiveName name)
    {
        currentRoom.ProgressObjective(name);
    }

    void SwitchCurrentRoom(Room newRoom)
    {
        if (newRoom == null) throw new System.Exception("THE NEW ROOM IS NULL");

        if (currentRoom != null)
            currentRoom.OnExit();

        currentRoom = newRoom;
        currentRoom.OnEnter();
    }

    #region TUTORIAL METHODS
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
        //Objective.SetActive(false);
    }

    void CloseAllUI()
    {
        Heart.SetActive(false);
        Objective.SetActive(false);
        Dialogue.SetActive(false);
    }

    void OpenUI()
    {
        Heart.SetActive(true);
        Objective.SetActive(true);
    }

    void TutorialDeath()
    {
        StartCoroutine(T_Death());
    }

    void TutorialRespawn()
    {
        StartCoroutine(T_Respawn());
        anxietyHandler.isDead = false;
    }

    IEnumerator T_Death()
    {
        vfx.BeginFadeScreen();
        CloseAllUI();
        yield return new WaitUntil(() => vfx.isFaded);
        em_t.TriggerEvent(TutorialEvents.DEATH_SCREEN_FADED);
        yield return new WaitForSeconds(1f);
        vfx.BeginUnfadeScreen();
    }

    IEnumerator T_Respawn()
    {
        vfx.BeginFadeScreen();
        yield return new WaitUntil(() => vfx.isFaded);
        em_t.TriggerEvent(TutorialEvents.RES_SCREEN_FADED);
        vfx.BeginUnfadeScreen();
        yield return new WaitUntil(() => !vfx.isFaded);
        OpenUI();
    }
    #endregion
}
