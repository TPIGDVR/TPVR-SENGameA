using UnityEngine;

namespace GrabInteraction.New_scripts
{
    public class VisorDisplay : MonoBehaviour, IScriptLoadQueuer
    {
        PlayerEvents showEvent;
        PlayerEvents hideEvent;


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