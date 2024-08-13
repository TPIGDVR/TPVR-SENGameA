using Dialog;
using JetBrains.Annotations;
using SoundRelated;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Hologram/Slide show data")]
public class HologramSlideShowData : ScriptableObject
{
    public HologramSlideShowLine[] Lines;
    public DialogueLines DialogAfterComplete; 
}

[Serializable]
public struct Hologram3DSlideShowLine
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timerOffset;
    public GameObject prefab3D;
}

[Serializable]
public struct HologramSlideShowLine 
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timerOffset;
    public Sprite image;
    public Vector2 preferredImageSize;
}

[Serializable]
public struct HologramDialogLine
{
    [TextArea(3,5)]
    public string line;
    public MusicClip transcript;
    public float timerOffset;

    public HologramDialogLine(HologramSlideShowLine data)
    {
        line = data.line;
        transcript = data.transcript;
        timerOffset = data.timerOffset;
    }

    public HologramDialogLine(Hologram3DSlideShowLine data)
    {
        line = data.line;
        transcript = data.transcript;
        timerOffset = data.timerOffset;
    }
}
