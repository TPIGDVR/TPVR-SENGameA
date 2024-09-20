using Dialog;
using SoundRelated;
using System;
using UnityEngine;

/// <summary>
/// The basic scriptable object to show the hologram. Use
/// To contain the specific hologram data that is used for reference.
/// </summary>
public class HologramData : ScriptableObject
{
    //set the reference in the hologram
    [HideInInspector]
    /*
        this is a function that should be override if 
        you want to display dialog in the hologram.
        Take a look at the hologram slideshowdata and hologram3D
        data on how it is being used.
    */
    public virtual HologramDialogLine[] dialogLine => null;
    public DialogueLines DialogAfterComplete;
}

[CreateAssetMenu(menuName = "Hologram/Slide show data")]
public class HologramSlideShowData : HologramData
{
    public HologramSlideShowLine[] Lines;
    public override HologramDialogLine[] dialogLine => Lines;
}

#region data
[Serializable]
public class HologramDialogLine
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timerOffset;
}

[Serializable]
public class Hologram3DSlideShowLine : HologramDialogLine
{
    public GameObject prefab3D;
}

[Serializable]
public class HologramSlideShowLine : HologramDialogLine
{
    public Sprite image;
    public Vector2 preferredImageSize;
}

#endregion