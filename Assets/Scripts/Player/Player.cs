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

    [ContextMenu("Kill Player")]
    void Death()
    {
        StartCoroutine(T_Death());
    }

    void Respawn()
    {
        print("Start respawning");
        StartCoroutine(T_Respawn());
        anxietyHandler.isDead = false;
    }

    IEnumerator T_Death()
    {
        
        CloseAllUI();
        //make sure to stop any hologram that is being played in the background
        em_l.TriggerEvent(LevelEvents.INTERRUPT_HOLOGRAM);
        StartCoroutine(DisableMovementFaint());
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
        print("respawnning");
        yield return new WaitUntil(() => vfx.isFaded);
        em_p.TriggerEvent(PlayerEvents.RES_SCREEN_FADED);
        print("begin unfading");
        vfx.BeginUnfadeScreen();
        yield return new WaitUntil(() => !vfx.isFaded);
        EnableMovement();
        print("Finish");

        OpenUI();
    }

    float initialAccleration;

    void EnableMovement()
    {
        playerController.enabled = true;
        playerController.EnableLinearMovement = true;
    }

    IEnumerator DisableMovementFaint()
    {
        playerController.EnableLinearMovement = false;

        yield return StartCoroutine(vfx.FaintScreen());
        

        playerController.Stop();
        playerController.enabled = false;
        
    }

    void DisableMovement()
    {
        playerController.Stop();
        playerController.enabled = false;
    }


    #endregion
}
