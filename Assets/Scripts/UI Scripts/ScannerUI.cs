using System.Collections;
using SoundRelated;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScannerUI : MonoBehaviour , IScriptLoadQueuer
{
    [SerializeField]
    Collider triggerBox;
    [SerializeField]
    GameObject canvas;

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

    //due to some weird bug in the editor
    bool pauseUpdateLoop = false;

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL);
    }

    public void Initialize()
    {
        scanCompleted = false;
        audioPlayer = SoundManager.Instance;
        UpdateProgressUI();
    }

    void Update()
    {
        if (scanCompleted || (progress <= 0 && !scanning) || pauseUpdateLoop) return;

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
        print("Calling Complete scanner");
        scanCompleted = true;
        pauseUpdateLoop = true;
        onCompleteScan?.Invoke();
        audioPlayer.RetrieveAudioSource(globalAudioSource);
        audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);
    }

     public void ResetScanner()
    {
        print("reset scanner");
        progress = 0;
        scanCompleted = false;
        scanning = false;
        UpdateProgressUI();
        StartCoroutine(WaitToResetScanner());
    }

    IEnumerator WaitToResetScanner()
    {
        yield return null;
        pauseUpdateLoop = false;
    }

    #region Simple Interactor
    public void StartScan()
    {
        if (scanCompleted) return;
        scanning = true;
        if (globalAudioSource) audioPlayer.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING, transform.position);
    }

    public void StopScan()
    {
        if (scanCompleted) return;
        scanning = false;
        if (globalAudioSource)
        {
            audioPlayer.RetrieveAudioSource(globalAudioSource);
        }
    }
    #endregion

    public void SetActive(bool active)
    {
        if(active)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
        triggerBox.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        StartScan();
    }

    private void OnTriggerExit(Collider other)
    {
        StopScan();
    }
}
