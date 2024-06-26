using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Breathing
{

    public enum Breathing { Inhale, Exhale, Others }; //Two possible breathing states 

    public class BreathingState : FSMState
    {
        protected BreathingDetection detector;
        protected MicrophoneController microphoneController;

        public BreathingState(FSM fsm, int id , 
            BreathingDetection detector,
            MicrophoneController controller) : base(fsm, id)
        {
            this.detector = detector;
            microphoneController = controller;
        }
    }

    /*
         * This function checks if all the criteria to transition from Exhale state to Inhale state have been met and then transitions to Inhale state
         * 
         * Criteria:
         * Microphone loudness and variance have to be lower than our thresholds.
         * OR 
         * Microphone loudness has to be much lower than our inhale loudness threshold
         * 
         */
    public class InhalingState : BreathingState
    {
        public InhalingState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {
            float pitch = microphoneController.getPitch();
            bool isVolumeExhale = detector.minimizedLoudness > detector.exhaleLoudnessThresholdLow;
            bool isWithinExhalePitchRange = (pitch > detector.pitchFrequencyThresholdLow && 
                pitch < detector.pitchFrequencyThresholdHigh);

            Debug.Log($"Exhale condition. Loud: {isVolumeExhale} pitch: {isWithinExhalePitchRange}");// current pitch {micControl.getPitch()}

            if (isVolumeExhale
                &&
                isWithinExhalePitchRange)
            {
                mFsm.SetCurrentState((int)Breathing.Exhale); //Change state to exhaling
            }

        }
    }


    /*
        * This function checks if all the criteria to transition from Inhale state to Exhale state have been met and then transitions to Exhale state
        * 
        * Criteria:
        * Microphone loudness and variance have to be higher than our thresholds.
        * OR 
        * Microphone loudness has to be very loud and variance has to be under threshold for the last X frames
        * 
        */
    public class ExhalingState : BreathingState
    {
        public ExhalingState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {
            bool isInhaleVolume =detector.minimizedLoudness < detector.inhaleLoudnessThresholdHigh;
            bool isTooQuiet = detector.minimizedLoudness < detector.inhaleLoudnessThresholdLow;
            bool hasLargeVolumeDifference = detector.Variance < detector.inhaleVarianceThreshold;
            Debug.Log($"Inhale condition. LoudLow: {isTooQuiet} LoudHigh: {isInhaleVolume} Varance: {hasLargeVolumeDifference}");// current pitch {micControl.getPitch()}

            if (
                ((isInhaleVolume &&
                hasLargeVolumeDifference) ||
                isTooQuiet))
            {
                mFsm.SetCurrentState((int)Breathing.Inhale);			
            }
        }
    }
}