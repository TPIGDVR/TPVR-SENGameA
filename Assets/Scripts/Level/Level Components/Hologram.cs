using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    protected abstract void PlayAnimation();

    // protected IEnumerator TypeNextSentence()
    // {
    //     globalAudioSource = audioPlayer.PlayAudioContinuous(SoundRelated.SFXClip.TEXT_TYPING);
    //     dialogText.text = "";
    //     string text = kioskData.Lines[indexDialog].Text;

    //     foreach (char c in text.ToCharArray())
    //     {
    //         dialogText.text += c;
    //         yield return new WaitForSeconds(0.5f / textSpeed);
    //     }
    //     audioPlayer.RetrieveAudioSource(globalAudioSource);
    //     globalAudioSource = null;
    // }
}
