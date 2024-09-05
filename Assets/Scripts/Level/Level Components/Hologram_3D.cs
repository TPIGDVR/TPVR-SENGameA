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
    int enableHologramhash = Animator.StringToHash("EnableHologram");
    int disableHologramhash = Animator.StringToHash("DisableHologram");

    [ContextMenu("Set and play")]
    public void Set3DData()
    {
        InitHologram(_dataForDebugging);
        PlayAnimation();
    }

    protected override void Start()
    {
        base.Start();
        originalTextComponent = subtitleText;
        originalTargetPosition = targetPosition;

        //set inactive for the panel
        DisableHologram();
        gameObject.SetActive(false);
    }

    public override void PlayAnimation()
    {
        base.PlayAnimation();
        EnabledHologram();
    }

    //call in the 3d hologram animator
    void Spawn3DHologram()
    {
        if(current3DHologram) current3DHologram.SetActive(false);
        current3DHologram = Instantiate(_Data.Lines[curIndex].prefab3D);
        MoveHologramToTargetPosition();
        SetHologramFadeValue(0f);
        StartCoroutine(FadeInHologram());
    }
    //call in the 3d hologram animator
    void Hide3DHologram()
    {
        SetHologramFadeValue(1);
        StartCoroutine(FadeOutHologram());
    }

    protected override void OnInteruptHologram()
    {
        base.OnInteruptHologram();
        OnEndHologram();
    }

    protected override void OnCompleteLine()
    {
        animator.SetTrigger("Complete");
    }
    protected override void OnNextHologram()
    {
        base.OnNextHologram();
        animator.SetTrigger("NewHologram");
    }
    protected override void OnEndHologram()
    {
        base.OnEndHologram();
        //do another animation here to hide hologram

        if (GameData.playerHologram.IsActive)
        {
            GameData.playerHologram.Hide();
        }
        if (current3DHologram) current3DHologram.gameObject.SetActive(false);

        animator.SetTrigger("HideHologram");
        enabled = false;
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
        if (current3DHologram == null) return;
        //so basically transfer the information to the portable hologram
        Hologram_Portable portableHologram = GameData.playerHologram;
        //change the reference to the original hologram stateText
        originalTextComponent.text = portableHologram.Text.text;
        targetPosition = originalTargetPosition;
        subtitleText = originalTextComponent;

        //move the hologram to the target location
        MoveHologramToTargetPosition();

        portableHologram.Hide();
        //afterwards, change the imageComponent and stateText to look the same!

        //current3DHologram = Instantiate(_Data.Lines[curIndex].prefab3D, targetPosition);
        //afterward hide the component
        //hide the slideshow hologram
        animator.SetTrigger(enableHologramhash);
        animator.ResetTrigger(disableHologramhash);
    }

    protected override void OnPlayerExitTrigger()
    {
        var portableHologram = GameData.playerHologram;
        //change and set the reference
        targetPosition = portableHologram.Placement3D;
        subtitleText = portableHologram.Text;
        subtitleText.text = originalTextComponent.text;

        //afterwards replace the stateText and show the hologram
        MoveHologramToTargetPosition();

        //afterward hide the component
        //hide the slideshow hologram
        portableHologram.Show();
        animator.SetTrigger(disableHologramhash);
        animator.ResetTrigger(enableHologramhash);
    }

    private void MoveHologramToTargetPosition()
    {
        var hologramTransform = current3DHologram.transform;
        hologramTransform.parent = targetPosition;
        hologramTransform.localPosition = Vector3.zero;
        hologramTransform.localRotation = Quaternion.identity;
        hologramTransform.localScale = Vector3.one;
    }

}
