using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Hologram_3D : Hologram <Hologram3DData>
{
    [SerializeField] Hologram3DData _dataForDebugging;
    [SerializeField] Transform targetPosition;
    [SerializeField] Material hologramMaterial;
    [SerializeField] float hologram3DFadeInSec = 1f;
    GameObject current3DHologram;

    [Header("children gameobject")]
    [SerializeField]  GameObject hololights;
    [SerializeField] GameObject hologramPanel;

    Transform originalTargetPosition;
    TextMeshProUGUI originalTextComponent;


    [ContextMenu("Set and play")]
    public void Set3DData()
    {
        InitHologram(_dataForDebugging);
        PlayAnimation();
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);

        originalTextComponent = subtitleText;
        originalTargetPosition = targetPosition;
    }

    protected override void OnCompleteLine()
    {
        animator.SetTrigger("Complete");
    }

    //call in the 3d hologram animator
    void Spawn3DHologram()
    {
        if(current3DHologram) current3DHologram.SetActive(false);
        current3DHologram = Instantiate(_Data.Lines[indexDialog].prefab3D, targetPosition);
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
        GetComponent<Collider>().enabled = false;
        enabled = false;
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
    void DisableHologram()
    {
        hologramPanel.SetActive(false);
        hololights.SetActive(false);
    }

    void EnabledHologram()
    {
        hologramPanel.SetActive(true);
        hololights.SetActive(true);
    }

    void SetHologramFadeValue(float value)
    {
        hologramMaterial.SetFloat("_Fade_In", value);
    }


    protected override void OnPlayerEnterTrigger()
    {
        //so basically transfer the information to the portable hologram
        Hologram_Portable portableHologram = GameData.playerHologram;
        //change the reference to the original hologram text
        originalTextComponent.text = portableHologram.Text.text;
        targetPosition = portableHologram.Placement3D;

        //settle the gameobject.
        current3DHologram.gameObject.SetActive(false);
        portableHologram.Hide();
        subtitleText = originalTextComponent;
        //afterwards, change the imageComponent and text to look the same!

        
        current3DHologram = Instantiate(_Data.Lines[indexDialog].prefab3D, targetPosition);
        //afterward hide the component
        //hide the slideshow hologram
        animator.SetTrigger("EnableHologram");
    }

    protected override void OnPlayerExitTrigger()
    {
        var portableHologram = GameData.playerHologram;
        //change the reference
        targetPosition = portableHologram.Placement3D;
        subtitleText = portableHologram.Text;

        //make a copy of the animation in the 3d position for the camera to render
        current3DHologram.SetActive(false);
        current3DHologram = Instantiate(_Data.Lines[indexDialog].prefab3D, targetPosition);
        //afterwards replace the text and show the hologram
        subtitleText.text = originalTextComponent.text;
        portableHologram.Show();

        //afterward hide the component
        //hide the slideshow hologram
        animator.SetTrigger("DisableHologram");
    }
}
