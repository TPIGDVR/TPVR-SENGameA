using SoundRelated;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialog
{

    /// <summary>
    /// 
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        EventManager<DialogEvents> em_l = EventSystem.dialog;

        //dialog information
        Queue<DialogueLines> _queueDialog = new();
        List<DialogueLines> listOfFinishDialog = new(); 
        DialogueLines currentDialog;
        int currentIndex;


        [Header("Dialog Component")]
        [SerializeField]
        GameObject dialogueBox;
        [SerializeField]
        TextMeshProUGUI dialogueText;
        [SerializeField]
        TextMeshProUGUI dialogueSpeaker;
        [SerializeField] 
        Animator dialogAnimator;
        [SerializeField, Range(1, 20)]
        float textSpeed;

        private void OnEnable()
        {
            em_l.AddListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);

        }

        private void OnDisable()
        {
            em_l.RemoveListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);
        }

        private void Start()
        {
            dialogueBox.SetActive(false);

        }

        void OpenDialogueBox()
        {
            SoundManager.Instance.PlayAudio(SoundRelated.SFXClip.START_DIALOG);
            dialogueBox.SetActive(true);
        }
        void HideDialogueBox()
        {
            dialogAnimator.SetTrigger("Close");
            SoundManager.Instance.PlayAudio(SoundRelated.SFXClip.END_DIALOG);
            StopAllCoroutines();
            StartCoroutine(closeDialog());


            IEnumerator closeDialog()
            {
                int hash = Animator.StringToHash("Exit state");
                while (dialogAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != hash)
                {
                    yield return null;
                }
                print("finish");
                dialogueBox.SetActive(false);
                dialogueText.text = string.Empty;
                dialogueSpeaker.text = string.Empty;
            }
        }

        //public void NextDialogueLine()
        //{
        //    if (_currentD.NextDialogue())
        //    {
        //        dialogueBox.SetActive(false);
        //        return;
        //    }
        //    _currentLine = _currentD.CurrentLine;

        //    PrintCurrentDialogueLine();
        //}

        //public void PreviousDialogueLine()
        //{
        //    _currentD.PreviousDialogue();
        //    _currentLine = _currentD.CurrentLine;
        //    PrintCurrentDialogueLine();
        //}

        #region printing
        public void PrintCurrentDialogueLine()
        {
            StopAllCoroutines();
            //em_l.TriggerEvent(DialogEvents.NEXT_LINE);
            SoundManager.Instance.PlayAudio(SoundRelated.SFXClip.NEXT_LINE);

            StartCoroutine(PrintLine());
        }

        IEnumerator PrintLine()
        {
            var line = currentDialog.Lines[currentIndex];
            dialogueText.text = string.Empty;
            dialogueSpeaker.text = line.Speaker.ToString();
            foreach (char c in line.Text.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(0.5f / textSpeed);
            }
        }
        #endregion

        void AddDialog(DialogueLines line)
        {
            if (listOfFinishDialog.Contains(line)) return;

            listOfFinishDialog.Add(line);
            _queueDialog.Enqueue(line);
            if (currentDialog == null)
            {
                currentDialog = _queueDialog.Dequeue();
                //em_l.TriggerEvent(DialogEvents.START_DIALOG);
                OpenDialog();
            }
        }

        void OpenDialog()
        {
            if (currentDialog == null) throw new Exception("Cant have empty dialog!!!");
            OpenDialogueBox();
            currentIndex = 0;
            PrintCurrentDialogueLine();
        }

        public void NextLine()
        {
            if (currentDialog == null) return;
            currentIndex++;
            if (currentIndex >= currentDialog.Lines.Length)
            {
                //end the Dialog;
                EndDialog();
            }
            else
            {
                PrintCurrentDialogueLine();
            }
        }

        public void PreviousLine()
        {
            if (currentDialog == null) return;
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
                return;
            }
            PrintCurrentDialogueLine();
        }

        void EndDialog()
        {
            if (_queueDialog.Count > 0)
            {
                currentDialog = _queueDialog.Dequeue();
                OpenDialog();
            }
            else
            {
                print("ending dialog");
                currentDialog = null;
                HideDialogueBox();
            }
        }


    }
}