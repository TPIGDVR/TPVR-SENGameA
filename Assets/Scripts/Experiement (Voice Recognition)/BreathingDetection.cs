
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using PGGE.Patterns;
using Unity.Mathematics;
using static Unity.VisualScripting.Member;

namespace Breathing
{
    [RequireComponent(typeof(MicrophoneController))]
    public class BreathingDetection : MonoBehaviour
    {
        [Header("Data Text")]
        public TextMeshProUGUI stateText;
        public TextMeshProUGUI dataProjector;

        protected MicrophoneController micControl; //microphone controller for loudness and variance calculation

        private float prevLoudness = 0f; //loudness in previous frame
        private float variance = 0f; //variance of current frame
        private int varianceUnderThresholdCounter = 0; //counts how many frames the variance spent under threshold

        [Header("Hysteresis variables")]

        //exhaling
        //[Tooltip("determines how loud the exhale needs to be to change state to Exhale")]
        //public float exhaleLoudnessThresholdHigh = 0.5f;
        [Tooltip("detemines how loud the volume to change from inhale to exhale")]
        public float exhaleLoudnessThreshold = 0.2f;  

        //inhaling
        [Tooltip("determines how silent should the volume be to change state to Inhale")]
        public float inhaleLoudnessThreshold = 0.1f; //determines how silent should the exhale be to change state to INHALE
        //[Tooltip("Err not too sure if this is still being used")]
        //public float inhaleLoudnessThresholdLow = 0.05f; //determines how silent should the exhale be to change state to INHALE

        [Header("Silent")]

        public float silentVolumeThreshold = 0.05f;

        public float silentPitchThresholdLow = 20f;
        public float silentPitchThresholdHigh = 40f;
        public float silentVarianceThreshold = 0.1f;
        public int silentVariancePitchThreshold = 3;

        [Tooltip("determines how large should the variance be to change state to EXHALE")]
        public float exhaleVarianceThreshold = 2;
        [Tooltip("Determines how consistent the pitch should be to change a state to inhale")]
        public int inhaleVariancePitchThreshold = 3;

        [Header("Exahle pitch")]
        public float exhalePitchFrequencyThresholdLow = 1000; //if sound pitch is between these values it could be an exhale
        public float exhalePitchFrequencyThresholdHigh = 2000;

        [Header("Inhale pitch")]
        [Tooltip("How low the pitch be to determine if it is a inhale")]
        public float inhalePitchFrequencyThresholdLow = 3000;
        [Tooltip("How high the pitch is to cut off any high pitch sounds that aren't recognise as inhaling")]
        public float inhalePitchFrequencyThresholdHigh = 5000;

        //Loudness minimization variables
        private List<float> loudnessList = new List<float>();
        private int maxLoudnessListCount = 10;
        [HideInInspector]
        public float calculateLoudness = 999f;

        //Pitch minimization variables
        private List<float> pitchList = new();
        private int maxPitchListCount = 10;
        [HideInInspector]
        public float calculatedPitch = float.PositiveInfinity;


        [Header("Other settings")]
        [SerializeField] private CalculationMethod loudnessCalculationMethod;
        [SerializeField] private CalculationMethod pitchCalculationMethod;
        [SerializeField] float samplingSize = 1000;
        public float testingTimer;
        //for deciding the states
        protected FSM fsm;

        public float Variance { get => variance; set => variance = value; }

        protected virtual void Start()
        {
            micControl = this.GetComponent<MicrophoneController>();
            if (micControl == null)
            {
                Debug.LogError("Cannot find MicrophoneController attached to this object.");

            }
            fsm = new FSM();
            SetUpFSM();
        }

        protected virtual void FixedUpdate()
        {
            UpdateVariance();
            CalculateLoudness();
            CalculatePitch();
            fsm.FixedUpdate();
            ProjectText();
        }

        private void ProjectText()
        {
            //Debug.Log($"current loudness now {calculateLoudness}");
            //Debug.Log($"current variance {variance}");
            if (stateText != null)
            {
                stateText.text = "Current state: " + fsm.GetCurrentState().ToString();
            }
            if(dataProjector != null)
            {
                dataProjector.text = $"Data \n" +
                    $"calculate Pitch: {(int)calculatedPitch} \n" +
                    $"calculate Loudness: {(half)calculateLoudness} \n" +
                    $"Varance: {(half)variance}";
            }
        }

        #region old code
        /*
         * This function checks if all the criteria to transition from INHALE state to EXHALE state have been met and then transitions to EXHALE state
         * 
         * Criteria:
         * Microphone loudness and variance have to be higher than our thresholds.
         * OR 
         * Microphone loudness has to be very loud and variance has to be under threshold for the last X frames
         * 
         */

        //void checkIfExhaling()
        //{

        //    if (currentState == Breathing.INHALE)
        //    {
        //        //if (calculateLoudness > exhaleLoudnessThreshold
        //        //    && 
        //        //	//check if the pitch is within a certain threshold.
        //        //	(micControl.getPitch() > exhalePitchFrequencyThresholdLow &&
        //        //	micControl.getPitch() < exhalePitchFrequencyThresholdHigh)) 
        //        //{
        //        //	currentState = Breathing.EXHALE; //Change state to exhaling
        //        //	BreathingEvents.TriggerOnExhale (); //Trigger onExhale event
        //        //}
        //        print($"current loudness now {calculateLoudness}");
        //        bool isLoudLow = calculateLoudness > exhaleLoudnessThreshold;
        //        bool isRightPitch = (micControl.getPitch() > exhalePitchFrequencyThresholdLow && micControl.getPitch() < exhalePitchFrequencyThresholdHigh);
        //        print($"EXHALE condition. Loud: {isLoudLow} pitch: {isRightPitch}");// current pitch {micControl.getPitch()}

        //        if (calculateLoudness > exhaleLoudnessThreshold
        //            &&
        //            //check if the pitch is within a certain threshold.
        //            (micControl.getPitch() > exhalePitchFrequencyThresholdLow &&
        //            micControl.getPitch() < exhalePitchFrequencyThresholdHigh))
        //        {
        //            currentState = Breathing.EXHALE; //Change state to exhaling
        //            //BreathingEvents.TriggerOnExhale(); //Trigger onExhale event
        //        }
        //    }
        //}


        /*
         * This function checks if all the criteria to transition from EXHALE state to INHALE state have been met and then transitions to INHALE state
         * 
         * Criteria:
         * Microphone loudness and variance have to be lower than our thresholds.
         * OR 
         * Microphone loudness has to be much lower than our inhale loudness threshold
         * 
         */

        //void checkIfInhaling()
        //{
        //    bool isRightVolumeHigh = calculateLoudness < inhaleLoudnessThreshold;
        //    bool isRightVolumeLow = calculateLoudness < inhaleLoudnessThresholdLow;
        //    bool isRightVarance = variance < inhaleVariancePitchThreshold;
        //    print($"INHALE condition. LoudLow: {isRightVolumeLow} LoudHigh: {isRightVolumeHigh} Varance: {isRightVarance}");// current pitch {micControl.getPitch()}

        //    if (currentState == Breathing.EXHALE &&

        //        ((calculateLoudness < inhaleLoudnessThreshold &&
        //        variance < inhaleVariancePitchThreshold) ||
        //        calculateLoudness < inhaleLoudnessThresholdLow))
        //    {

        //        currentState = Breathing.INHALE; //Change state to inhaling			
        //        //BreathingEvents.TriggerOnInhale(); //Trigger onInhale event			
        //    }
        //}
        #endregion

        protected virtual void SetUpFSM()
        {
            fsm.Add(new InhalingState(fsm, (int)Breathing.INHALE, this, micControl));
            fsm.Add(new ExhalingState(fsm, (int)Breathing.EXHALE, this, micControl));
            fsm.Add(new SilentState(fsm, (int)Breathing.SILENT, this, micControl)); 
            fsm.SetCurrentState((int)Breathing.INHALE);
        }
        /// <summary>
        /// Get the difference of loudness between different
        /// frame. Store in a variable called variance.
        /// </summary>
        void UpdateVariance()
        {

            variance = micControl.loudness - prevLoudness;
            prevLoudness = micControl.loudness;

            //update variance counter
            if (variance < exhaleVarianceThreshold)
            {
                varianceUnderThresholdCounter++;

            }
            else
            {
                varianceUnderThresholdCounter = 0;
            }

        }

        void CalculateLoudness()
        {

            switch (loudnessCalculationMethod)
            {
                case CalculationMethod.none:
                    calculateLoudness = micControl.loudness;
                    break;
                case CalculationMethod.Average:
                    CalculateAverageLoudness();
                    break;
                case CalculationMethod.Min:
                    CalculateMinLoudness();
                    break;
            }


            void CalculateAverageLoudness()
            {
                loudnessList.Add(prevLoudness);

                if (loudnessList.Count >= maxLoudnessListCount)
                {//remove the last one
                    loudnessList.RemoveAt(0);
                }

                float averageVal = 0;
                for (int i = 0; i < loudnessList.Count; i++)
                {
                    averageVal += loudnessList[i];
                }

                calculateLoudness = averageVal / loudnessList.Count;
            }

            void CalculateMinLoudness()
            {
                loudnessList.Add(prevLoudness);

                if (prevLoudness <= calculateLoudness)
                {
                    calculateLoudness = prevLoudness;
                }

                //Remove oldest loudness from list and recalculate calculateLoudness
                //(only if the oldest loudness is the currentMinimizedLoudness)
                if (loudnessList.Count >= maxLoudnessListCount)
                {
                    if (loudnessList[0] <= calculateLoudness)
                    {
                        //Find new calculateLoudness
                        float min = loudnessList[1];
                        for (int i = 1; i < loudnessList.Count; i++)
                        {
                            if (loudnessList[i] < min)
                            {
                                min = loudnessList[i];
                            }
                        }
                        calculateLoudness = min;

                    }
                    loudnessList.RemoveAt(0);

                }
            }

            /*string dLog = "";
            for (int i = 0; i < loudnessList.Count; i++) {
                dLog += loudnessList[i] + ", ";
            }
            Debug.Log (dLog);
            Debug.Log("MinimizedLoudness: " + calculateLoudness + " Past loudness: " + prevLoudness);
            */
        }

        void CalculatePitch()
        {
            switch (pitchCalculationMethod)
            {
                case CalculationMethod.none:
                    CalculateNormalPitch();
                    break;
                case CalculationMethod.Average:
                    CalculateAveragePitch();
                    break;
                case CalculationMethod.Min:
                    CalculateMinPitch();
                    break;
            }


            void CalculateAveragePitch()
            {
                float curPitch = micControl.getPitch();
                pitchList.Add(curPitch);

                if(pitchList.Count >= maxPitchListCount)
                {//remove the last one
                    pitchList.RemoveAt(0);
                }

                float averageVal = 0;
                for(int i = 0; i < pitchList.Count; i++)
                {
                    averageVal += pitchList[i];
                }

                calculatedPitch = averageVal / pitchList.Count;

            }

            void CalculateMinPitch()
            {
                float curPitch = micControl.getPitch();
                pitchList.Add(curPitch);

                if (curPitch <= calculatedPitch)
                {
                    calculatedPitch = curPitch;
                }


                if (pitchList.Count >= maxPitchListCount)
                {
                    if (pitchList[0] <= calculatedPitch)
                    {
                        //Find new minimizedpitch
                        float min = pitchList[1];
                        for (int i = 1; i < pitchList.Count; i++)
                        {
                            if (pitchList[i] < min)
                            {
                                min = pitchList[i];
                            }
                        }
                        calculatedPitch = min;

                    }
                    pitchList.RemoveAt(0);

                }
            }

            void CalculateNormalPitch()
            {
                calculatedPitch = micControl.getPitch();
            }
        }

        enum CalculationMethod
        {
            none,
            Average,
            Min
        }
    }
}