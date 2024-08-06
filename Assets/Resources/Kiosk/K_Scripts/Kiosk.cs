using Dialog;
using PopUpAssistance;
using SoundRelated;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Kiosk : MonoBehaviour , IScriptLoadQueuer
{
    [SerializeField]
    Image progressUI;
    [SerializeField]
    GameObject progress_GO;
    // first and second audio speechSource is for SFX

    //[SerializeField]
    //AudioSource audioSource;
    //[SerializeField]
    //AudioSource speechSource;

    [SerializeField]
    Animator animator;

    /// <summary>
    /// 0. When the panel is touch when it has not been downloaded
    /// 1. when the panel have finish downloading
    /// 2. texting
    /// 3. image transition
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

    [SerializeField] Transform targetDestination;
    

    [Header("Popup")]
    [SerializeField] 
    PopUp popup;
    [SerializeField]
    GameObject mapIcon;

    SoundManager audioPlayer;
    AudioSource globalAudioSource = null;

    [Header("Kiosk Dialog")]
    [SerializeField] KioskLines kioskData;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] Image image;
    [SerializeField] GridLayoutGroup imageSizer; // using the cell size to increase the width and heigh of it.1
    [SerializeField, Range(1, 30)]
    float textSpeed = 20;
    int indexDialog = 0;
    int changePanel = Animator.StringToHash("ShowImage");
    int hidePanelHash = Animator.StringToHash("HidePanel");


    [SerializeField]
    bool hasHologramPanels;

    public Transform TargetDestination { get => targetDestination; }
    public bool ScanCompleted { get => scanCompleted; }

    [SerializeField]DialogueLines triggerLines;

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

            //StopSFX();
            //audioSource.PlayOneShot(audioClips[1]);
            //audioPlayer.StopPlayingContinuousAudio(SoundRelated.SFXClip.TEXT_TYPING);
            audioPlayer.RetrieveAudioSource(globalAudioSource);
            audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);
            EventSystem.level.TriggerEvent<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, ObjectiveName.KIOSK);
            animator.SetBool("Completed", true);       
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

    //called by xr simple interactor
    public void ScanStart()
    {
        animator.SetBool("Hand_Detected", true);
        authenticate = true;

        if (!scanCompleted)
            //audioSource.PlayOneShot(audioClips[0]);
            audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.KIOSK_AUTHETICATED,transform.position);
    }

    //called by xr simple interactor
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

    public void StartKioskDialog()
    {
        ChangeImage();
        StartTyping();
    }

    public void ChangeImagePanel()
    {
        ChangeImage();
        dialogText.text = "";
    }

    public void StartTyping()
    {
        StopAllCoroutines();
        StartCoroutine(WaitTimer(kioskData.Lines[indexDialog].duration));
    }

    void ChangeImage()
    {
        var line = kioskData.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredDimension;
    }

    private IEnumerator WaitTimer(float second)
    {
        var clip = kioskData.Lines[indexDialog].clip;
        AudioSource speechSource = null;

        if (clip)
        {
            MusicClip musicClip = new MusicClip(clip);  
            speechSource = audioPlayer.PlayMusicClip(musicClip,transform.position);
        }
        StartCoroutine(TypeNextSentence());
        yield return new WaitForSeconds(second);


        //audio source related
        //For error catch safety.
        if (globalAudioSource) audioPlayer.RetrieveAudioSource(globalAudioSource);
        if (speechSource) audioPlayer.RetrieveAudioSource(speechSource);

        indexDialog++;

        if (indexDialog >= kioskData.Lines.Length)
        {
            //dialog is complete
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE,transform.position);
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, triggerLines);
            animator.SetTrigger(hidePanelHash);
        }
        else
        {
            //audioSource.PlayOneShot(audioClips[3]);
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            animator.SetTrigger(changePanel);
        }
        //animator.SetTrigger()
    }

    private IEnumerator TypeNextSentence()
    {
        globalAudioSource = audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
        dialogText.text = "";
        string text = kioskData.Lines[indexDialog].Text;

        foreach (char c in text.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        audioPlayer.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = null;
    }

    public void Initialize()
    {
        progressUI.fillAmount = 0f;
        progress_GO.SetActive(true);
        audioPlayer = SoundManager.Instance;
        ScriptableObjectManager.AddIntoSOCollection(kioskData.OtherDialogue);
        triggerLines = (DialogueLines)ScriptableObjectManager.RetrieveRuntimeScriptableObject(kioskData.OtherDialogue);
    }
}
