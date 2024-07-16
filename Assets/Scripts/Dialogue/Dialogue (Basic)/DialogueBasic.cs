using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.HelpfulScripts;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueBasic : MonoBehaviour
{
    EventManager<LevelEvents> em_l = EventSystem.level;

    [SerializeField]
    GameObject dialogueBox;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    TextMeshProUGUI dialogueSpeaker;
    [SerializeField]
    DialogueLine[] Lines;
    [SerializeField]
    float textSpeed;

    int index;
    bool isTriggered;
    string currentLine;

    [SerializeField]ControllerManager controllerManager;

    void Start()
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index = 0;
        currentLine = Lines[index].Line;
    }

    void InitializeDialogue()
    {
        dialogueBox.SetActive(true);
        isTriggered = true;
        PrintCurrentDialogueLine();
    }

    public void NextDialogue()
    {
        //prevent exceeding array contents
        if(index + 1 >= Lines.Length)
        {
            dialogueBox.SetActive(false);
            return;
        }
        index += 1;
        currentLine = Lines[index].Line;
    }

    public void PreviousDialogue()
    {
        //prevent exceeding array contents
        if (index - 1 < 0)
        {
            return;
        }

        index -= 1;
        currentLine = Lines[index].Line;
    }

    public void PrintCurrentDialogueLine() 
    {
        StopAllCoroutines();
        StartCoroutine(PrintLine());
    }

    IEnumerator PrintLine()
    {
        dialogueSpeaker.text = Lines[index].Speaker.ToString();
        foreach (char c in currentLine.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            //dialogueBox.SetActive(true);
            //isTriggered = true;
            //controllerManager.dict[Controls.AButton].started += DoDialogue;
            //controllerManager.dict[Controls.BButton].started += PreviousDialogue;
            Debug.Log($"Entered Dialogue Trigger : {gameObject.name}");
            em_l.TriggerEvent<DialogueBasic>(LevelEvents.ENTER_DIALOGUE_TRIGGER,this);
            InitializeDialogue();
        }
    }
}
