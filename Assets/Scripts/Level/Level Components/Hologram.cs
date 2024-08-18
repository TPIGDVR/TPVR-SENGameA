using Dialog;
using System.Collections;
using UnityEngine;
using TMPro;
using SoundRelated;
using Unity.VisualScripting;

public abstract class BaseHologram : MonoBehaviour
{
    public abstract void PlayAnimation();
}

public abstract class Hologram<DataType> : BaseHologram where DataType : HologramData 
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    float stoppingRadius = 10f;

    [Header("Subtitles")]
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float textSpeed = 20f;
    protected int indexDialog = 0;
    [SerializeField]
    protected bool use3DAudio;

    //protected SoundManager soundManager;
    [SerializeField] AudioSource globalAudioSource;
    [SerializeField] AudioSource speechSource;
    protected DialogueLines dialogLine;

    [SerializeField] protected DataType _Data;
    protected bool isRunning = false;

    protected virtual void Start()
    {
        //NOOP
    }

    //how a typical hologram would play can change this to abstract if
    //all hologram needs to operate differently
    public override void PlayAnimation()
    {
        gameObject.SetActive(true);
        isRunning = true;
        EventSystem.player.TriggerEvent(PlayerEvents.START_PLAYING_HOLOGRAM);
        EventSystem.player.TriggerEvent(PlayerEvents.INTERRUPT_HOLOGRAM);
        EventSystem.player.AddListener(PlayerEvents.INTERRUPT_HOLOGRAM, InteruptHologram);

    }

    /// <summary>
    /// setting the related data to the hologram
    /// </summary>
    /// <param name="data">The data that is set for the hologram</param>
    public virtual void InitHologram(DataType data)
    {
        ScriptableObjectManager.AddIntoSOCollection(data);
        _Data = (DataType) ScriptableObjectManager.RetrieveRuntimeScriptableObject(data);
        if (_Data)
        {
            if (_Data.DialogAfterComplete)
            {
                ScriptableObjectManager.AddIntoSOCollection(_Data.DialogAfterComplete);
                dialogLine = (DialogueLines)ScriptableObjectManager.RetrieveRuntimeScriptableObject(_Data.DialogAfterComplete);
            }
        }
        else
        {
            throw new System.Exception("Cant convert data into slideshow data");
        }
    }

    protected void RunPanel()
    {
        StartCoroutine(RunHologram());
    }

    private IEnumerator RunHologram()
    {
        yield return PrintKioskLines(_Data.dialogLine[indexDialog]);
        OnCompleteLine();
        DecideState();
    }

    #region on complete function
    /// <summary>
    /// Once a line has been completed, this function will call.
    /// Override it t extend the functionality
    /// </summary>
    protected virtual void OnCompleteLine()
    {
        //NOOP
    }
    /// <summary>
    /// Deciding which state the Hologram would be doing.
    /// </summary>
    private void DecideState()
    {
        if(indexDialog >= _Data.dialogLine.Length)
        {
            EndHologram();   
        }
        else
        {
            NextHologram();
        }
    }

    /// <summary>
    /// called if the hologram is reach the end of the dialog
    /// </summary>
    protected virtual void EndHologram()
    {
        if (dialogLine) EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogLine);
        EventSystem.player.RemoveListener(PlayerEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
        //add override here!
    }
    /// <summary>
    /// Called if the hologram has more hologram to show.
    /// </summary>
    protected virtual void NextHologram()
    {
        //NOOP
    }
    #endregion

    #region typing courtine
    /// <summary>
    /// coroutine to print the line for the kiosk
    /// </summary>
    /// <param name="targetLine">the dialog line to print out</param>
    /// <returns></returns>
    protected IEnumerator PrintKioskLines(HologramDialogLine targetLine)
    {
        var clip = targetLine.transcript;
        speechSource = null;

        //time it takes to wait for the text to finish 
        float timerToWait = (0.5f / textSpeed) * targetLine.line.Length;

        //if there is a clip than play the clip transcipt.
        if (clip.clip)
        {
            PlaySpeechClip(clip);
            timerToWait = Mathf.Max(timerToWait, clip.clip.length);
        }

        timerToWait += targetLine.timerOffset;
        StartCoroutine(TypeNextSentence(targetLine.line));

        //then wait
        float elapseTime = 0;

        while (elapseTime < timerToWait)
        {
            if (isRunning)
            {
                print($"elapse time {elapseTime} timer to wait {timerToWait}");
                if (!speechSource.isPlaying)
                {
                    speechSource.UnPause();
                }
                elapseTime += Time.deltaTime;
                yield return null;
            }
            else
            {
                speechSource.Pause();
                yield return null;
            }
        }


        //audio source related
        //For error catch safety.
        RetrieveAudioSource();

        indexDialog++;
        //ignore this for reference
    }

    #region playing audiosource
    private void PlaySpeechClip(MusicClip clip)
    {
        //if (use3DAudio)
        //{
        //    speechSource = SoundManager.Instance.PlayMusicClip(clip, transform.position);
        //}
        //else
        //{
        //    speechSource = SoundManager.Instance.PlayMusicClip(clip);
        //}

        //play a global audiosource instead 
        speechSource = SoundManager.Instance.PlayMusicClip(clip);
    }
    private void PlayGlobalAudioSource()
    {
        //if (use3DAudio)
        //{
        //    globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING, transform.position);
        //}
        //else
        //{
        //    globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
        //}

        globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);

    }

    #endregion


    protected void RetrieveAudioSource()
    {
        if (globalAudioSource) SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        if (speechSource) SoundManager.Instance.RetrieveAudioSource(speechSource);
    }

    protected IEnumerator TypeNextSentence(string text)
    {
        PlayGlobalAudioSource();
        subtitleText.text = "";
        //this is needed since we dont want splits to happen if the player is out of bound.
        string textToDisplay = "";
        int index = 0;
        while (index < text.Length)
        {
            if (isRunning)
            {
                if (!globalAudioSource.isPlaying)
                {
                    globalAudioSource.Play();
                }
                textToDisplay += text[index];
                subtitleText.text = textToDisplay;
                index++;
                yield return new WaitForSeconds(0.5f / textSpeed);
            }
            else
            {
                globalAudioSource.Pause();
                yield return null;
            }
        }
        SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = null;
    }

    #endregion

    #region interupt hologram
    private void InteruptHologram()
    {
        //if (!virtualCamera.gameObject.active) return;
        StopAllCoroutines();
        RetrieveAudioSource(); //basically stop the audio from playing
        OnInteruptHologram();
        //remove the listener since it is not needed anymore.
        EventSystem.player.RemoveListener(PlayerEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
    }

    protected virtual void OnInteruptHologram()
    {
        //NOOP
    }
    #endregion

    #region sound effect related
    public void PlayNewSlideSFX()
    {
        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN);
    }

    public void PlayCloseHologramSFX()
    {
        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE);
    }
    #endregion

    #region legacy
    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (other.transform == GameData.playerTransform && !isRunning)
    //    //{
    //    //    print("enter");
    //    //    //Resume the hologram
    //    //    isRunning = true;
    //    //    EventSystem.player.TriggerEvent(PlayerEvents.UNPAUSE_HOLOGRAM);
    //    //}
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.transform == GameData.playerTransform && isRunning)
    //    {
    //        print("exit");
    //        isRunning = false;
    //        EventSystem.player.TriggerEvent<HologramSlideShowData>(PlayerEvents.PAUSE_HOLOGRAM , _Data);
    //    }
    //}
    #endregion
}
