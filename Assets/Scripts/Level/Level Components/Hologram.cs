using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SoundRelated;

public abstract class Hologram : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;

    [Header("Subtitles")]
    [SerializeField]
    protected KioskLines kioskData;
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float textSpeed = 20f;
    protected int indexDialog;

    [Header("Dialogue After Kiosk")]
    [SerializeField]
    DialogueLines dialogueAfterKioskData;


    //audio
    AudioSource globalAudioSource = null;
    SoundManager soundManager;
    
    protected abstract void PlayAnimation();

    protected IEnumerator PrintKioskLines(float second)
    {
        var clip = kioskData.Lines[indexDialog].clip;
        AudioSource speechSource = null;

        if (clip)
        {
            MusicClip musicClip = new MusicClip(clip);
            speechSource = soundManager.PlayMusicClip(musicClip, transform.position);
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
            //animator.SetTrigger(hidePanelHash);
        }
        else
        {
            //audioSource.PlayOneShot(audioClips[3]);
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            //animator.SetTrigger(changePanel);
        }
        //animator.SetTrigger()
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
}
