using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hologram_Slideshow : Hologram<HologramSlideShowData>
{
    [SerializeField]
    HologramSlideShowData slideshowData;

    [SerializeField] 
    Image image;
    [SerializeField] 
    GridLayoutGroup imageSizer;


    [ContextMenu("manual set init")]
    private void ManualSet()
    {
        InitHologram(slideshowData);
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
        virtualCamera.SetActive(false);
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
        print("Image change");
        ChangeImage();
        subtitleText.text = "";
    }

    //called by animation event : Image Change
    void StartTyping()
    {
        print("Start typing");
        StopAllCoroutines();
        RunPanel();
    }

    void ChangeImage()
    {
        var line = slideshowData.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredImageSize;
    }    

    //IEnumerator RunPanel()
    //{
    //    yield return PrintKioskLines(slideshowData.Lines[indexDialog]);

    //    if (indexDialog >= slideshowData.Lines.Length)
    //    {
    //        //dialog is complete
    //        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HOLOGRAM_CLOSE, transform.position);
    //        //if can trigger line than trigger the dialog sequence
    //        EndHologram();
    //    }
    //    else
    //    {
    //        //audioSource.PlayOneShot(audioClips[3]);
    //        //SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
    //        //change the trigger
    //        animator.SetTrigger("ShowImage");
    //    }
    //}

    protected override void OnInteruptHologram()
    {
        EndHologram(); 
    }
    protected override void EndHologram()
    {
        base.EndHologram();
        //stop focusing on the camera
        EventSystem.player.TriggerEvent(PlayerEvents.FINISH_PLAYING_HOLOGRAM);
        animator.SetTrigger("HidePanel");
    }

    protected override void NextHologram()
    {
        base.NextHologram();
        animator.SetTrigger("ShowImage");
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.IMAGE_KIOSK_OPEN, transform.position);
    }

    //will be played at the animator
    

}
