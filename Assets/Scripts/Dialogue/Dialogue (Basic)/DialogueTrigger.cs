using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    EventManager<LevelEvents> em_l = EventSystem.level;

    [SerializeField]
    DialogueLines lines;
    int lineCount;
    Line currentLine;

    int index;
    bool isTriggered;

    void Start()
    {
        index = 0;
        lineCount = lines.Lines.Length;
        currentLine = lines.Lines[index];
    }

    public bool NextDialogue()
    {
        //prevent exceeding array contents
        if (index + 1 >= lineCount)
        {
            return true;
        }
        index += 1;
        currentLine = lines.Lines[index];
        return false;
    }

    public void PreviousDialogue()
    {
        //prevent exceeding array contents
        if (index - 1 < 0)
        {
            return;
        }

        index -= 1;
        currentLine = lines.Lines[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            Debug.Log($"Entered Dialogue Trigger : {gameObject.name}");
            em_l.TriggerEvent<DialogueTrigger>(LevelEvents.ENTER_DIALOGUE_TRIGGER, this);
            isTriggered = true;
        }
    }

    public Line CurrentLine => currentLine;
}
