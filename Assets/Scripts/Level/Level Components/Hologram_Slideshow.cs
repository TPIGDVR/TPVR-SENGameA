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
    DialogueLines dialogLine;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    //CALL THIS METHOD FROM KIOSK CLASS
    public override void PlayAnimation()
    {
        //play the animation here...
        gameObject.SetActive(true);
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
            HologramDialogLine(slideshowData.Lines[indexDialog]));

        if (indexDialog >= slideshowData.Lines.Length)
        {
            //dialog is complete
            //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
            //if can trigger line than trigger the dialog sequence
            EventSystem.dialog.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, dialogLine);

            animator.SetTrigger("HidePanel");
        }
        else
        {
            //audioSource.PlayOneShot(audioClips[3]);
            //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
            //change the trigger
            animator.SetTrigger("ShowImage");
        }
    }

    //will be played at the animator
    void PlayNewSlideSFX()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
    }

    void PlayCloseHologramSFX()
    {
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    }

    public override void InitHologram(object data)
    {
        var convertedData = (HologramSlideShowData)data;
        if (convertedData)
        {
            slideshowData = convertedData;
            ScriptableObjectManager.AddIntoSOCollection(convertedData.DialogAfterComplete);
            dialogLine = (DialogueLines)ScriptableObjectManager.RetrieveRuntimeScriptableObject(convertedData.DialogAfterComplete);
        }
        else
        {
            throw new System.Exception("Cant convert data into slideshow data");
        }
    }
}
