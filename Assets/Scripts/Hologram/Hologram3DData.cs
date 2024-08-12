using Dialog;
using UnityEngine;

[CreateAssetMenu(menuName = "Hologram/3D data")]
public class Hologram3DData : ScriptableObject
{
    public Hologram3DSlideShowLine[] Lines;
    public DialogueLines DialogAfterComplete;
}
