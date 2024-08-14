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
    AudioSource speechSource;
    [SerializeField]
    protected GameObject virtualCamera;

    protected virtual void Start()
    {
        EventSystem.player.AddListener(PlayerEvents.INTERRUPT_HOLOGRAM, InteruptHologram);
    }

    //how a typical hologram would play can change this to abstract if
    //all hologram needs to operate differently
    public virtual void PlayAnimation()
    {
        gameObject.SetActive(true);
        virtualCamera.SetActive(true);
        EventSystem.player.TriggerEvent(PlayerEvents.START_PLAYING_HOLOGRAM);
    }

    public abstract void InitHologram(object data);

    #region typing courtine

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
}
