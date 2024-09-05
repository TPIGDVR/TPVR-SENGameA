using SoundRelated;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Dialog
{
    public class DialogueManager : MonoBehaviour, IScriptLoadQueuer
    {
        EventManager<DialogEvents> em_d = EventSystem.dialog;

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

        //Hash int
        int exitHash = Animator.StringToHash("Exit state");

        public TMP_Text debugtext;
        private void OnDisable()
        {
            em_d.RemoveListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);
        }

        private void Awake()
        {
            ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.SYSTEM);
        }

        public void Initialize()
        {
            dialogueBox.SetActive(false);
            em_d.AddListener<DialogueLines>(DialogEvents.ADD_DIALOG, AddDialog);
            //when the objective has been progress. stop eve from talking.
            EventSystem.level.AddListener<ObjectiveName>(LevelEvents.OBJECTIVE_PROGRESSED, InteruptDialog);
            EventSystem.player.AddListener(PlayerEvents.DEATH, OnDeath);
            EventSystem.player.AddListener(PlayerEvents.RES_SCREEN_FADED, OnResumeGame);

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
                while (dialogAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != exitHash)
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
            //em_d.TriggerEvent(DialogEvents.NEXT_LINE);
            SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.NEXT_LINE);

            printingCoroutine = StartCoroutine(PrintLine(currentLine));
        }

        /// <summary>
        /// Immediately Print the dialog if the player request it.
        /// </summary>
        /// <param name="l"></param>
        void InstantPrintLine(Line l)
        {
            dialogueText.text = l.Text;
            dialogueSpeaker.text = l.Speaker.ToString();
            TriggerDialogueEvents(l);
        }

        /// <summary>
        /// Print the dialog in the dialog panel
        /// </summary>
        /// <param name="l">The dialog dialog specified.</param>
        /// <returns></returns>
        IEnumerator PrintLine(Line l)
        {
            if (l.audioClip.clip != null)
            {//check if there is a valid transcript to play.
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

            //get the first dialog of stateText
            currentIndex = 0;
            currentLine = currentDialog.Lines[currentIndex];

            //print it
            PrintCurrentDialogueLine();
        }

        //called by controller button
        public void EvaluateAction_BtnA()
        {
            debugtext.text = "Btn A";
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
                //proceed to next dialog
                currentIndex++;

                //check if the dialog is the end
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

                //Debug.Log($"Triggered Event : {evnt}");
            }
        }

        void InteruptDialog(ObjectiveName objective)
        {
            if (dialogueBox.activeSelf)
            {
                if (printingCoroutine != null)
                {
                    StopCoroutine(printingCoroutine);
                    //set as null and clear all the dialog out.
                    printingCoroutine = null;
                }
                _queueDialog.Clear();
                currentDialog = null;
                HideDialogueBox();
            }
            
        }

        void OnDeath()
        {
            if (dialogueBox.activeSelf)
            {
                ////Means there is something on the dialog.
                //if (currentDialog)
                //{
                //    var newQueue = new Queue<DialogueLines>();
                //    newQueue.Enqueue(currentDialog);
                //    //add the remaining dialog to the new queue
                //    while(_queueDialog.Count > 0)
                //    {
                //        newQueue.Enqueue(_queueDialog.Dequeue());
                //    }
                //    //then change the reference of the queue.
                //    _queueDialog = newQueue;
                //    currentDialog = null;
                //}
                //since the index still remains the same. we can call it later
                HideDialogueBox();
            }
        }

        void OnResumeGame()
        {
            if(currentDialog != null)
            {
                //that means the before death, the player had dialog to run.
                OpenDialogueBox();
                currentLine = currentDialog.Lines[currentIndex];
                PrintCurrentDialogueLine();
            }
        }
    }
}