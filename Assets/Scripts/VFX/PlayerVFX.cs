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
    GameObject _Camera;
    [SerializeField]
    Transform _TrackingSpace;
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

        while (fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            fadeTimer += interval;
            
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeTimer / fadeTime);
        }
        
        
        isFaded = true;
    }

    public IEnumerator FaintScreen()
    {
        var cc = _PlayerRig.GetComponent<CharacterController>();
        var ovrRig = _PlayerRig.GetComponent<OVRCameraRig>();

        float interval = 0.01f;
        fadeTimer = 0;
        float playerHeight = cc.height;
        Quaternion playerRotation = _TrackingSpace.rotation;


        ovrRig.enabled = false;
        while (fadeTimer < fadeTime)
        {
            yield return new WaitForSeconds(interval);
            var initQuat = Quaternion.Euler(0, _Camera.transform.rotation.y, 0);
            var targetQuat = Quaternion.Euler(0, playerRotation.y, 60f);
            fadeTimer += interval;
            
            _TrackingSpace.rotation = Quaternion.Lerp(
                    playerRotation, 
                    targetQuat, 
                    fadeTimer / fadeTime
                    );

            //lower the player height
            cc.height = Mathf.Lerp(playerHeight, faintHeight, fadeTimer / fadeTime);
            
            _fadeImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeTimer / fadeTime);
        }
        //_Camera.transform.rotation = playerRotation;
        _TrackingSpace.rotation = playerRotation;
        ovrRig.enabled = true;
        cc.height = playerHeight;

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
