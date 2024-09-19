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

    public IEnumerator FadeScreen()
    {
        float interval = 0.01f;
        fadeTimer = 0;
        float playerHeight = _PlayerRig.GetComponent<CharacterController>().height;
        Quaternion playerRotation = _PlayerRig.transform.rotation;
        _PlayerRig.GetComponent<OVRCameraRig>().enabled = false;
        while (fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            fadeTimer += interval;

            _PlayerRig.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0,playerRotation.y, 0), Quaternion.Euler(90,playerRotation.y,0), fadeTimer / fadeTime);
            _PlayerRig.GetComponent<CharacterController>().height = Mathf.Lerp(playerHeight , faintHeight, fadeTimer / fadeTime);
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeTimer / fadeTime);
        }
        _PlayerRig.transform.rotation = playerRotation;
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
