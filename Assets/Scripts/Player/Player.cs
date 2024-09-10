using Assets.HelpfulScripts;
using Assets.Scripts.Player.Anxiety_Scripts;
using SoundRelated;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IScriptLoadQueuer
{
    PlayerAnxietyHandler anxietyHandler; //handles noise + light + breathing
    PlayerVFX vfx;
    PlayerObjectiveHandler objectiveHandler;

    //references to ui
    [SerializeField]
    GameObject HeartUI, 
        ObjectiveUI, 
        DialogueUI, 
        SkipHologramText, 
        MapUI;

    [Header("Breathing detection related")]
    //for breathing
    [SerializeField] 
    GameObject BreathDetection;

    [Header("controller related")]
    [SerializeField]
    OVRPlayerController playerController;
    [SerializeField]
    CharacterController characterController;

    [SerializeField]
    Transform playerTransform;

    //room / objective variables
    Room currentRoom;

    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    public bool IsWearingHeadphones;
    public bool isWearingSunglasses;

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.PLAYER);
    }

    private void Update()
    {
        anxietyHandler.CalculateAnxiety();
        if (GameData.IsInTutorial)
        {
            anxietyHandler._anxietyLevel = 0;
        }
    }

    #region Initialization
    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<DialogEvents> em_d = EventSystem.dialog;
    EventManager<LevelEvents> em_l = EventSystem.level;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    public Transform LeftHand { get => leftHand; set => leftHand = value; }
    public Transform RightHand { get => rightHand; set => rightHand = value; }

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
        em_d.AddListener(DialogEvents.ACTIVATE_BREATHING, ActivateBreathingMechanic);
        //level events
        em_l.AddListener<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, ProgressObjective);
        em_l.AddListener<Room>(LevelEvents.ENTER_NEW_ROOM, SwitchCurrentRoom);        
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
        HeartUI.SetActive(true);
    }

    void ActivateBreathingMechanic()
    {
        BreathDetection.SetActive(true);
    }

    void DeactivateAllMechanic()
    {

        //heart rate: deactivate the ui + anxiety build up
        //disable noise range indicator
        //disable objective indicator
        anxietyHandler.CanRun = false;
        HeartUI.SetActive(false);
        BreathDetection.SetActive(false);
        //ObjectiveUI.SetShow(false);
    }

    void CloseAllUI()
    {
        HeartUI.SetActive(false);
        ObjectiveUI.SetActive(false);

        MapUI.SetActive(false);
    }

    void OpenUI()
    {
        HeartUI.SetActive(true);
        ObjectiveUI.SetActive(true);
        MapUI.SetActive(true);
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
        //make sure to stop any hologram that is being played in the background
        em_l.TriggerEvent(LevelEvents.INTERRUPT_HOLOGRAM);
        DisableMovement();
        yield return new WaitUntil(() => vfx.isFaded);
        em_p.TriggerEvent(PlayerEvents.DEATH_SCREEN_FADED); 
        yield return new WaitForSeconds(1f);
        EnableMovement();
        vfx.BeginUnfadeScreen();
    }

    IEnumerator T_Respawn()
    {
        vfx.BeginFadeScreen();
        DisableMovement();

        yield return new WaitUntil(() => vfx.isFaded);
        em_p.TriggerEvent(PlayerEvents.RES_SCREEN_FADED);
        vfx.BeginUnfadeScreen();
        yield return new WaitUntil(() => !vfx.isFaded);
        EnableMovement();

        OpenUI();
    }

    float initialAccleration;

    void EnableMovement()
    {
        //playerController.Teleported = false;
        //characterController.enabled = false;
        //playerController.Acceleration = initialAccleration;
        playerController.enabled = true;
    }

    void DisableMovement()
    {
        playerController.Stop();
        playerController.enabled = false;
        //playerController.Teleported = true;
        //characterController.enabled = false;
        //if (playerController.Acceleration > 0)
        //{
        //    //will always make sure the keep a position value acceleration
        //    initialAccleration = playerController.Acceleration;
        //}
        ////make sure the player cant move anymore
        //playerController.Stop();
        //playerController.Acceleration = 0;
    }


    #endregion

    #region hologram
    //legacy code for pausing the hologram
    //void OnHologramPlaying()
    //{
    //    //stop controller from moving
    //    movementController.enabled = false;
    //    cameraController.enabled = false;
    //    //then stop the anxiety handler from running
    //    anxietyHandler.CanRun = false;
    //}

    //void OnHologramStop()
    //{
    //    //stop controller from moving
    //    movementController.enabled = true;
    //    cameraController.enabled = true;
    //    //then stop the anxiety handler from running
    //    anxietyHandler.CanRun = true;
    //}

    //void OnHologramPause()
    //{
    //    SkipHologramText.SetActive(true);
    //}

    //void OnHologramUnpause()
    //{
    //    SkipHologramText.SetActive(false);
    //}

    //void MovePlayerToKioskPosition(Transform targetTransform)
    //{
    //    playerTransform.position = targetTransform.position;
    //    playerTransform.rotation = targetTransform.rotation;

    //    SkipHologramText.SetActive(true);
    //    EventSystem.player.AddListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, RemoveInterruptHologramButton);
    //    controllerManager.AddOnPressEvent(Controls.BButton, TriggerInterruptHologram);
    //}

    //void TriggerInterruptHologram()
    //{
    //    print("is triggered??");
    //    EventSystem.player.TriggerEvent(PlayerEvents.INTERRUPT_HOLOGRAM);
    //}

    //void RemoveInterruptHologramButton()
    //{
    //    controllerManager.RemoveHoldEvent(Controls.BButton, TriggerInterruptHologram);
    //    EventSystem.player.RemoveListener(PlayerEvents.FINISH_PLAYING_HOLOGRAM, RemoveInterruptHologramButton);

    //}

    #endregion
}
