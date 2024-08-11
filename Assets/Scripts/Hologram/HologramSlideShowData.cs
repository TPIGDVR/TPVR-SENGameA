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

[CreateAssetMenu(menuName = "Hologram/3D data")]
public class Hologram3DData : ScriptableObject
{
    public Hologram3DSlideLine[] Lines;
    public DialogueLines DialogAfterComplete;
}

[Serializable]
public struct Hologram3DSlideLine
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timer;
    public GameObject prefab3D;
}

[Serializable]
public struct HologramSlideShowLine 
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timer;
    public Sprite image;
    public Vector2 preferredImageSize;
}

[Serializable]
public struct HologramDialogLine
{
    [TextArea(3,5)]
    public string line;
    public MusicClip transcript;
    public float timer;

    public HologramDialogLine(HologramSlideShowLine data)
    {
        line = data.line;
        transcript = data.transcript;
        timer = data.timer;
    }

    public HologramDialogLine(Hologram3DSlideLine data)
    {
        line = data.line;
        transcript = data.transcript;
        timer = data.timer;
    }
}
