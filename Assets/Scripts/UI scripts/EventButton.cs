using Patterns;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI_scripts
{
    public class EventButton : CustomButton
    {
        //can only trigger event without any values
        [Header("event")]
        [SerializeField] Event eventToCall;

        private void OnEnable()
        {
            actions.AddListener(CallEvent);
        }

        private void OnDisable()
        {
            actions.RemoveListener(CallEvent);
        }

        private void CallEvent()
        {
            
            //EventManager<>.Instance.TriggerEvent(eventToCall);
        }
    }
}