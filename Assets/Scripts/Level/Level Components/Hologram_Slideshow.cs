using Dialog;
using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    TextMeshProUGUI originalTextComponent;
    Image originalImageComponent;

    [ContextMenu("manual set init")]
    private void ManualSet()
    {
        InitHologram(slideshowData);
    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);

        //store the imageComponent component in another reference to call it again
        originalImageComponent = image;
        originalTextComponent = subtitleText;
    }


    //called by animation event : DigitalCircle_Completed
    void StartKioskDialog()
    {
        ChangeImage();
        StartTyping();
    }

    //called by animation event : SlideShowImage Change
    void ChangeImagePanel()
    {
        ChangeImage();
        subtitleText.text = "";
    }

    //TODO remove this function HERE!!!!
    //called by animation event : SlideShowImage Change
    void StartTyping()
    {
        StopAllCoroutines();
        RunPanel();
    }

    void ChangeImage()
    {
        var line = _Data.Lines[indexDialog];
        image.sprite = line.image;
        imageSizer.cellSize = line.preferredImageSize;
    }

    protected override void OnEndHologram()
    {
        animator.SetTrigger("HidePanel");

        if (GameData.playerHologram.IsActive)
        {
            GameData.playerHologram.Hide();
        }

        //afterwards, set them inactive.
        isRunning = false;
        enabled = false;
    }

    protected override void OnNextHologram()
    {
        base.OnNextHologram();
        animator.SetTrigger("ShowImage");

    }

    protected override void OnInteruptHologram()
    {
        base.OnInteruptHologram();
        //call to end the hologram
        OnEndHologram();
    }


    int hidePanelHash = Animator.StringToHash("HidePanelIfFar");
    int showPanelIfCloseHash = Animator.StringToHash("ShowPanelIfClose");
    protected override void OnPlayerExitTrigger()
    {

        //so basically transfer the information to the portable hologram
        Hologram_Portable portableHologram = GameData.playerHologram;
        //change the reference to the portable hologram stateText
        image = portableHologram.SlideShowImage;
        subtitleText = portableHologram.Text;

        //afterwards, change the imageComponent and stateText to look the same!
        ChangeImage();
        subtitleText.text = originalTextComponent.text;
        portableHologram.Show();

        //afterward hide the component
        //hide the slideshow hologram
        animator.SetTrigger(hidePanelHash);
        animator.ResetTrigger(showPanelIfCloseHash);
    }

    protected override void OnPlayerEnterTrigger()
    {
        //so basically transfer the information to the portable hologram
        Hologram_Portable portableHologram = GameData.playerHologram;

        //change the current imageComponent and stateText to be the same
        originalImageComponent.sprite = image.sprite;
        originalTextComponent.text = subtitleText.text;

        //change the reference to the portable hologram stateText
        image = originalImageComponent;
        subtitleText = originalTextComponent;
        portableHologram.Hide();

        //afterward hide the component
        animator.SetTrigger(showPanelIfCloseHash);
        animator.ResetTrigger(hidePanelHash);
    }
    void HideHologramFit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void ShowHologramFit()
    {
        transform.GetChild(0).gameObject.SetActive(true);

    }
}
