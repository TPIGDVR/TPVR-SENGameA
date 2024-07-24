using UnityEngine;
using UnityEngine.Video;

namespace Dialog
{

    [CreateAssetMenu(menuName = "Dialogue/Lines")]
    public class DialogueLines : ScriptableObject
    {
        //public enum Speakers
        //{
        //    Markiplier,
        //    HIM
        //}

        //public Speakers Speaker;

        //public string Line;
        public Line[] Lines;

    
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
            System,
            Markiplier,
            HIM
        }

        public Speakers Speaker;
        public string Text;
    }

    [System.Serializable]
    public class KLine : Line
    {
        [Header("Image related")]
        public Sprite image;
        public Vector2 preferredDimension;
        [Header("Others")]
        public VideoClip clip;
        public float duration = 3f;
    }
}