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
        public DialogEvents dialogEndTrigger = DialogEvents.NONE;
    }

    [CreateAssetMenu(menuName = "Dialogue/kiosk lines")]
    public class KioskLines : ScriptableObject
    {
        public KLine[] Lines;
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
    }

    [System.Serializable]
    public class KLine : Line
    {
        [Header("Image related")]
        public Sprite image;
        public Vector2 preferredDimension;
        [Header("Others")]
        public AudioClip clip;
        public float duration = 3f;
    }
}