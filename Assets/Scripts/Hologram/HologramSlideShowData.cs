using Dialog;
using SoundRelated;
using System;
using UnityEngine;


public class HologramSlideShowData : ScriptableObject
{
    public HologramSlideShowBasic[] Lines;
    public DialogueLines OtherDialogue; 
    
}

[Serializable]
public struct HologramSlideShowBasic 
{
    [TextArea(3, 5)]
    public string line;
    public MusicClip transcript;
    public float timer;
    public Sprite image;
    public Vector2 preferredImageSize;
}

[Serializable]
public struct HologramDialogLineBasic
{
    [TextArea(3,5)]
    public string line;
    public MusicClip transcript;
    public float timer;

    public HologramDialogLineBasic(HologramSlideShowBasic data)
    {
        line = data.line;
        transcript = data.transcript;
        timer = data.timer;
    }
}
