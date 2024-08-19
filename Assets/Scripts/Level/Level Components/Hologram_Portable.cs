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

    [Header("Slide show")]
    [SerializeField] Image image;

    [Header("3D hologram")]
    [SerializeField] RenderTexture textureReference;
    [SerializeField] Transform placement3D;
    public bool IsActive => gameObject.activeSelf;
    public Image Image { get => image; set => image = value; }
    public TextMeshProUGUI Text { get => text; set => text = value; }

    private void Start()
    {
        GameData.playerHologram = this;
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

}
