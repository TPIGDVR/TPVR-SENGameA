using System;
using UnityEngine;

namespace GrabInteraction.New_scripts
{
    public class VisorDisplay : MonoBehaviour, IScriptLoadQueuer
    {
        [SerializeField] PlayerEvents showEvent;
        [SerializeField] PlayerEvents hideEvent;


        private void Awake()
        {
            ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.PLAYER);
        }

        public void Initialize()
        {
            EventSystem.player.AddListener(showEvent, ShowVisor);
            EventSystem.player.AddListener(hideEvent, HideVisor);
            gameObject.SetActive(false);
        }

        void ShowVisor()
        {
            gameObject.SetActive(true);
        }

        void HideVisor()
        {
            gameObject.SetActive(false);
        }
    }
}