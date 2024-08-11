using Dialog;
using System.Collections;
using UnityEngine;
using TMPro;
using SoundRelated;

public abstract class Hologram : MonoBehaviour
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

    //protected SoundManager soundManager;
    AudioSource globalAudioSource;



    public abstract void PlayAnimation();


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
        if (globalAudioSource) SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        if (speechSource) SoundManager.Instance.RetrieveAudioSource(speechSource);

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
        globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
        subtitleText.text = "";
        //string text = kioskData.Lines[indexDialog].Text;

        foreach (char c in text.ToCharArray())
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(0.5f / textSpeed);
        }
        SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        globalAudioSource = null;
    }

    #endregion
}
