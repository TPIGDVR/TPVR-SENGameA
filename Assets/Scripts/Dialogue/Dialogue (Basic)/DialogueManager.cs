using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    EventManager<LevelEvents> em_l = EventSystem.level;
    DialogueTrigger _currentD;
    Line _currentLine;

    [SerializeField]
    GameObject dialogueBox;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    TextMeshProUGUI dialogueSpeaker;
    [SerializeField,Range(1,20)]
    float textSpeed;

    private void Start()
    {
        em_l.AddListener<DialogueTrigger>(LevelEvents.ENTER_DIALOGUE_TRIGGER, ChangeDialogueTriggerReference);
        em_l.AddListener(LevelEvents.ENTER_DIALOGUE_TRIGGER, OpenDialogueBox);

        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
    }

    void OpenDialogueBox()
    {
        dialogueBox.SetActive(true);
    }

    void ChangeDialogueTriggerReference(DialogueTrigger d)
    {
        _currentD = d;
        _currentLine = _currentD.CurrentLine;
        OpenDialogueBox();
        PrintCurrentDialogueLine();
    }
    public void NextDialogueLine()
    {
        if (_currentD.NextDialogue())
        {
            dialogueBox.SetActive(false);
            return;
        }
        _currentLine = _currentD.CurrentLine;

        PrintCurrentDialogueLine();
    }

    public void PreviousDialogueLine()
    {
        _currentD.PreviousDialogue();
        _currentLine = _currentD.CurrentLine;
        PrintCurrentDialogueLine();
    }

    public void PrintCurrentDialogueLine()
    {
        StopAllCoroutines();
        StartCoroutine(PrintLine());
    }

    IEnumerator PrintLine()
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = _currentLine.Speaker.ToString();
        foreach (char c in _currentLine.Text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.5f/textSpeed);
        }
    }

}
