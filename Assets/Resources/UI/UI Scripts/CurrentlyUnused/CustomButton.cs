using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_scripts
{
    //this is just an add on to add music for buttons
    public class CustomButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private SFXClip clipToPlayWhenClick;
        [SerializeField] private float delayInSeconds = 0.1f;
        public UnityEvent actions;

        //this is special case for the button for the connecting button.
        public void ClickSound()
        {
            AudioManager.Instance.Play(clipToPlayWhenClick);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ClickSound();
            StartCoroutine(PlayingButtonBeforeActions());
        }

        private IEnumerator PlayingButtonBeforeActions()
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);
            actions?.Invoke();
        }
    }


}