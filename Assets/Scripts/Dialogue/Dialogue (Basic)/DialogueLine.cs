using SoundRelated;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Dialog
{

    [CreateAssetMenu(menuName = "Dialogue/Lines")]
    public class DialogueLines : ScriptableObject
    {
        public Line[] Lines;
    }

    [System.Serializable]
    public class Line
    {
        public enum Speakers
        {
            EVE
        }

        public Speakers Speaker;
        [TextArea(5,5)]
        public string Text;
        public bool hasBeenTriggered = false;
        public DialogEvents dialogEndTrigger = DialogEvents.NONE;

        public MusicClip audioClip;
    }
}