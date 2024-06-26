
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using PGGE.Patterns;
using Unity.Mathematics;

namespace Breathing
{
    [RequireComponent(typeof(MicrophoneController))]
    public class BreathingDetection : MonoBehaviour
    {

        public TextMeshProUGUI stateText;
        public TextMeshProUGUI dataProjector;

        private MicrophoneController micControl; //microphone controller for loudness and variance calculation

        private float prevLoudness = 0f; //loudness in previous frame
        private float variance = 0f; //variance of current frame
        private int varianceUnderThresholdCounter = 0; //counts how many frames the variance spent under threshold

        [Header("Hysteresis variables")]

        //exhaling
        [Tooltip("determines how loud the exhale needs to be to change state to Exhale")]
        public float exhaleLoudnessThresholdHigh = 0.5f;
        [Tooltip("detemines how loud the volumn to change from inhale to exhale")]
        public float exhaleLoudnessThresholdLow = 0.2f;  

        //inhaling
        [Tooltip("determines how silent should the exhale be to change state to Inhale")]
        public float inhaleLoudnessThresholdHigh = 0.1f; //determines how silent should the exhale be to change state to Inhale
        [Tooltip("determines how")]
        public float inhaleLoudnessThresholdLow = 0.05f; //determines how silent should the exhale be to change state to Inhale

        [HideInInspector]
        public float exhaleVarianceThreshold = 2; //determines how large should the variance be to change state to Exhale
        [HideInInspector]
        public float inhaleVarianceThreshold = -3; //determines how silent should the variance be to change state to Inhale


        public float pitchFrequencyThresholdLow = 1000; //if sound pitch is between these values it could be an exhale
        public float pitchFrequencyThresholdHigh = 2000;


        //Loudness minimization variables
        private List<float> loudnessList = new List<float>();
        private int maxLoudnessListCount = 10;
        [HideInInspector]
        public float minimizedLoudness = 999f;

        [SerializeField] private bool useMinimizedLoudness = true;
        //for deciding the states
        FSM fsm;

        public float Variance { get => variance; set => variance = value; }

        void Start()
        {
            micControl = this.GetComponent<MicrophoneController>();
            if (micControl == null)
            {
                Debug.LogError("Cannot find MicrophoneController attached to this object.");
            }
            fsm = new FSM();
            fsm.Add(new InhalingState(fsm, (int)Breathing.Inhale, this,micControl));
            fsm.Add(new ExhalingState(fsm, (int)Breathing.Exhale, this, micControl));
            fsm.SetCurrentState((int)Breathing.Inhale);
        }

        void FixedUpdate()
        {
            updateVariance();

            minimizeLoudness();

            fsm.FixedUpdate();
            ProjectText();

        }

        private void ProjectText()
        {
            Debug.Log($"current loudness now {minimizedLoudness}");
            Debug.Log($"current variance {variance}");
            if (stateText != null)
            {
                stateText.text = "Current state: " + fsm.GetCurrentState().ToString();
            }
            if(dataProjector != null)
            {
                dataProjector.text = $"Data \n" +
                    $"Pitch: {(int)micControl.getPitch()} \n" +
                    $"min Loudness: {(half)minimizedLoudness} \n" +
                    $"Varance: {(half)variance}";
                    
            }
        }


        #region old code
        /*
         * This function checks if all the criteria to transition from Inhale state to Exhale state have been met and then transitions to Exhale state
         * 
         * Criteria:
         * Microphone loudness and variance have to be higher than our thresholds.
         * OR 
         * Microphone loudness has to be very loud and variance has to be under threshold for the last X frames
         * 
         */

        //void checkIfExhaling()
        //{

        //    if (currentState == Breathing.Inhale)
        //    {
        //        //if (minimizedLoudness > exhaleLoudnessThresholdLow
        //        //    && 
        //        //	//check if the pitch is within a certain threshold.
        //        //	(micControl.getPitch() > pitchFrequencyThresholdLow &&
        //        //	micControl.getPitch() < pitchFrequencyThresholdHigh)) 
        //        //{
        //        //	currentState = Breathing.Exhale; //Change state to exhaling
        //        //	BreathingEvents.TriggerOnExhale (); //Trigger onExhale event
        //        //}
        //        print($"current loudness now {minimizedLoudness}");
        //        bool isLoudLow = minimizedLoudness > exhaleLoudnessThresholdLow;
        //        bool isRightPitch = (micControl.getPitch() > pitchFrequencyThresholdLow && micControl.getPitch() < pitchFrequencyThresholdHigh);
        //        print($"Exhale condition. Loud: {isLoudLow} pitch: {isRightPitch}");// current pitch {micControl.getPitch()}

        //        if (minimizedLoudness > exhaleLoudnessThresholdLow
        //            &&
        //            //check if the pitch is within a certain threshold.
        //            (micControl.getPitch() > pitchFrequencyThresholdLow &&
        //            micControl.getPitch() < pitchFrequencyThresholdHigh))
        //        {
        //            currentState = Breathing.Exhale; //Change state to exhaling
        //            //BreathingEvents.TriggerOnExhale(); //Trigger onExhale event
        //        }
        //    }
        //}


        /*
         * This function checks if all the criteria to transition from Exhale state to Inhale state have been met and then transitions to Inhale state
         * 
         * Criteria:
         * Microphone loudness and variance have to be lower than our thresholds.
         * OR 
         * Microphone loudness has to be much lower than our inhale loudness threshold
         * 
         */

        //void checkIfInhaling()
        //{
        //    bool isRightVolumeHigh = minimizedLoudness < inhaleLoudnessThresholdHigh;
        //    bool isRightVolumeLow = minimizedLoudness < inhaleLoudnessThresholdLow;
        //    bool isRightVarance = variance < inhaleVarianceThreshold;
        //    print($"Inhale condition. LoudLow: {isRightVolumeLow} LoudHigh: {isRightVolumeHigh} Varance: {isRightVarance}");// current pitch {micControl.getPitch()}

        //    if (currentState == Breathing.Exhale &&

        //        ((minimizedLoudness < inhaleLoudnessThresholdHigh &&
        //        variance < inhaleVarianceThreshold) ||
        //        minimizedLoudness < inhaleLoudnessThresholdLow))
        //    {

        //        currentState = Breathing.Inhale; //Change state to inhaling			
        //        //BreathingEvents.TriggerOnInhale(); //Trigger onInhale event			
        //    }
        //}
        #endregion

        /// <summary>
        /// Get the difference of loudness between different
        /// frame. Store in a variable called variance.
        /// </summary>
        void updateVariance()
        {

            variance = micControl.loudness - prevLoudness;
            prevLoudness = micControl.loudness;

            //update variance counter
            if (variance < exhaleVarianceThreshold)
            {
                //means that this frame is not able to trigger exhale.
                varianceUnderThresholdCounter++;

            }
            else
            {
                varianceUnderThresholdCounter = 0;
            }

        }

        void minimizeLoudness()
        {

            if (useMinimizedLoudness)
            {
                loudnessList.Add(prevLoudness);

                if (prevLoudness <= minimizedLoudness)
                {
                    minimizedLoudness = prevLoudness;
                }

                //Remove oldest loudness from list and recalculate minimizedLoudness
                //(only if the oldest loudness is the currentMinimizedLoudness)
                if (loudnessList.Count >= maxLoudnessListCount)
                {
                    if (loudnessList[0] <= minimizedLoudness)
                    {
                        //Find new minimizedLoudness
                        float min = loudnessList[1];
                        for (int i = 1; i < loudnessList.Count; i++)
                        {
                            if (loudnessList[i] < min)
                            {
                                min = loudnessList[i];
                            }
                        }
                        minimizedLoudness = min;

                    }
                    loudnessList.RemoveAt(0);

                }
            }
            else
            {
                minimizedLoudness = micControl.loudness;
                //Debug.Log(minimizedLoudness);

            }

            /*string dLog = "";
            for (int i = 0; i < loudnessList.Count; i++) {
                dLog += loudnessList[i] + ", ";
            }
            Debug.Log (dLog);
            Debug.Log("MinimizedLoudness: " + minimizedLoudness + " Past loudness: " + prevLoudness);
            */
        }
    }
}