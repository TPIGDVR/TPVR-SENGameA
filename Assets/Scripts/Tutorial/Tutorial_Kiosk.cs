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

    [SerializeField]
    Transform targetDestination;

    [Header("Event")]
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;
    [SerializeField]
    TutorialEvents t_event;

    [Header("Popup")]
    [SerializeField] 
    PopUp popup;

    SoundManager audioPlayer;

    public Transform TargetDestination { get => targetDestination;}

    private void OnEnable()
    {
        em_t.AddListener(TutorialEvents.DETERMINE_LAST_KIOSK, CheckIfFinalKiosk);
    }

    private void OnDisable()
    {
        em_t.RemoveListener(TutorialEvents.DETERMINE_LAST_KIOSK, CheckIfFinalKiosk);
    }

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

    //void StopSFX()
    //{
    //    audioSource.Stop();
    //    audioSource.loop = false;
    //}

    void CheckIfFinalKiosk()
    {
        if(!scanCompleted)
        {//if kioskdownloaded is the three one and the kiosk is 
            //not scan, it is the last kiosk
            em_t.TriggerEvent<Tutorial_Kiosk>(TutorialEvents.LAST_KIOSK, this);
        }
    }

}
