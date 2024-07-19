using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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
    [SerializeField]
    AudioSource completeSound;
    [SerializeField]
    Animator loadAnimator;

    [SerializeField]
    float minSpeedMultiplier, maxSpeedMultiplier;
    float speedMultiplier;
    [SerializeField]float progress = 0;
    [SerializeField]
    Color lowColor, medColor, hiColor;

    [SerializeField]
    bool scanCompleted;
    bool scanning = false;
    bool authenticate = false;
    EventManager<LevelEvents> em_l = EventSystem.level;

    public bool ScanCompleted {  get { return scanCompleted; } }

    private void Start()
    {
        progressUI.fillAmount = 0f;
        speedMultiplier = Random.Range(minSpeedMultiplier,maxSpeedMultiplier);
        progress_GO.SetActive(true);
    }


    private void Update()
    {
        if (scanning && !scanCompleted)
        {
            progress += Time.fixedDeltaTime / 100 * speedMultiplier;
        }
        else if(!scanning &&  !scanCompleted)
        {
            progress -= Time.fixedDeltaTime / 200 * speedMultiplier;

            if (progress <= 0 && !authenticate)
            {
                loadAnimator.SetBool("Hand_Detected", false);
                progress_GO.SetActive(false);
            }
        }
        progress = Mathf.Clamp01(progress);

        UpdateProgressUI();

        if (progress >= 1 && !scanCompleted)
        {
            scanCompleted = true;
            completeSound.Play();
            em_l.TriggerEvent(LevelEvents.KIOSK_CLEARED);

        }
    }

    void UpdateProgressUI()
    {
        progressUI.fillAmount = progress;

        if(progress < 0.5) 
        {
            progressUI.color = Color.Lerp(lowColor, medColor, progress * 0.5f);
        }
        else
        {
            progressUI.color = Color.Lerp(medColor, hiColor, progress * 0.5f);
        }
    }

    public void ScanStart()
    {
        loadAnimator.SetBool("Hand_Detected",true);
        authenticate = true;
    }


    public void ScanStop()
    {
        scanning = false;
        authenticate = false;
    }

    void StartScan()
    {
        scanning = true;
    }
}


