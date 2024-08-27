using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField]
    Image _fadeImg;

    [SerializeField]
    float fadeTime;
    public float fadeTimer;

    public bool isFaded = false;

    public void BeginFadeScreen()
    {
        StartCoroutine(FadeScreen());
    }

    public void BeginUnfadeScreen()
    {
        StartCoroutine(UnfadeScreen());
    }

    IEnumerator FadeScreen()
    {
        float interval = 0.01f;
        fadeTimer = 0;
        while(fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            fadeTimer += interval;
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeTimer / fadeTime);
        }

        isFaded = true;
    }

    IEnumerator UnfadeScreen()
    {
        float interval = 0.01f;
        fadeTimer = 0;
        while (fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            fadeTimer += interval;
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), fadeTimer / fadeTime);
        }

        isFaded = false;
    }

    public void DisplayFadeScreen()
    {
        _fadeImg.color = new Color(0, 0, 0, 1);
        isFaded = true;
    }

    public void HideFadeScreen()
    {
        _fadeImg.color = new Color(0, 0, 0, 0);
        isFaded = false;
    }

}
