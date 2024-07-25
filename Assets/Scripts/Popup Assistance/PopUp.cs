using System.Collections;
using UnityEngine;

namespace PopUpAssistance
{
    public class PopUp : MonoBehaviour
    {
        
        [SerializeField] bool canPopUp = true;
        GameObject display;
        //add enum here to differentiat
        public bool CanPopUp { get => canPopUp; set => canPopUp = value; }
        public GameObject Display { get => display; set => display = value; }

        public bool hasDisplay => display != null;

        public void SetCanPopUp(bool canPopUp)
        {
            this.canPopUp= canPopUp;
        }

    }
}