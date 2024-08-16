using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hologram_SlideShow_Portable : Hologram<HologramSlideShowData>
{
    [SerializeField] Image image;

    protected override void Start()
    {
        base.Start();
        EventSystem.player.AddListener<HologramSlideShowData>(PlayerEvents.PAUSE_HOLOGRAM, AssignData);
        gameObject.SetActive(false);

    }

    private void AssignData(HologramSlideShowData data)
    {
        indexDialog = 0;
        _Data = data;
        dialogLine = (DialogueLines) ScriptableObjectManager.RetrieveRuntimeScriptableObject(data.DialogAfterComplete);
        PlayAnimation();
    }

    public override void PlayAnimation()
    {
        base.PlayAnimation();
        image.sprite = _Data.Lines[indexDialog].image;
        RunPanel();
    }

    protected override void NextHologram()
    {
        image.sprite = _Data.Lines[indexDialog].image;
        RunPanel();
    }

    protected override void EndHologram()
    {
        base.EndHologram();
        gameObject.SetActive(false);
    }

}
