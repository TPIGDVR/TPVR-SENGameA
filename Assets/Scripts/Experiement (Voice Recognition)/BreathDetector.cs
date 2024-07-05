using Breathing3;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Experiement__Voice_Recognition_
{
    /// <summary>
    /// TODO:
    /// 1. Make sure that the amount of time they spend breathing correlate to reducing
    /// of anxiety
    /// 2. Link it up with the event manager breath.
    /// 3. reduce the anxiety if the player is not near a loud spot
    /// </summary>

    public class BreathDetector : MonoBehaviour 
    {
        OutputBreath outPutBreath;
        [Range(0.1f,1)]
        [SerializeField] float minAnxietyReduction = 0.2f;
        [SerializeField] float maxAnxietyReduction = 0.8f;
        [SerializeField] float maximumInhaleTimer = 3f;
        [SerializeField] float maximumExhaleTimer = 3f;
        [SerializeField] TextMeshProUGUI displayText;
        private int leniecyCounter = 0;
        [SerializeField] int leniecyThreshold;

        BreathingStates previousState = BreathingStates.SILENT;
        BreathingStates currentState = BreathingStates.SILENT;
        bool waitingForBreathOut = false;

        float inhaleElapseTime = 0f;
        float exhaleElapseTime = 0f;
        EventManager<Event> em = EventSystem.em;

        private void Start()
        {
            outPutBreath = GetComponent<OutputBreath>();
        }

        private void Update()
        {
            DecideBreathingState();
            displayText.text = $"Cur State: {currentState}\n" +
                $"prev State: {previousState}";
            CalculateInhaleElapseTime();
            CalculateExhaleElapseTime();
        }

        void DecideBreathingState()
        {
            BreathingStates state = outPutBreath.PlayerBreathState();
            if(state != currentState)
            {
                //ignore it 
                if(leniecyCounter < leniecyThreshold)
                {
                    leniecyCounter++;
                }
                else
                {
                    leniecyCounter = 0;
                    previousState = currentState;
                    currentState = state;
                }
            }
            else
            {
                leniecyCounter = 0;
            }

        }

        void CalculateInhaleElapseTime()
        {
            if(currentState == BreathingStates.INHALE &&
                !waitingForBreathOut)
            {
                //so if it is not waiting for a breath out then dont cout
                inhaleElapseTime += Time.deltaTime;
            }
            else if(previousState == BreathingStates.INHALE)
            {
                //if the inhale is pass, then wait for the player to breathe out.
                waitingForBreathOut = true;
            }
        }

        void CalculateExhaleElapseTime()
        {
            if(currentState == BreathingStates.EXHALE &&
                waitingForBreathOut)
            {
                exhaleElapseTime += Time.deltaTime;
            }
            else if(previousState == BreathingStates.EXHALE &&
                waitingForBreathOut)
            {
                CalculateAnxietyReduction();
            }
        }

        void CalculateAnxietyReduction()
        {
            inhaleElapseTime = Mathf.Clamp(inhaleElapseTime, 0, maximumInhaleTimer);
            exhaleElapseTime = Mathf.Clamp(exhaleElapseTime, 0, maximumExhaleTimer);

            float percentageAchieve = Mathf.InverseLerp(0 , 2, 
                inhaleElapseTime / maximumInhaleTimer +
                exhaleElapseTime / maximumExhaleTimer); 

            em.TriggerEvent<float>(Event.ANXIETY_BREATHE, percentageAchieve);

            //reset the elapse Time to be called again.
            inhaleElapseTime = 0f;
            exhaleElapseTime = 0f;
            //now wait for the player to breathin again.
            waitingForBreathOut = false;
        }
    }
}