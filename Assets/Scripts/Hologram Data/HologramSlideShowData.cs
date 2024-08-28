using Dialog;
using JetBrains.Annotations;
using SoundRelated;
using System;
using UnityEngine;

public class HologramData : ScriptableObject
{
    //set the reference in the hologram
    [HideInInspector]
    public virtual HologramDialogLine[] dialogLine => null;
    public DialogueLines DialogAfterComplete;

}

[CreateAssetMenu(menuName = "Hologram/Slide show data")]
public class HologramSlideShowData : HologramData
{
    public HologramSlideShowLine[] Lines;
    public override HologramDialogLine[] dialogLine => Lines;
    //public DialogueLines DialogAfterComplete; 
}

#region data
[Serializable]
public class HologramDialogLine
{
    [TextArea(3,5)]
    public string line;
    public MusicClip transcript;
    public float timerOffset;
}

[Serializable]
public class Hologram3DSlideShowLine: HologramDialogLine
{
    public GameObject prefab3D; 
}

[Serializable]
public class HologramSlideShowLine  : HologramDialogLine
{
    public Sprite image;
    public Vector2 preferredImageSize;
}

#endregion