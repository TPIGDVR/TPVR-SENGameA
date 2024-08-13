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
    [SerializeField]
    protected TextMeshProUGUI subtitleText;
    [SerializeField]
    float textSpeed = 20f;
    protected int indexDialog = 0;

    //protected SoundManager soundManager;
    AudioSource globalAudioSource;

    public abstract void PlayAnimation();

    public abstract void InitHologram(object data);

    #region typing courtine

    protected IEnumerator PrintKioskLines(HologramDialogLine targetLine)
    {
        var clip = targetLine.transcript;
        AudioSource speechSource = null;

        //time it takes to wait for the text to finish 
        float timerToWait = (0.5f / textSpeed) * targetLine.line.Length;

        //if there is a clip than play the clip transcipt.
        if (clip.clip)
        {
            speechSource = SoundManager.Instance.PlayMusicClip(clip, transform.position);
            timerToWait = Mathf.Max(timerToWait, clip.clip.length);
        }

        timerToWait += 1f;
        StartCoroutine(TypeNextSentence(targetLine.line));
        yield return new WaitForSeconds(timerToWait);

        //audio source related
        //For error catch safety.
        if (globalAudioSource) SoundManager.Instance.RetrieveAudioSource(globalAudioSource);
        if (speechSource) SoundManager.Instance.RetrieveAudioSource(speechSource);

        indexDialog++;
        //ignore this for reference
    }

    protected IEnumerator TypeNextSentence(string text)
    {
        globalAudioSource = SoundManager.Instance.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
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
}
