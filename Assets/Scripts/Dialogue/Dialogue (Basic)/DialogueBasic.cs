using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBasic : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    TextMeshProUGUI dialogueSpeaker;
    [SerializeField]
    DialogueLine[] Lines;
    [SerializeField]
    float textSpeed;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index = -1;
    }


    public void DoDialogue()
    {
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;
        index += 1;

        if (index >= Lines.Length)
        {
            this.gameObject.SetActive(false);
        }


        StartCoroutine(PrintDialogue());
    }

    public void PreviousDialogue()
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
}
