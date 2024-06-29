using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;

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
            //bool isTooQuiet = _detector.minimizedLoudness < _detector.inhaleLoudnessThresholdLow;
            bool hasLargeVolumeDifference = true; /*_detector.Variance < _detector.inhaleVarianceThreshold;*/
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
            _micController.StartRecording();
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
            var curMaxPitch = _micController.CalculateMaxPitchFromRecording();
            if(curMaxPitch < -_detector.ignoreMaxPitchInhale)
            {
                //ignore it as we dont want it to dirty the data
                return;
            }
            totalMaxPitch += curMaxPitch;
            totalVolume += _micController.CalculateVolumeFromRecording();
            _micController.CalculateSpectrumDataFromRecording();
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
            _detector.inhalePitchFrequencyThresholdLow = avgMinPitch;
            _detector.inhalePitchFrequencyThresholdHigh = avgMaxPitch;
            _detector.inhaleLoudnessThresholdLow = avgVolume;
            _detector.inhaleLoudnessThresholdHigh = avgVolume;
        }

    }


    public class TestingExhale : TestingBreathingState
    {
        public TestingExhale(FSM fsm, int id, BreathingDetectionNew detector, MicrophoneController controller) : base(fsm, id, detector, controller)
        {
        }



        public override void FixedUpdate()
        {
            frameCounter++;
            totalVolume += _micController.CalculateVolumeFromRecording();
            _micController.CalculateSpectrumDataFromRecording();
            totalMinPitch += _micController.CalculateMinPitchFromRecording(
                _detector.minAmplitudeThresholdExhale,
                _detector.ignoreFrequencyThresholdExhale);
            totalMaxPitch += _micController.CalculateMaxPitchFromRecording();

            Debug.Log($"Evaluated values\n" +
    $"totalMinPitch: {totalMinPitch} \n" +
    $"totalMaxPtich: {totalMaxPitch}\n" +
    $"frameCounter: {frameCounter}\n" +
    $"Volume: {totalVolume}");
        }

        public override void Exit()
        {
            base.Exit();
            _detector.exhalePitchFrequencyThresholdLow = avgMinPitch;
            _detector.exhalePitchFrequencyThresholdHigh = avgMaxPitch;
            _detector.exhaleLoudnessThresholdHigh= avgVolume;
            _detector.exhaleLoudnessThresholdLow = avgVolume;
        }

    }
    #endregion
}