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
        //virtualCamera.SetActive(false);
    }

    private void Update()
    {
        
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
        var line = _Data.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredImageSize;
    }    

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

}
