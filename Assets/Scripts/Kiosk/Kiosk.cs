using Dialog;
using PopUpAssistance;
using SoundRelated;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Kiosk : MonoBehaviour , IScriptLoadQueuer
{
    [SerializeField]
    Image progressUI;
    [SerializeField]
    GameObject progress_GO;

    [SerializeField]
    Animator animator;

    /// <summary>
    /// 0. When the panel is touch when it has not been downloaded
    /// 1. when the panel have finish downloading
    /// 2. texting
    /// 3. imageComponent transition
    /// 4. when the panel is close
    /// 5. downloading
    /// </summary>

    [Header("Scanning")]
    [SerializeField]
    float speedMultiplier;
    [SerializeField] float progress = 0;
    [SerializeField]
    Color lowColor, medColor, hiColor;
    [SerializeField]
    bool scanCompleted;
    bool scanning = false;
    bool authenticate = false;

    [SerializeField] Transform automatonTargetDestination;
    [SerializeField] Transform hologramTargetDestination;
    [Header("Popup")]
    [SerializeField] 
    PopUp popup;
    [SerializeField]
    GameObject mapIcon;

    SoundManager audioPlayer;
    AudioSource globalAudioSource = null;

    int changePanel = Animator.StringToHash("ShowImage");
    int hidePanelHash = Animator.StringToHash("HidePanel");

    [SerializeField]
    Hologram3DData hologram3DData;
    [SerializeField]
    HologramSlideShowData hologramslideShowData;
    [SerializeField]
    BaseHologram hologram;

    [SerializeField]
    bool hasHologramPanels;

    public bool ScanCompleted { get => scanCompleted; }
    public Transform AutomatonTargetDestination { get => automatonTargetDestination; }

    //DialogueLines triggerLines;

    public void Initialize()
    {
        progressUI.fillAmount = 0f;
        progress_GO.SetActive(true);
        audioPlayer = SoundManager.Instance;
        //ScriptableObjectManager.AddIntoSOCollection(kioskData.OtherDialogue);
        //triggerLines = (DialogueLines)ScriptableObjectManager.RetrieveRuntimeScriptableObject(kioskData.OtherDialogue);

        if (hologram3DData)
        {
            var hologramGO = Instantiate(GameData.hologram3D, hologramTargetDestination);
            hologramGO.transform.localPosition = Vector3.zero;
            //get the component of the scriptable object from the 
            var hologram3d = hologramGO.GetComponent<Hologram_3D>();
            hologram3d.InitHologram(hologram3DData);
            hologram = hologram3d;
        }
        else if (hologramslideShowData)
        {
            var hologramGO = Instantiate(GameData.hologramSlideShow, hologramTargetDestination);
            hologramGO.transform.localPosition = Vector3.zero;
            //get the component of the scriptable object from the 
            var hologramSlideShow = hologramGO.GetComponentInChildren<Hologram_Slideshow>();
            hologramSlideShow.InitHologram(hologramslideShowData);
            hologram = hologramSlideShow;
        }

    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL);
    }

    private void Update()
    {
        if (authenticate && progress > 0)
        {
            scanning = true;
        }

        if (scanning && !scanCompleted)
        {
            progress += Time.deltaTime / 100 * speedMultiplier;
        }
        else if (!scanning && !scanCompleted)
        {
            //make the progress decay slightly slower than the gain speed
            progress -= Time.deltaTime / 200 * speedMultiplier;

            if (progress <= 0 && !authenticate) //check if there is progress and hand is still on the kiosk
            {
                animator.SetBool("Hand_Detected", false);
                progress_GO.SetActive(false);
            }
        }
        progress = Mathf.Clamp01(progress); //make sure we maintain the 0-1 values

        UpdateProgressUI();

        if (progress >= 1 && !scanCompleted)
        {
            popup.CanPopUp = false;
            mapIcon.gameObject.SetActive(false);
            scanCompleted = true;
            
            audioPlayer.RetrieveAudioSource(globalAudioSource);
            audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);
            if (hologram)
            {
                hologram.PlayAnimation();
                //StartCoroutine(TeleportPlayer());
            }

            animator.SetBool("Completed", true);
            EventSystem.level.TriggerEvent<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, ObjectiveName.KIOSK);

            //teleport player to location once the 
        }
    }

    void UpdateProgressUI()
    {
        progressUI.fillAmount = progress;

        //slowly changes color as it progresses
        if (progress < 0.5)
        {
            progressUI.color = Color.Lerp(lowColor, medColor, progress * 0.5f);
        }
        else
        {
            progressUI.color = Color.Lerp(medColor, hiColor, progress * 0.5f);
        }
    }

#region Scanning

    //called by ScanTrigger
    public void ScanStart()
    {
        animator.SetBool("Hand_Detected", true);
        authenticate = true;

        if (!scanCompleted)
            //audioSource.PlayOneShot(audioClips[0]);
            audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.KIOSK_AUTHETICATED,transform.position);
    }

    //called by ScanTrigger
    public void ScanStop()
    {
        scanning = false;
        authenticate = false;

        if (!scanCompleted)
        {
            if (globalAudioSource)
            {
                audioPlayer.RetrieveAudioSource(globalAudioSource);
            }
            //StopSFX();
            //audioPlayer.StopPlayingContinuousAudio(SoundRelated.SFXClip.TEXT_TYPING);
        }
    }

    //called as a animation event for DigitalCircle_Authenticated
    void StartScan()
    {
        if (authenticate)
        {
            scanning = true;
            //if there is a audio speechSource than keep it
            if (globalAudioSource) audioPlayer.RetrieveAudioSource(globalAudioSource);
            globalAudioSource = audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING , transform.position);
        }
    }
#endregion
    public void SetHide()
    {
        popup.CanPopUp = false;
        mapIcon.gameObject.SetActive(false);
        enabled = false;
    }

    public void SetShow()
    {
        enabled = true;
        if (!scanCompleted)
        {
            popup.CanPopUp = true;
            mapIcon.gameObject.SetActive(true);
        }
    }

    //public IEnumerator TeleportPlayer()
    //{
    //    var player = GameData.player;
    //    //wait for the cinemachine to have an active blend
    //    while (player.MainCinemachineBrain.ActiveBlend == null) yield return null;
    //    var blend = player.MainCinemachineBrain.ActiveBlend;

    //    while(!blend.IsComplete) yield return null;
    //    //player.MovePlayerToTransform(playerFinalDestination);
    //    EventSystem.player.TriggerEvent<Transform>(PlayerEvents.MOVE_PLAYER_TO_KIOKPOSITION, playerFinalDestination);

    //}

}
