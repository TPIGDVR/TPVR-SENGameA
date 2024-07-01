using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;

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
        protected BreathingDetection _detector;
        protected MicrophoneController _micController;

        public BreathingState(FSM fsm, int id , 
            BreathingDetection detector,
            MicrophoneController controller) : base(fsm, id)
        {
            this._detector = detector;
            _micController = controller;
        }


        //counter


        //private void ResetChecker()
        //{
        //    inhaleVarianceChecker = 0;
        //}

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
            float pitch = _detector.calculatedPitch;

            bool isInhaleVolume = _detector.calculateLoudness > _detector.inhaleLoudnessThreshold;
            //bool hasConsistentPitch = inhaleVarianceChecker >= _detector.inhaleVariancePitchThreshold;
            bool withinInhalePitch = (pitch > _detector.inhalePitchFrequencyThresholdLow &&
                pitch < _detector.inhalePitchFrequencyThresholdHigh);

            //Debug.Log($"Inhale condition. \n" +
            //    $"withinpitch {withinInhalePitch}\n" +
            //    $" LoudHigh: {isInhaleVolume} \n");// current pitch {micControl.getPitch()}

            if (isInhaleVolume &&
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
            //float pitch = _micController.getPitch();
            float pitch = _detector.calculatedPitch;
            bool isVolumeExhale = _detector.calculateLoudness > _detector.exhaleLoudnessThreshold;
            bool isWithinExhalePitchRange = (pitch > _detector.exhalePitchFrequencyThresholdLow &&
                pitch < _detector.exhalePitchFrequencyThresholdHigh);

            //Debug.Log($"Exhale condition. Loud: {isVolumeExhale} pitch: {isWithinExhalePitchRange}");// current pitch {micControl.getPitch()}

            if (isVolumeExhale
                &&
                isWithinExhalePitchRange)
            {
                mFsm.SetCurrentState((int)Breathing.EXHALE); //Change state to exhaling
            }
        }

        protected void CheckSilent()
        {

            float pitch = _detector.calculatedPitch;
            bool isSilent = _detector.calculateLoudness < _detector.silentVolumeThreshold;
            bool hasLowPitch = pitch < _detector.silentPitchThresholdHigh &&
                                pitch > _detector.silentPitchThresholdLow;
            bool hasLowVariance = _detector.Variance < _detector.silentVarianceThreshold;
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
            //check if it is still silent
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
        int counter;
        public SilentState(FSM fsm, int id, BreathingDetection detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void FixedUpdate()
        {

            //bool isSilent = _detector.calculateLoudness < _detector.silentVolumeThreshold;
            //bool hasLowPitch = _detector.calculatedPitch < _detector.silentPitchThresholdHigh;
            //bool hasLowVariance = _detector.Variance < _detector.silentVarianceThreshold;

            //Debug.Log($"silent condition. silent: {isSilent} low pitch: {hasLowPitch} Low Variance: {hasLowVariance}");// current pitch {micControl.getPitch()}

            //if (isSilent && hasLowPitch && hasLowVariance) return;

            CheckInhale();
            CheckExhale();

        }
    }

    #region testing
    
    public class TestingBreathingState : FSMState
    {
        protected BreathingDetectionNew _detector;
        protected MicrophoneController _micController;
        protected float elapseTime;

        protected float totalMinPitch;
        protected float totalMaxPitch;
        protected float totalVolume;
        protected int frameCounter;

        protected float avgMinPitch => totalMinPitch / frameCounter;
        protected float avgMaxPitch => totalMaxPitch / frameCounter;
        protected float avgVolume => totalVolume / frameCounter;


        protected int nextState = (int)Breathing.SILENT;
        public TestingBreathingState(FSM fsm, int id,
            BreathingDetectionNew detector,
            MicrophoneController controller) : base(fsm, id)
        {
            this._detector = detector;
            _micController = controller;
        }

        public override void Enter()
        {
            Debug.Log("start recording for controller");
            _micController.StartRecording();
            Debug.Log($"mic is ready: {_micController.IsMicrophoneReady}");
            totalMaxPitch = 0;
            totalMinPitch = 0;
            totalVolume = 0;

            elapseTime = 0f;
            frameCounter = 0;
        }

        public override void Update()
        {
            while(elapseTime < _detector.testingTimer)
            {
                //Debug.Log($"Running State: {(Breathing)mId} ");
                elapseTime += Time.deltaTime;
                return;
            }
            Debug.Log($"Next state {(Breathing)nextState}");
            mFsm.SetCurrentState(nextState);
            //add state change here
        }

        public override void FixedUpdate()
        {
            frameCounter++;
            totalVolume += _micController.CalculateVolumeFromRecording();
            _micController.CalculateSpectrumDataFromRecording();

            //PrintArray(_micController.FftSpectrum);

            totalMinPitch += _micController.CalculateMinPitchFromRecording();
            totalMaxPitch += _micController.CalculateMaxPitchFromRecording();
            Debug.Log($"Evaluated values\n" +
                $"totalMinPitch: {totalMinPitch} \n" +
                $"totalMaxPtich: {totalMaxPitch}\n" +
                $"frameCounter: {frameCounter}\n" +
                $"Volume: {totalVolume}");

        }


        public override void Exit()
        {
            Debug.Log("Stop recording for controller");
            _micController.StopRecording();
        }
        //do your calculation in exit

        void PrintArray(float[] array)
        {
            // Join the array elements into a single string separated by commas
            string arrayString = string.Join(", ", array);
            // Print the string to the console
            Debug.Log(arrayString);
        }
    }


    public class TestingSilence : TestingBreathingState
    {

        public TestingSilence(FSM fsm, int id, BreathingDetectionNew detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void Enter()
        {
            base.Enter();
            nextState = (int)Breathing.TESTING_INHALE;
        }


        public override void Exit()
        {
            base.Exit();
            _detector.silentPitchThresholdLow = avgMinPitch;
            _detector.silentPitchThresholdHigh = avgMaxPitch;
            _detector.silentVolumeThreshold = avgVolume;
        }

    }

    public class TestingInhale : TestingBreathingState
    {
        public TestingInhale(FSM fsm, int id, BreathingDetectionNew detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }

        public override void Enter()
        {
            base.Enter();
            nextState = (int)Breathing.TESTING_EXHALE;
        }


        public override void FixedUpdate()
        {
            _micController.CalculateSpectrumDataFromRecording();

            var curMaxPitch = _micController.CalculateMaxPitchFromRecording();
            if(curMaxPitch < -_detector.ignoreMaxPitchInhale)
            {
                //ignore it as we dont want it to dirty the data
                return;
            }
            totalMaxPitch += curMaxPitch;
            totalVolume += _micController.CalculateVolumeFromRecording();
            totalMinPitch += _micController.CalculateMinPitchFromRecording(
                _detector.minAmplitudeThresholdInhale,
                _detector.ignoreFrequencyThresholdInhale
                );


            frameCounter++;

            Debug.Log($"Evaluated values\n" +
            $"totalMinPitch: {totalMinPitch} \n" +
            $"totalMaxPtich: {totalMaxPitch}\n" +
            $"frameCounter: {frameCounter}\n" +
            $"Volume: {totalVolume}");
        }

        public override void Exit()
        {
            base.Exit();
            _detector.inhalePitchFrequencyThresholdLow = avgMinPitch - _detector.pitchOffsetLenancyInhale ;
            _detector.inhalePitchFrequencyThresholdHigh = avgMaxPitch + _detector.pitchOffsetLenancyInhale;
            //_detector.inhaleLoudnessThresholdLow = avgVolume;
            _detector.inhaleLoudnessThreshold = avgVolume;
        }

    }


    public class TestingExhale : TestingBreathingState
    {
        public TestingExhale(FSM fsm, int id, BreathingDetectionNew detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }



        public override void FixedUpdate()
        {
            _micController.CalculateSpectrumDataFromRecording();
            var curMaxPitch = _micController.CalculateMaxPitchFromRecording();
            if(curMaxPitch < _detector.ignoreMaxPitchExhale)
            {
                //ignore
                return;
            }
            totalMaxPitch += curMaxPitch;
            frameCounter++;
            totalVolume += _micController.CalculateVolumeFromRecording();
            totalMinPitch += _micController.CalculateMinPitchFromRecording(
                _detector.minAmplitudeThresholdExhale,
                _detector.ignoreFrequencyThresholdExhale);

            Debug.Log($"Evaluated values\n" +
    $"totalMinPitch: {totalMinPitch} \n" +
    $"totalMaxPtich: {totalMaxPitch}\n" +
    $"frameCounter: {frameCounter}\n" +
    $"Volume: {totalVolume}");
        }

        public override void Exit()
        {
            base.Exit();
            _detector.exhalePitchFrequencyThresholdLow = avgMinPitch + _detector.pitchOffsetLenancyExhale;
            _detector.exhalePitchFrequencyThresholdHigh = avgMaxPitch + _detector.pitchOffsetLenancyExhale;
            //_detector.exhaleLoudnessThresholdHigh= avgVolume;
            _detector.exhaleLoudnessThreshold = avgVolume;
        }

    }
    #endregion
}