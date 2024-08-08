using Dialog;
using System.Collections;
using UnityEngine;
using TMPro;
using SoundRelated;

public abstract class Hologram : MonoBehaviour , IScriptLoadQueuer
{
    [SerializeField]
    protected Animator animator;

    [Header("Subtitles")]
    //[SerializeField]
    //protected KioskLines kioskData;
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float textSpeed = 20f;
    protected int indexDialog;

    [Header("Dialogue After Kiosk")]
    [SerializeField]
    DialogueLines dialogueAfterKioskData;

    protected SoundManager soundManager;
    AudioSource globalAudioSource;

    //audio

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL);
    }

    public abstract void PlayAnimation();

    public void Initialize()
    {
        soundManager = SoundManager.Instance;
    }

    #region typing courtine

    protected IEnumerator PrintKioskLines(float second)
    {
        var clip = slideshowData.Lines[indexDialog].clip;
        AudioSource speechSource = null;

        if (clip.clip)
        {
            speechSource = SoundManager.Instance.PlayMusicClip(clip, transform.position);
        }
        StartCoroutine(TypeNextSentence());
        yield return new WaitForSeconds(second);

        //audio source related
        //For error catch safety.
        if (globalAudioSource) soundManager.RetrieveAudioSource(globalAudioSource);
        if (speechSource) soundManager.RetrieveAudioSource(speechSource);

        indexDialog++;

        if (indexDialog >= kioskData.Lines.Length)
        {
            //dialog is complete
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogueAfterKioskData);
        }
        else
        {
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
        }
    }

    protected IEnumerator TypeNextSentence()
    {
        globalAudioSource = soundManager.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
        subtitleText.text = "";
        string text = kioskData.Lines[indexDialog].Text;

        foreach (char c in text.ToCharArray())
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        soundManager.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = null;
    }

    #endregion
}
