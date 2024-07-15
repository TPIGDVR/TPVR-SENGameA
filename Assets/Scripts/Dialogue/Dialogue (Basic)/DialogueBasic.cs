using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.HelpfulScripts;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueBasic : MonoBehaviour
{
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

    [SerializeField]ControllerManager controllerManager;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index = -1;
    }


    public void DoDialogue(InputAction.CallbackContext context)
    {
        print("Dialog");
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index += 1;

        if (index >= Lines.Length)
        {
            dialogueBox.SetActive(false);
            controllerManager.dict[Controls.AButton].started -= DoDialogue;
            controllerManager.dict[Controls.BButton].started -= PreviousDialogue;
        }


        StartCoroutine(PrintDialogue());
    }

    public void PreviousDialogue(InputAction.CallbackContext context)
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index -= 1;
        StartCoroutine(PrintDialogue());
    }

    IEnumerator PrintDialogue()
    {
        foreach (char c in Lines[index].Line.ToCharArray())
        {
            dialogueSpeaker.text = Lines[index].Speaker.ToString();
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            dialogueBox.SetActive(true);
            isTriggered = true;
            controllerManager.dict[Controls.AButton].started += DoDialogue;
            controllerManager.dict[Controls.BButton].started += PreviousDialogue;
        }
    }
}
