using Dialog;
using System.Collections;
using UnityEngine;
using TMPro;
using SoundRelated;

public abstract class BaseHologram : MonoBehaviour
{
    public abstract void PlayAnimation();
}

public abstract class Hologram<DataType> : BaseHologram where DataType : HologramData 
{
    [SerializeField]
    protected Animator animator;

    [Header("Subtitles")]
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float textSpeed = 20f;
    protected int indexDialog = 0;

    //protected SoundManager soundManager;
    AudioSource globalAudioSource;
    AudioSource speechSource;
    DialogueLines dialogLine;

    [SerializeField] protected DataType _Data;

    [SerializeField]
    protected GameObject virtualCamera;

    protected virtual void Start()
    {
        EventSystem.player.AddListener(PlayerEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
    }

    //how a typical hologram would play can change this to abstract if
    //all hologram needs to operate differently
    public override void PlayAnimation()
    {
        gameObject.SetActive(true);
        //virtualCamera.SetActive(true);
        EventSystem.player.TriggerEvent(PlayerEvents.START_PLAYING_HOLOGRAM);
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

    protected virtual void OnCompleteLine()
    {
        //NOOP
    }

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

    protected virtual void EndHologram()
    {
        var dialogLine = _Data.DialogAfterComplete;
        if (dialogLine) EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogLine);
        //add override here!
    }

    protected virtual void NextHologram()
    {
        //NOOP
    }

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
            speechSource = SoundManager.Instance.PlayMusicClip(clip, transform.position);
            timerToWait = Mathf.Max(timerToWait, clip.clip.length);
        }

        timerToWait += targetLine.timerOffset;
        StartCoroutine(TypeNextSentence(targetLine.line));
        yield return new WaitForSeconds(timerToWait);

        //audio source related
        //For error catch safety.
        RetrieveAudioSource();

        indexDialog++;
        //ignore this for reference
    }

    private void RetrieveAudioSource()
    {
        if (globalAudioSource) SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        if (speechSource) SoundManager.Instance.RetrieveAudioSource(speechSource);
    }

    protected IEnumerator TypeNextSentence(string text)
    {
        globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING,transform.position);
        subtitleText.text = "";

        foreach (char c in text.ToCharArray())
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = null;
    }


    #endregion

    #region interupt hologram
    private void InteruptHologram()
    {
        if (!virtualCamera.gameObject.active) return;
        StopAllCoroutines();
        RetrieveAudioSource();//basically stop the audio from playing
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
        print("running");
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
    }

    public void PlayCloseHologramSFX()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }
    #endregion
}
