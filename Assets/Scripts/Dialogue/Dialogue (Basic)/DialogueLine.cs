using UnityEngine;

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

    [System.Serializable]
    public class Line
    {
        public enum Speakers
        {
            Markiplier,
            HIM
        }

        public Speakers Speaker;
        public string Text;
    }
}