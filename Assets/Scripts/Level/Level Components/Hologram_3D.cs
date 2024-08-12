using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram_3D : Hologram
{
    [SerializeField] Hologram3DData _data;
    [SerializeField] Transform targetPosition;
    [SerializeField] Material hologramMaterial;
    [SerializeField] float hologram3DFadeInSec = 1f;
    GameObject current3DHologram;
    DialogueLines dialogLine;
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public override void PlayAnimation()
    {
        gameObject.SetActive(true);
    }

    IEnumerator RunPanel()
    {
        yield return PrintKioskLines(new
            HologramDialogLine(_data.Lines[indexDialog]));
        animator.SetTrigger("Complete");
        DecideNextState();
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

    void DecideNextState()
    {
        if (indexDialog >= _data.Lines.Length)
        {
            //dialog is complete
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogLine);
            //do another animation here to hide hologram
            animator.SetTrigger("HideHologram");
        }
        else
        {
            //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            //change the trigger
            //do an animation to hide the gameobject with the target position
            animator.SetTrigger("NewHologram");
        }
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

    void StartHologram()
    {
        StartCoroutine(RunPanel());
    }

    void HideHologram()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }

    void SetHologramFadeValue(float value)
    {
        hologramMaterial.SetFloat("_Fade_In", value);
    }

    void PlayCloseHologramSFX()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }

    public override void InitHologram(object data)
    {
        var convertedData= (Hologram3DData)data;
        if(convertedData)
        {
            _data = convertedData;
            ScriptableObjectManager.AddIntoSOCollection(convertedData.DialogAfterComplete);
            dialogLine = (DialogueLines)ScriptableObjectManager.RetrieveRuntimeScriptableObject(convertedData.DialogAfterComplete);
        }
        else
        {
            throw new System.Exception("Cant convert data into 3D data");
        }
    }
}
