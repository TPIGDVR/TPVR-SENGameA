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
        public GameEvents dialogEndTrigger = GameEvents.NONE;
    }

    [CreateAssetMenu(menuName = "Dialogue/kisok lines")]
    public class KisokLines : ScriptableObject
    {
        public KLine[] Lines;
    }

    [System.Serializable]
    public class Line
    {
        public enum Speakers
        {
            EVE,
            Markiplier,
            HIM
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