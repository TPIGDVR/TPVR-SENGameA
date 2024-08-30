using Dialog;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hologram_Portable : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Animator animator;
    [Header("Slide show")]
    [SerializeField] Image imageComponent;

    [Header("3D hologram")]
    [SerializeField] GameObject hologramRenderer;
    [SerializeField] RawImage rawImageComponent;
    [SerializeField] Transform placement3D;
    public bool IsActive => gameObject.activeSelf;
    public Image SlideShowImage { get {
            rawImageComponent.gameObject.SetActive(false);
            hologramRenderer.SetActive(false);
            imageComponent.gameObject.SetActive(true);
            return imageComponent;
        } }

    public TextMeshProUGUI Text { get => text;}
    public Transform Placement3D { get {
            //make sure the raw image and hologram render is rendering the gameobject
            rawImageComponent.gameObject.SetActive(true);
            hologramRenderer.SetActive(true);
            //then hide the component.
            imageComponent.gameObject.SetActive(false);
            return placement3D;
        } }


    int hideHash = Animator.StringToHash("Hide");
    int showHash = Animator.StringToHash("Show");
    private void Start()
    {
        GameData.playerHologram = this;
        HideGameobject();
    }

    public void Hide()
    {

        animator.ResetTrigger(showHash);
        animator.SetTrigger(hideHash);
    }

    void HideGameobject()
    {
        hologramRenderer.SetActive(false);
    }

    //will automatically show the animation to the display.
    public void Show()
    {
        animator.ResetTrigger(hideHash);
        animator.SetTrigger(showHash);
    }

}
