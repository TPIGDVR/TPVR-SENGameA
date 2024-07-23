using Dialog;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class HandsScanning : MonoBehaviour
{

    [SerializeField]
    Image progressUI;
    [SerializeField]
    GameObject progress_GO;
    // first and second audio source is for SFX
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource audioSource2;
    [SerializeField]
    AudioSource speechSource;

    [SerializeField]
    AudioClip[] audioClips;
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

    [SerializeField]
    float speedMultiplier;
    [SerializeField]float progress = 0;
    [SerializeField]
    Color lowColor, medColor, hiColor;

    [SerializeField]
    bool scanCompleted;
    bool scanning = false;
    bool authenticate = false;
    bool hasPlayedAuthenticationSFX = false;
    EventManager<LevelEvents> em_l = EventSystem.level;

    [Header("Kiosk Dialog")]
    [SerializeField] KisokLines kioskData;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] Image image;
    [SerializeField] GridLayoutGroup imageSizer; // using the cell size to increase the width and heigh of it.1
    public bool ScanCompleted {  get { return scanCompleted; } }
    [SerializeField, Range(1, 30)]
    float textSpeed = 20;

    private void Start()
    {
        progressUI.fillAmount = 0f;
        progress_GO.SetActive(true);
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
        else if(!scanning &&  !scanCompleted)
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
            //stop audiosource 2 from playing
            audioSource2.Stop();
            audioSource2.loop = false;

            scanCompleted = true;
            audioSource.PlayOneShot(audioClips[1]);
            em_l.TriggerEvent(LevelEvents.KIOSK_CLEARED);
            animator.SetBool("Completed",true);
        }
    }

    void UpdateProgressUI()
    {
        progressUI.fillAmount = progress;


        //slowly changes color as it progresses
        if(progress < 0.5) 
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
        animator.SetBool("Hand_Detected",true);
        authenticate = true;

        if(!scanCompleted)
            audioSource.PlayOneShot(audioClips[0]);

        if(!scanCompleted && !hasPlayedAuthenticationSFX)
        {
            hasPlayedAuthenticationSFX = true;
            audioSource2.loop = true;
            audioSource2.clip = audioClips[5];
            audioSource2.Play();
        }
    }

    //called by xr simple interactor
    public void ScanStop()
    {
        scanning = false;
        authenticate = false;
        if (!scanCompleted && hasPlayedAuthenticationSFX)
        {
            hasPlayedAuthenticationSFX = false;
            audioSource2.Stop();
            audioSource2.loop = false;
        }
    }

    //called as a animation event for DigitalCircle_Authenticated
    void StartScan()
    {
        if(authenticate)
            scanning = true;
    }

    int indexDialog = 0;
    int canShowImageHash = Animator.StringToHash("CanShowImage");
    int changeTextHash = Animator.StringToHash("ShowText");
    int hidePanelHash = Animator.StringToHash("HidePanel");
    public void DecidePanel()
    {
        var line = kioskData.Lines[indexDialog];
        if (line.image)
        {
            image.sprite = line.image;
            imageSizer.cellSize = line.preferredDimension;
            animator.SetBool(canShowImageHash, true);
            audioSource2.PlayOneShot(audioClips[3]);
        }

        StartCoroutine(WaitTimer(line.duration));
    }

    public void ChangeNewText()
    {
        StopAllCoroutines();
        StartCoroutine(TypeNextSentence());
    }

    public void StartDialog()
    {
        var line = kioskData.Lines[indexDialog];
        StartCoroutine(TypeNextSentence());
        
        if (line.image)
        {
            audioSource2.PlayOneShot(audioClips[3]);
            image.sprite = line.image;
            imageSizer.cellSize = line.preferredDimension;
            animator.SetBool(canShowImageHash, true);
        }

        StartCoroutine(WaitTimer(line.duration));
    }

    private IEnumerator WaitTimer(float second)
    {
        yield return new WaitForSeconds(second);
        if (animator.GetBool(canShowImageHash))
        {
            audioSource2.PlayOneShot(audioClips[4]);
            animator.SetBool(canShowImageHash, false);
        }

        indexDialog++;

        if(indexDialog >= kioskData.Lines.Length)
        {
            audioSource.PlayOneShot(audioClips[4]);
            animator.SetTrigger(hidePanelHash);
        }
        else
        {
            animator.SetTrigger(changeTextHash);
        }
        //animator.SetTrigger()
    }

    private IEnumerator TypeNextSentence()
    {
        dialogText.text = "";
        string text = kioskData.Lines[indexDialog].Text;

        //start playing audio clip to play the typing sfx
        audioSource.clip = audioClips[2];
        audioSource.loop = true;
        audioSource.Play();

        //play the audio clip forspeech
        speechSource.PlayOneShot(kioskData.Lines[indexDialog].clip);

        foreach (char c in text.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        audioSource.loop = false;
        audioSource.Stop();
    }


}


