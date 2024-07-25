using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Dialog
{
    public class DialogueTrigger : MonoBehaviour
    {
        EventManager<DialogEvents> em_l = EventSystem.dialog;

        [SerializeField]
        DialogueLines lines;

        DialogueLines runtime_Lines;

        private void Start()
        {
            runtime_Lines = ScriptableObject.Instantiate(lines);
        }

        private void OnTriggerEnter(Collider other)
        {
            em_l.TriggerEvent<DialogueLines>(DialogEvents.ADD_DIALOG, runtime_Lines);
        }
    }
}