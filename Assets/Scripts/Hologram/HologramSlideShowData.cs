using Dialog;
using SoundRelated;
using System;
using UnityEngine;


public class HologramSlideShowData : ScriptableObject
{
    public HologramShowLineBasic[] Lines;
    public DialogueLines OtherDialogue; 
    
}

[Serializable]
public struct HologramShowLineBasic
{
    [TextArea(3,5)]
    public string line;
    public MusicClip transcript;
    public float timer;
}
