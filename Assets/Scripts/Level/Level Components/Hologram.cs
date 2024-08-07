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
}
