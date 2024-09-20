using Dialog;
using System.Collections;
using UnityEngine;
using TMPro;
using SoundRelated;

/// <summary>
/// The reference to all the different types of holograms that exist in the game. 
/// </summary>
public abstract class BaseHologram : MonoBehaviour
{
    public abstract void PlayAnimation();
}

/// <summary>
/// Class to structure how a hologram is being being played. Specify a hologram data
/// that will be used in the children script and the way display the hologram through overrides.
/// </summary>
/// <typeparam name="DataType">The type of hologram data that will be passed to</typeparam>
public abstract class Hologram<DataType> : BaseHologram where DataType : HologramData
{
    [SerializeField]
    protected Animator animator;

    [Header("Subtitles")]
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float letterPerSecond = 0.3f;
    protected int curIndex = 0;

    [SerializeField] AudioSource typingAudioSource;
    [SerializeField] AudioSource speechSource;

    protected DialogueLines dialogLine;

    [SerializeField] protected DataType _Data;
    protected bool isRunning = false;

    [SerializeField] float exitRadius = 7.4f;
    bool hasTriggeredPortableHologram = false;

    Coroutine currentCoroutine;
    Coroutine typingCoroutine;
    /// <summary>
    /// How the hologram should start, override it if you want the hologram
    /// to do something at start.
    /// </summary>
    protected virtual void Start()
    {
        //NOOP
    }

    //how a typical hologram would play can change this to abstract if
    //all hologram needs to operate differently
    [ContextMenu("Run animation manually")]
    public override void PlayAnimation()
    {
        isRunning = true;
        gameObject.SetActive(true);
        EventSystem.level.TriggerEvent(LevelEvents.INTERRUPT_HOLOGRAM);
        EventSystem.level.AddListener(LevelEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
    }

    /// <summary>
    /// setting the related data to the hologram
    /// </summary>
    /// <param name="data">The data that is set for the hologram</param>
    public virtual void InitHologram(DataType data)
    {
        ScriptableObjectManager.AddIntoSOCollection(data);
        _Data = (DataType)ScriptableObjectManager.RetrieveRuntimeScriptableObject(data);
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

    /// <summary>
    /// The method to start running the hologram.
    /// </summary>
    protected void RunPanel()
    {
        if (currentCoroutine == null)
        {
            //make sure that it is run once even if it is called other places.
            currentCoroutine = StartCoroutine(RunHologram());
        }
    }

    /// <summary>
    /// The hologram behaviour runs on coroutine. It will
    /// 1. Complete printing the current kiosk line 
    /// 2. calls whatever function after it completes the line 
    /// 3. See if there is more hologram data to display
    /// 
    /// If it has, it will calls functions to run the RunPanel(),
    /// Else it will call functions to end the hologram.
    /// </summary>
    private IEnumerator RunHologram()
    {
        yield return PrintKioskLines(_Data.dialogLine[curIndex]);
        OnCompleteLine();
        DecideState();
    }

    /// <summary>
    /// Checks the distance between the player and the hologram. It the player
    /// is very far, it would call the OnPlayerOutOfView() method else,
    /// it would call the OnPlayerWithinView(). 
    /// </summary>
    private void Update()
    {
        bool acceptableDistance = Vector3.Distance(GameData.playerTransform.position, this.transform.position) < exitRadius;
        if (isRunning && !acceptableDistance && !hasTriggeredPortableHologram)
        {
            //if outside of acceptable distance and has not trigger the portable hologram.
            hasTriggeredPortableHologram = true;
            OnPlayerOutOfView();
        }
        else if (isRunning && acceptableDistance && hasTriggeredPortableHologram)
        {
            //if the player is within acceptable distance and has trigger portable hologram
            hasTriggeredPortableHologram = false;
            OnPlayerWithinView();
        }
    }

    /// <summary>
    /// Deciding which state the Hologram would be doing.
    /// </summary>
    private void DecideState()
    {
        if (curIndex >= _Data.dialogLine.Length)
        {
            FinishHologram();
        }
        else
        {
            currentCoroutine = null;
            OnNextHologram();
        }
    }

    /// <summary>
    /// When the hologram finishes all its panel. it will call this function
    /// </summary>
    private void FinishHologram()
    {
        if (dialogLine) EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogLine);
        EventSystem.level.RemoveListener(LevelEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
        isRunning = false;
        OnEndHologram();
    }

    #region on complete function
    /// <summary>
    /// Once a line has been completed, this function will call.
    /// Override it to extend the functionality
    /// </summary>
    protected virtual void OnCompleteLine()
    {
        //NOOP
    }

    /// <summary>
    /// called if the hologram is reach the end of the dialog
    /// </summary>
    protected virtual void OnEndHologram()
    {
        //NOOP
    }
    /// <summary>
    /// called when the current panel is finish. The action required to 
    /// move on to the next panel should be implemented here!
    /// </summary>
    protected virtual void OnNextHologram()
    {
        //NOOP
    }

    protected virtual void OnPlayerOutOfView()
    {
        //NOOP
    }
    protected virtual void OnPlayerWithinView()
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

        //time it takes to wait for the stateText to finish + 0.1 buffer
        float timerToWait = letterPerSecond * targetLine.line.Length + 0.1f;

        //if there is a clip than play the clip transcipt.
        if (clip.clip)
        {
            PlaySpeechClip(clip);
            timerToWait = Mathf.Max(timerToWait, clip.clip.length);
        }

        timerToWait += targetLine.timerOffset;
        typingCoroutine = StartCoroutine(TypeNextSentence(targetLine.line));

        //afterwards, wait for the either the timer to finish or speech to finish.
        yield return new WaitForSeconds(timerToWait);


        StopCoroutine(typingCoroutine);
        typingCoroutine = null;

        //audio source related
        RetrieveAudioSource();

        //increment the index since it is already done.
        curIndex++;
    }

    protected IEnumerator TypeNextSentence(string text)
    {
        PlayGlobalAudioSource();
        subtitleText.text = "";
        //this is needed since we dont want splits to happen if the player is out of bound.
        string textToDisplay = "";
        //slowly place in the words into the subtile stateText
        foreach (var c in text.ToCharArray())
        {
            textToDisplay += c;
            subtitleText.text = textToDisplay;
            yield return new WaitForSeconds(letterPerSecond);
        }

        print("retrieve audio here");
        SoundManager.Instance.RetrieveAudioSource(typingAudioSource);
        typingAudioSource = null;
    }
    #endregion

    #region playing audiosource
    private void PlaySpeechClip(MusicClip clip)
    {
        //play a global audiosource instead 
        speechSource = SoundManager.Instance.PlayMusicClip(clip);
    }

    private void PlayGlobalAudioSource()
    {
        typingAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
    }

    protected void RetrieveAudioSource()
    {
        if (typingAudioSource) SoundManager.Instance.RetrieveAudioSource(typingAudioSource);
        if (speechSource) SoundManager.Instance.RetrieveAudioSource(speechSource);

        typingAudioSource = null;
        speechSource = null;
    }
    #endregion

    #region interupt hologram
    /// <summary>
    /// Stop the hologram from running
    /// </summary>
    private void InteruptHologram()
    {
        StopAllCoroutines();
        RetrieveAudioSource(); //basically stop the audio from playing
        OnInteruptHologram();
        //remove the listener since it is not needed anymore.
        EventSystem.level.RemoveListener(LevelEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
    }

    /// <summary>
    /// any addition to the interupt hologram state can be added here.
    /// </summary>
    protected virtual void OnInteruptHologram()
    {
        //NOOP
    }
    #endregion

    #region sound effect related
    public void PlayNewPanelSFX()
    {
        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN);
    }

    public void PlayClosePanelSFX()
    {
        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE);
    }
    #endregion
}
