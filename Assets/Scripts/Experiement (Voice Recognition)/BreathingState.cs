using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Breathing
{

    public enum Breathing 
    { 
        TESTING_INHALE,
        TESTING_EXHALE,
        TESTING_SILENT,
        INHALE, 
        EXHALE, 
        SILENT,
        
    }; 

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


        /* 
        * This function checks if all the criteria to transition from INHALE state to EXHALE state have been met and then transitions to EXHALE state
        * 
        * Criteria:
        * Microphone loudness and variance have to be higher than our thresholds.
        * OR 
        * Microphone loudness has to be very loud and variance has to be under threshold for the last X frames
        * 
        */
        protected void CheckInhale()
        {
            bool isInhaleVolume = detector.minimizedLoudness < detector.inhaleLoudnessThresholdHigh;
            //bool isTooQuiet = detector.minimizedLoudness < detector.inhaleLoudnessThresholdLow;
            bool hasLargeVolumeDifference = true; /*detector.Variance < detector.inhaleVarianceThreshold;*/
            bool withinInhalePitch = (microphoneController.getPitch() > detector.inhalePitchFrequencyThresholdLow &&
                microphoneController.getPitch() < detector.inhalePitchFrequencyThresholdHigh);

            Debug.Log($"Inhale condition. withinpitch {withinInhalePitch} LoudHigh: {isInhaleVolume} Varance: {hasLargeVolumeDifference}");// current pitch {micControl.getPitch()}

            if (isInhaleVolume &&
                hasLargeVolumeDifference &&
                withinInhalePitch
                )
            {
                mFsm.SetCurrentState((int)Breathing.INHALE);
            }
        }


        /*
        * This function checks if all the criteria to transition from EXHALE state to INHALE state have been met and then transitions to INHALE state
        * 
        * Criteria:
        * Microphone loudness and variance have to be lower than our thresholds.
        * OR 
        * Microphone loudness has to be much lower than our inhale loudness threshold
        * 
        */
        protected void CheckExhale()
        {
            float pitch = microphoneController.getPitch();
            bool isVolumeExhale = detector.minimizedLoudness > detector.exhaleLoudnessThresholdLow;
            bool isWithinExhalePitchRange = (pitch > detector.exhalePitchFrequencyThresholdLow &&
                pitch < detector.exhalePitchFrequencyThresholdHigh);

            Debug.Log($"Exhale condition. Loud: {isVolumeExhale} pitch: {isWithinExhalePitchRange}");// current pitch {micControl.getPitch()}

            if (isVolumeExhale
                &&
                isWithinExhalePitchRange)
            {
                mFsm.SetCurrentState((int)Breathing.EXHALE); //Change state to exhaling
            }
        }

        protected void CheckSilent()
        {
            bool isSilent = detector.minimizedLoudness < detector.silentVolumeThreshold;
            bool hasLowPitch = microphoneController.getPitch() < detector.silentPitchThresholdHigh && 
                                microphoneController.getPitch() > detector.silentPitchThresholdLow;
            bool hasLowVariance = detector.Variance < detector.silentVarianceThreshold;
            if(isSilent && hasLowPitch &&
                hasLowVariance)
            {
                mFsm.SetCurrentState((int)Breathing.SILENT);
            }
        }
    }
   
    public class InhalingState : BreathingState
    {
        public InhalingState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {
            CheckExhale();
            CheckSilent();
        }
    }
    
    public class ExhalingState : BreathingState
    {
        public ExhalingState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {
            CheckInhale();
            CheckSilent() ;
        }
    }

    public class SilentState : BreathingState
    {
        //can only transition to inhale
        public SilentState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {
            CheckInhale();
            CheckExhale();

        }
    }

    #region testing

    public class TestingSilent : BreathingState
    {
        float elapseTime;
        public TestingSilent(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
            elapseTime = 0;
        }


        public override void Enter()
        {
            microphoneController.StartRecording((int)detector.testingTimer);
        }

        public override void Update()
        {
            while(elapseTime < detector.testingTimer)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            mFsm.SetCurrentState((int)Breathing.TESTING_INHALE);
        }

        public override void Exit()
        {
           var slientClip = microphoneController.StopRecording();
            microphoneController.TestClipFromRecord(slientClip);
            if (slientClip != null)
            {
                float minCommonPitch;
                float maxCommonPitch;
                microphoneController.CalculateRangePitch(slientClip,out minCommonPitch, out maxCommonPitch);

                //test this first
                detector.silentPitchThresholdHigh = maxCommonPitch;
                detector.silentPitchThresholdLow = minCommonPitch;

                detector.silentVolumeThreshold = microphoneController.CalculateVolume(slientClip);
            }
            else
            {
                Debug.Log("ERROR!");
            }
        }

        #region old code
        //public override void Update()
        //{
        //    elapseTime += Time.deltaTime;
        //}

        //public override void FixedUpdate()
        //{
        //    while(elapseTime < detector.testingTimer)
        //    {
        //        float pitch = microphoneController.getPitch();
        //        float volume = detector.minimizedLoudness;
        //        float varance = detector.Variance;
        //        if(volume > volumeThreshold)
        //        {
        //            volumeThreshold = volume;
        //        }

        //        if(pitch < minPitch)
        //        {
        //            minPitch = pitch;
        //        }
        //        if(pitch > maxPitch)
        //        {
        //            maxPitch = pitch;
        //        }

        //        if(varance > maxVariance)
        //        {
        //            maxVariance = varance;
        //        }

        //        return;
        //    }
        //    mFsm.SetCurrentState((int)Breathing.TESTING_INHALE);
        //}

        //public override void Exit()
        //{
        //    detector.silentPitchThresholdLow = minPitch;
        //    detector.silentPitchThresholdHigh = maxPitch;
        //    detector.silentVarianceThreshold = maxVariance;
        //    detector.silentVolumeThreshold = volumeThreshold;
        //}
        #endregion
    }

    public class TestingInhaling : BreathingState
    {
        float elapseTime = 0f;
        //float minInhalePitchThreshold = float.PositiveInfinity;
        //float maxInhalePitchThreshold = float.NegativeInfinity;
        //float maxVolumeThreshold = float.NegativeInfinity;
        public TestingInhaling(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void Enter()
        {
            microphoneController.StartRecording((int)detector.testingTimer);
        }

        public override void Update()
        {
            while (elapseTime < detector.testingTimer)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            mFsm.SetCurrentState((int)Breathing.TESTING_EXHALE);
        }

        public override void Exit()
        {
            var inhaleClip = microphoneController.StopRecording();
            microphoneController.TestClipFromRecord(inhaleClip);
            if (inhaleClip != null)
            {
                float minCommonPitch;
                float maxCommonPitch;
                microphoneController.CalculateRangePitch(inhaleClip, out minCommonPitch, out maxCommonPitch);

                //test this first
                detector.inhalePitchFrequencyThresholdLow = minCommonPitch;
                detector.inhalePitchFrequencyThresholdHigh = maxCommonPitch;

                detector.inhaleLoudnessThresholdHigh = microphoneController.CalculateVolume(inhaleClip);
            }
            else
            {
                Debug.Log("ERROR!");
            }
        }

        #region
        //public override void Update()
        //{
        //    elapseTime += Time.deltaTime;
        //}

        //public override void FixedUpdate()
        //{
        //    while(elapseTime < detector.testingTimer)
        //    {
        //        float pitch = microphoneController.getPitch();
        //        float volume = detector.minimizedLoudness;
        //        //float varance = detector.Variance;

        //        if(pitch <= detector.silentPitchThresholdHigh || volume <= detector.silentVolumeThreshold)
        //        {
        //            //dont record as they are not prepared to do the inhaling
        //            return;
        //        }

        //        if (volume > maxVolumeThreshold )
        //        {
        //            maxVolumeThreshold = volume;
        //        }

        //        if (pitch < minInhalePitchThreshold)
        //        {
        //            minInhalePitchThreshold = pitch;
        //        }
        //        if(pitch > maxInhalePitchThreshold)
        //        {
        //            maxInhalePitchThreshold = pitch;
        //        }

        //        return;
        //    }
        //    mFsm.SetCurrentState((int)Breathing.TESTING_EXHALE);

        //    //todo change to exhaling testing
        //}


        //public override void Exit()
        //{
        //    detector.inhaleLoudnessThresholdHigh = maxVolumeThreshold;
        //    detector.inhalePitchFrequencyThresholdLow = minInhalePitchThreshold;
        //    detector.inhalePitchFrequencyThresholdHigh = maxInhalePitchThreshold;
        //}
        #endregion
    }

    public class TestingExhaling : BreathingState
    {
        //float minPitch;
        //float maxPitch;
        //float minVolumeThreshold;
        float elapseTime;
        public TestingExhaling(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
            //minPitch = float.PositiveInfinity;
            //maxPitch = float.NegativeInfinity;
            elapseTime = 0;
            //minVolumeThreshold = float.PositiveInfinity;
        }


        public override void Enter()
        {
            microphoneController.StartRecording((int)detector.testingTimer);
        }

        public override void Update()
        {
            while (elapseTime < detector.testingTimer)
            {
                elapseTime += Time.deltaTime;
                return;
            }

            mFsm.SetCurrentState((int)Breathing.SILENT);
        }

        public override void Exit()
        {
            var exhale = microphoneController.StopRecording();
            microphoneController.TestClipFromRecord(exhale);
            if (exhale != null)
            {
                float minCommonPitch;
                float maxCommonPitch;
                microphoneController.CalculateRangePitch(exhale, out minCommonPitch, out maxCommonPitch);

                //test this first
                detector.exhaleLoudnessThresholdLow = minCommonPitch;
                detector.exhaleLoudnessThresholdHigh = maxCommonPitch;

                detector.exhaleLoudnessThresholdLow = microphoneController.CalculateVolume(exhale);
            }
            else
            {
                Debug.Log("ERROR!");
            }
        }

        #region old code
        //public override void Update()
        //{
        //    elapseTime += Time.deltaTime;
        //}

        //public override void FixedUpdate()
        //{
        //    while (elapseTime < detector.testingTimer)
        //    {
        //        float pitch = microphoneController.getPitch();
        //        float volume = detector.minimizedLoudness;


        //        if (pitch <= detector.silentPitchThresholdHigh || volume <= detector.silentVolumeThreshold)
        //        {
        //            //dont record as they are not prepared to do the exhaling
        //            Debug.Log("not recording exhale");
        //            return;
        //        }

        //        if (volume < minVolumeThreshold)
        //        {
        //            minVolumeThreshold = volume;
        //        }

        //        if (pitch < minPitch)
        //        {
        //            minPitch = pitch;
        //        }
        //        if (pitch > maxPitch)
        //        {
        //            maxPitch = pitch;
        //        }

        //        return;
        //    }
        //    mFsm.SetCurrentState((int)Breathing.SILENT);
        //}

        //public override void Exit()
        //{
        //    detector.exhalePitchFrequencyThresholdLow = minPitch;
        //    detector.exhalePitchFrequencyThresholdHigh = maxPitch;
        //    detector.exhaleLoudnessThresholdLow = minVolumeThreshold;

        //}
        #endregion
    }

    #endregion
}