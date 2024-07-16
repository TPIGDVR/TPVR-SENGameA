using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    EventManager<LevelEvents> em_l = EventSystem.level;
    DialogueBasic _currentDialogue;

    private void Start()
    {
        em_l.AddListener<DialogueBasic>(LevelEvents.ENTER_DIALOGUE_TRIGGER,ChangeDialogue);
    }

    void ChangeDialogue(DialogueBasic d)
    {
        _currentDialogue = d;
    }

    public void NextDialogueLine()
    {
        _currentDialogue.NextDialogue();
        _currentDialogue.PrintCurrentDialogueLine();
    }

    public void PreviousDialogueLine()
    {
        _currentDialogue.PreviousDialogue();
        _currentDialogue.PrintCurrentDialogueLine();
    }
}
