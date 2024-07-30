using Caress.Examples;
using Dialog;
using PopUpAssistance;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Kiosk : MonoBehaviour
{
    [SerializeField]
    Image progressUI;
    [SerializeField]
    GameObject progress_GO;
    // first and second audio source is for SFX

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

    [Header("Event")]
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;
    [SerializeField]
    TutorialEvents t_event;

    [Header("Popup")]
    [SerializeField] 
    PopUp popup;

    SoundManager audioPlayer;

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

    private void Start()
    {
        progressUI.fillAmount = 0f;
        progress_GO.SetActive(true);
        audioPlayer = SoundManager.Instance;
    }

    private void Update()
    {
        if (authenticate && progress > 0)
        {
            scanning = true;
        }

        if (scanning && !scanCompleted)
        {
            progress += Time.fixedDeltaTime / 100 * speedMultiplier;
        }
        else if (!scanning && !scanCompleted)
        {
            //make the progress decay slightly slower than the gain speed
            progress -= Time.fixedDeltaTime / 200 * speedMultiplier;

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
            scanCompleted = true;

            //StopSFX();
            //audioSource.PlayOneShot(audioClips[1]);
            audioPlayer.StopPlayingContinuousAudio(SoundRelated.SFXClip.TEXT_TYPING);
            audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);


            em_t.TriggerEvent(t_event);
            if(TutorialLevelScript.kioskDownload == 1)
            {
                em_t.TriggerEvent<Transform>(TutorialEvents.FIRST_KIOSK, targetDestination);
            }
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
            //StopSFX();
            audioPlayer.StopPlayingContinuousAudio(SoundRelated.SFXClip.TEXT_TYPING);
        }
    }

    //called as a animation event for DigitalCircle_Authenticated
    void StartScan()
    {
        if (authenticate)
        {
            scanning = true;
            //audioSource.loop = true;
            //audioSource.clip = audioClips[5];
            //audioSource.Play();
            audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING , transform.position);
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
        StartCoroutine(TypeNextSentence());
        yield return new WaitForSeconds(second);

        indexDialog++;

        if (indexDialog >= kioskData.Lines.Length)
        {
            //audioSource.PlayOneShot(audioClips[4]);
            animator.SetTrigger(hidePanelHash);
        }
        else
        {
            //audioSource.PlayOneShot(audioClips[3]);
            animator.SetTrigger(changePanel);
        }
        //animator.SetTrigger()
    }

    private IEnumerator TypeNextSentence()
    {
        dialogText.text = "";
        string text = kioskData.Lines[indexDialog].Text;

        //start playing audio clip to play the typing sfx
        //audioSource.clip = audioClips[2];
        //audioSource.loop = true;
        //audioSource.Play();

        //play the audio clip forspeech
        //speechSource.PlayOneShot(kioskData.Lines[indexDialog].clip);

        foreach (char c in text.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        //audioSource.loop = false;
        //audioSource.Stop();
    }

}
