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
    protected int indexDialog = 0;

    [Header("Dialogue After Kiosk")]
    [SerializeField]
    DialogueLines dialogueAfterKioskData;

    protected SoundManager soundManager;
    AudioSource globalAudioSource;

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

    protected IEnumerator PrintKioskLines(HologramDialogLineBasic targetLine)
    {
        var clip = targetLine.transcript;
        AudioSource speechSource = null;

        if (clip.clip)
        {
            speechSource = SoundManager.Instance.PlayMusicClip(clip, transform.position);
        }
        StartCoroutine(TypeNextSentence(targetLine.line));
        yield return new WaitForSeconds(targetLine.timer);

        //audio source related
        //For error catch safety.
        if (globalAudioSource) soundManager.RetrieveAudioSource(globalAudioSource);
        if (speechSource) soundManager.RetrieveAudioSource(speechSource);

        indexDialog++;
        //ignore this for reference

        //if (indexDialog >= kioskData.Lines.Length)
        //{
        //    //dialog is complete
        //    SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
        //    //if can trigger targetLine than trigger the dialog sequence
        //    EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogueAfterKioskData);
        //}
        //else
        //{
        //    SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
        //}
    }

    protected IEnumerator TypeNextSentence(string text)
    {
        globalAudioSource = soundManager.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
        subtitleText.text = "";
        //string text = kioskData.Lines[indexDialog].Text;

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
