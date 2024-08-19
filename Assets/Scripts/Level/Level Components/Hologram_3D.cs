using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram_3D : Hologram <Hologram3DData>
{
    [SerializeField] Hologram3DData _data;
    [SerializeField] Transform targetPosition;
    [SerializeField] Material hologramMaterial;
    [SerializeField] float hologram3DFadeInSec = 1f;
    GameObject current3DHologram;
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }

    protected override void OnCompleteLine()
    {
        animator.SetTrigger("Complete");
    }

    //call in the 3d hologram animator
    void Spawn3DHologram()
    {
        if(current3DHologram) current3DHologram.SetActive(false);
        current3DHologram = Instantiate(_data.Lines[indexDialog].prefab3D, targetPosition);
        SetHologramFadeValue(0f);
        StartCoroutine(FadeInHologram());
    }
    //call in the 3d hologram animator
    void Hide3DHologram()
    {
        SetHologramFadeValue(1);
        StartCoroutine(FadeOutHologram());
    }

    protected override void OnEndHologram()
    {
        base.OnEndHologram();
        //do another animation here to hide hologram
        animator.SetTrigger("HideHologram");
    }

    protected override void OnNextHologram()
    {
        base.OnNextHologram();
        animator.SetTrigger("NewHologram");
    }

    IEnumerator FadeOutHologram() 
    {
        float elapseTime = 0;
        while (elapseTime < hologram3DFadeInSec)
        {
            elapseTime += Time.deltaTime;
            SetHologramFadeValue(1 - (elapseTime / hologram3DFadeInSec));
            yield return null;
        }
        SetHologramFadeValue(0);
        //set the animator here.
        animator.SetTrigger("FinishFadeOut");
        //stop focusing on the camera
    }

    IEnumerator FadeInHologram()
    {
        float elapseTime = 0;
        while(elapseTime < hologram3DFadeInSec)
        {
            print("running fade in hologram");
            elapseTime += Time.deltaTime;
            SetHologramFadeValue(elapseTime / hologram3DFadeInSec);
            yield return null;
        }
        SetHologramFadeValue(1);
    }
    
    //played in the kiosk
    void HideHologram()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }

    void SetHologramFadeValue(float value)
    {
        hologramMaterial.SetFloat("_Fade_In", value);
    }
}
