using Assets.HelpfulScripts;
using Assets.Scripts.Player.Anxiety_Scripts;
using Cinemachine;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour, IScriptLoadQueuer
{
    bool isWearingSunglasses;

    PlayerAnxietyHandler anxietyHandler; //handles noise + light + breathing
    PlayerVFX vfx;
    PlayerObjectiveHandler objectiveHandler;

    //references to ui
    [SerializeField]
    GameObject Heart,Objective,Dialogue,SkipHologramText;

    [SerializeField]
    Transform playerTransform;

    [Header("Controller related")]
    [SerializeField] ContinuousMoveProviderBase movementController;
    [SerializeField] ContinuousTurnProviderBase cameraController;
    [SerializeField] ControllerManager controllerManager;

    [Header("Camera related")]
    [SerializeField] CinemachineBrain mainCinemachineBrain;

    //room / objective variables
    Room currentRoom;

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.PLAYER);
    }

    private void Update()
    {
        if (!GameData.IsInTutorial)
        {
            anxietyHandler.CalculateAnxiety();
        }
    }

    #region Initialization
    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<DialogEvents> em_d = EventSystem.dialog;
    EventManager<LevelEvents> em_l = EventSystem.level;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    public CinemachineBrain MainCinemachineBrain { get => mainCinemachineBrain; }

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
        em_l.AddListener(LevelEvents.INIT_TUTORIAL, DeactivateAllMechanic);
        em_p.AddListener(PlayerEvents.DEATH, Death);
        em_p.AddListener(PlayerEvents.RESTART, Respawn);

        //dialogue events
        em_d.AddListener(DialogEvents.ACTIVATE_HEARTRATE, ActivateHeartRateMechanic);

        //level events
        em_l.AddListener<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, ProgressObjective);
        em_l.AddListener<Room>(LevelEvents.ENTER_NEW_ROOM, SwitchCurrentRoom);

        //for Hologram
        //em_p.AddListener(PlayerEvents.START_PLAYING_HOLOGRAM, OnHologramPlaying);
        //em_p.AddListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, OnHologramStop);
        //em_p.AddListener<Transform>(PlayerEvents.MOVE_PLAYER_TO_KIOKPOSITION, MovePlayerToKioskPosition);
        //em_p.AddListener(PlayerEvents.PAUSE_HOLOGRAM, OnHologramPause);
        //em_p.AddListener(PlayerEvents.UNPAUSE_HOLOGRAM, OnHologramUnpause);
    }

    void EventUnsubscribing()
    {
        em_l.RemoveListener(LevelEvents.INIT_TUTORIAL, DeactivateAllMechanic);
        em_d.RemoveListener(DialogEvents.ACTIVATE_HEARTRATE, ActivateHeartRateMechanic);
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

    void DeactivateAllMechanic()
    {
        //heart rate: deactivate the ui + anxiety build up
        //disable noise range indicator
        //disable objective indicator
        anxietyHandler.CanRun = false;
        Heart.SetActive(false);
        //Objective.SetShow(false);
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

    void Death()
    {
        StartCoroutine(T_Death());
    }

    void Respawn()
    {
        StartCoroutine(T_Respawn());
        anxietyHandler.isDead = false;
    }

    IEnumerator T_Death()
    {
        vfx.BeginFadeScreen();
        CloseAllUI();
        yield return new WaitUntil(() => vfx.isFaded);
        em_p.TriggerEvent(PlayerEvents.DEATH_SCREEN_FADED);
        yield return new WaitForSeconds(1f);
        vfx.BeginUnfadeScreen();
    }

    IEnumerator T_Respawn()
    {
        vfx.BeginFadeScreen();
        yield return new WaitUntil(() => vfx.isFaded);
        em_p.TriggerEvent(PlayerEvents.RES_SCREEN_FADED);
        vfx.BeginUnfadeScreen();
        yield return new WaitUntil(() => !vfx.isFaded);
        OpenUI();
    }
    #endregion

    #region hologram
    void OnHologramPlaying()
    {
        //stop controller from moving
        movementController.enabled = false;
        cameraController.enabled = false;
        //then stop the anxiety handler from running
        anxietyHandler.CanRun = false;
    }

    void OnHologramStop()
    {
        //stop controller from moving
        movementController.enabled = true;
        cameraController.enabled = true;
        //then stop the anxiety handler from running
        anxietyHandler.CanRun = true;
    }

    void OnHologramPause()
    {
        SkipHologramText.SetActive(true);
    }

    void OnHologramUnpause()
    {
        SkipHologramText.SetActive(false);
    }

    void MovePlayerToKioskPosition(Transform targetTransform)
    {
        playerTransform.position = targetTransform.position;
        playerTransform.rotation = targetTransform.rotation;

        SkipHologramText.SetActive(true);
        EventSystem.player.AddListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, RemoveInterruptHologramButton);
        controllerManager.AddOnPressEvent(Controls.BButton, TriggerInterruptHologram);
    }

    void TriggerInterruptHologram()
    {
        print("is triggered??");
        EventSystem.player.TriggerEvent(PlayerEvents.INTERRUPT_HOLOGRAM);
    }

    void RemoveInterruptHologramButton()
    {
        controllerManager.RemoveHoldEvent(Controls.BButton, TriggerInterruptHologram);
        EventSystem.player.RemoveListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, RemoveInterruptHologramButton);

    }

    #endregion
}
