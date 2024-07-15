using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerVFX : MonoBehaviour
{

    #region FADE VARIABLES
    Vignette _vig;
    [SerializeField]
    Image _fadeImg;

    [SerializeField]
    float fadeTime;
    public float fadeTimer;
    #endregion

    EventManager<GameEvents> em = EventSystem.game;
    private void Start()
    {

        em.AddListener(GameEvents.LOSE,BeginFadeScreen);
        em.AddListener(GameEvents.ENTER_NEW_SCENE,BeginUnfadeScreen);

        //test
        //BeginFadeScreen();
    }


    void BeginFadeScreen()
    {
        StartCoroutine(FadeScreen());
    }

    void BeginUnfadeScreen()
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
            //_vig.intensity.value = Mathf.Lerp(0, 5, fadeTimer/fadeTime);
        }

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
            //_vig.intensity.value = Mathf.Lerp(0, 5, fadeTimer/fadeTime);
        }

    }
}
