using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram_3D : Hologram
{
    [SerializeField] Hologram3DData _data;
    [SerializeField] Transform targetPosition;
    GameObject current3DHologram;
    public override void PlayAnimation()
    {
        
    }

    IEnumerator RunPanel()
    {
        current3DHologram = Instantiate(_data.Lines[indexDialog].prefab3D,targetPosition);
        yield return PrintKioskLines(new
            HologramDialogLine(_data.Lines[indexDialog]));

        if (indexDialog >= _data.Lines.Length)
        {
            //dialog is complete
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, _data.DialogAfterComplete);
            //do another animation here to hide hologram
        }
        else
        {
            //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            //change the trigger
            //do an animation to hide the gameobject with the target position
            
        }
    }

    void Change3DHologram()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
        current3DHologram.SetActive(false);
        current3DHologram = Instantiate(_data.Lines[indexDialog].prefab3D, targetPosition);
    }

    void StartHologram()
    {
        StartCoroutine(RunPanel());
    }

    void HideHologram()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }
}
