using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hologram_Slideshow : Hologram
{
    [SerializeField] 
    Image image;
    [SerializeField] 
    GridLayoutGroup imageSizer;

    protected override void PlayAnimation()
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
        ChangeImage();
        subtitleText.text = "";
    }

    //called by animation event : Image Change
    void StartTyping()
    {
        StopAllCoroutines();
        StartCoroutine(PrintKioskLines(kioskData.Lines[indexDialog].duration));
    }

    void ChangeImage()
    {
        var line = kioskData.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredDimension;
    }
}
