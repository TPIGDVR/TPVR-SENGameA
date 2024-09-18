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
    GameObject _PlayerRig;

    [SerializeField]
    float fadeTime;
    public float fadeTimer;
    public float faintHeight;

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
        float playerHeight = _PlayerRig.GetComponent<CharacterController>().height;
        while(fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            fadeTimer += interval;
            float rotationY = _PlayerRig.transform.rotation.y;
            float rotationY_Lerp = _PlayerRig.transform.rotation.y + 180;
            _PlayerRig.GetComponent<CharacterController>().height = Mathf.Lerp(playerHeight , faintHeight, fadeTimer / fadeTime);
            _PlayerRig.transform.rotation = Quaternion.Lerp(new Quaternion(0,rotationY,0,0), new Quaternion(0, rotationY_Lerp, 0, 0), fadeTimer / fadeTime);
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeTimer / fadeTime);
        }

        _PlayerRig.GetComponent<CharacterController>().height = playerHeight;
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
