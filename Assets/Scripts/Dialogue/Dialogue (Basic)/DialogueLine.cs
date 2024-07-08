using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Lines")]
public class DialogueLine : ScriptableObject
{
    public enum Speakers
    {
        Markiplier,
        HIM
    }

    public Speakers Speaker;

    public string Line;
}
