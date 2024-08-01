using Caress.Examples;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScannerUI : MonoBehaviour
{
    [SerializeField]
    Image progressBar;

    [SerializeField]
    float speedMultiplier;
    [SerializeField]
    Color lowColor, medColor, hiColor;

    float progress = 0;
    bool scanning = false;
    bool scanCompleted;

    public UnityEvent onCompleteScan;
    SoundManager audioPlayer;
    AudioSource globalAudioSource = null;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        scanCompleted = false;
        UpdateProgressUI();
    }

    void Update()
    {
        if (scanCompleted || (progress <= 0 && !scanning)) return;

        if (scanning && !scanCompleted)
        {
            progress += Time.fixedDeltaTime / 100 * speedMultiplier;
        }
        else if (!scanning && !scanCompleted)
        {
            //make the progress decay slightly slower than the gain speed
            progress -= Time.fixedDeltaTime / 200 * speedMultiplier;
        }

        progress = Mathf.Clamp01(progress); //make sure we maintain the 0-1 values

        UpdateProgressUI();

        //complete
        if (progress >= 1 && !scanCompleted)
        {
            Complete();
        }
    }

    void UpdateProgressUI()
    {
        progressBar.fillAmount = progress;

        if (progress < 0.5)
        {
            progressBar.color = Color.Lerp(lowColor, medColor, progress * 0.5f);
        }
        else
        {
            progressBar.color = Color.Lerp(medColor, hiColor, progress * 0.5f);
        }
    }

    void Complete()
    {
        scanCompleted = true;
        audioPlayer.RetrieveAudioSource(globalAudioSource);
        audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);
    }

    #region Simple Interactor
    public void StartScan()
    {
        scanning = true;
        if (globalAudioSource) audioPlayer.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING, transform.position);
    }

    public void StopScan()
    {
        scanning = false;
        if (globalAudioSource)
        {
            audioPlayer.RetrieveAudioSource(globalAudioSource);
        }
    }
    #endregion
}
