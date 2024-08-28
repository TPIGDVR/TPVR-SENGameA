using Dialog;
using UnityEngine;

[CreateAssetMenu(menuName = "Hologram/3D data")]
public class Hologram3DData : HologramData
{
    public Hologram3DSlideShowLine[] Lines; 
    public override HologramDialogLine[] dialogLine => Lines;
    //public DialogueLines DialogAfterComplete;
}
