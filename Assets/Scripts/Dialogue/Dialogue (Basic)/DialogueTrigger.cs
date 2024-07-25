using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static ScriptableObjectManager;
namespace Dialog
{
    public class DialogueTrigger : MonoBehaviour
    {
        EventManager<DialogEvents> em_l = EventSystem.dialog;

        [SerializeField]
        DialogueLines lines;

        private void Start()
        {
            //create instance of the dialogue scriptable object
            AddIntoSOCollection(lines);
        }

        private void OnTriggerEnter(Collider other)
        {
            em_l.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, (DialogueLines)RetrieveRuntimeScriptableObject(lines));
        }
    }
}