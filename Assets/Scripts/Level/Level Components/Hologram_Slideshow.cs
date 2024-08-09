using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hologram_Slideshow : Hologram
{
    [SerializeField]
    HologramSlideShowData slideshowData;

    [SerializeField] 
    Image image;
    [SerializeField] 
    GridLayoutGroup imageSizer;

    //CALL THIS METHOD FROM KIOSK CLASS
    public override void PlayAnimation()
    {
        //play the animation here...
    }

    //called by animation event : DigitalCircle_Completed
    void StartKioskDialog()
    {
        ChangeImage();
        StartTyping();
    }

    //called by animation event : Image Change
    void ChangeImagePanel()
    {
        ChangeImage();
        subtitleText.text = "";
    }

    //called by animation event : Image Change
    void StartTyping()
    {
        StopAllCoroutines();
        StartCoroutine(RunPanel());
    }

    void ChangeImage()
    {
        var line = slideshowData.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredImageSize;
    }    

    IEnumerator RunPanel()
    {
        yield return PrintKioskLines(new
            HologramDialogLineBasic(slideshowData.Lines[indexDialog]));

        if (indexDialog >= slideshowData.Lines.Length)
        {
            //dialog is complete
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, slideshowData.OtherDialogue);
            
            //animator.SetTrigger(hidePanelHash);
        }
        else
        {
            //audioSource.PlayOneShot(audioClips[3]);
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            //change the trigger
            //animator.SetTrigger(changePanel);
        }
    }

}
