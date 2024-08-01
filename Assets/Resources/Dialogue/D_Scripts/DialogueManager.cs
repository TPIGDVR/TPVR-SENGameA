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
    public class DialogueManager : MonoBehaviour , IScriptLoadQueuer
    {
        EventManager<DialogEvents> em_l = EventSystem.dialog;

        //dialog information
        Queue<DialogueLines> _queueDialog = new();
        List<DialogueLines> listOfFinishDialog = new(); 
        DialogueLines currentDialog;
        Line currentLine;
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

        Coroutine printingCoroutine = null;
        AudioSource CurrentAudioSource = null;
        private void OnEnable()
        {
            em_l.AddListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);
        }

        private void OnDisable()
        {
            em_l.RemoveListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);
        }

        private void Awake()
        {
            ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.SYSTEM);
        }

        #region opening/closing the dialogue box
        void OpenDialogueBox()
        {
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.START_DIALOG);
            dialogueBox.SetActive(true);
        }
        void HideDialogueBox()
        {
            dialogAnimator.SetTrigger("Close");
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.END_DIALOG);
            StopAllCoroutines();
            StartCoroutine(closeDialog());


            IEnumerator closeDialog()
            {
                int hash = Animator.StringToHash("Exit state");
                while (dialogAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != hash)
                {
                    yield return null;
                }
                dialogueBox.SetActive(false);
                dialogueText.text = string.Empty;
                dialogueSpeaker.text = string.Empty;
            }
        }
        #endregion

        #region printing
        public void PrintCurrentDialogueLine()
        {
            StopAllCoroutines();
            //em_l.TriggerEvent(DialogEvents.NEXT_LINE);
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.NEXT_LINE);

            printingCoroutine = StartCoroutine(PrintLine(currentLine));
        }

        void InstantPrintLine(Line l)
        {
            dialogueText.text = l.Text;
            dialogueSpeaker.text = l.Speaker.ToString();
            TriggerDialogueEvents(l);
        }

        IEnumerator PrintLine(Line l)
        {
            if(l.audioClip.clip != null)
            {//check if there is a valid clip to play.
                CurrentAudioSource = SoundManager.Instance.PlayMusicClip(l.audioClip);
            }

            dialogueText.text = string.Empty;
            dialogueSpeaker.text = l.Speaker.ToString();
            foreach (char c in l.Text.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(0.5f / textSpeed);
            }

            TriggerDialogueEvents(l);
            printingCoroutine = null;
        }
        #endregion

        void AddDialog(DialogueLines line)
        {
            //if dialogue encountered alrdy, dont show agn
            if (listOfFinishDialog.Contains(line)) return;

            //add to list of encountered dialogues
            listOfFinishDialog.Add(line);
            //set it into queue to be displayed
            _queueDialog.Enqueue(line);

            //if currently no dialogue
            if (currentDialog == null)
            {
                currentDialog = _queueDialog.Dequeue();
                OpenDialog();
            }
        }

        void OpenDialog()
        {
            if (currentDialog == null) throw new Exception("Cant have empty dialog!!!");

            //open up the dialogue box ui
            OpenDialogueBox();

            //get the first line of text
            currentIndex = 0;
            currentLine = currentDialog.Lines[currentIndex];

            //print it
            PrintCurrentDialogueLine();
        }

        //called by controller button
        public void EvaluateAction_BtnA()
        {
            //check if theres valid dialogue
            if (currentDialog == null) return;

            //Retrieve the audiosource that was playing
            if (CurrentAudioSource != null) SoundManager.Instance.RetrieveAudioSource(CurrentAudioSource);

            //complete print dialogue before advancing to the next
            if (printingCoroutine != null)
            {
                StopCoroutine(printingCoroutine);
                //set as null
                printingCoroutine = null;

                InstantPrintLine(currentLine);

                
            }
            else //not currently printing anymore
            {
                //proceed to next line
                currentIndex++;

                //check if the line is the end
                if (currentIndex >= currentDialog.Lines.Length)
                {
                    EndDialog();
                }
                else
                {
                    currentLine = currentDialog.Lines[currentIndex];
                    PrintCurrentDialogueLine();
                }
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
            currentLine = currentDialog.Lines[currentIndex];
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
                currentDialog = null;
                HideDialogueBox();
            }
        }

        void TriggerDialogueEvents(Line line)
        {
            if (!line.hasBeenTriggered)
            {
                DialogEvents evnt = line.dialogEndTrigger;
                EventSystem.dialog.TriggerEvent(evnt);
                line.hasBeenTriggered = true;

                Debug.Log($"Triggered Event : {evnt}");
            }
        }

        public void Initialize()
        {
            dialogueBox.SetActive(false);
        }
    }
}