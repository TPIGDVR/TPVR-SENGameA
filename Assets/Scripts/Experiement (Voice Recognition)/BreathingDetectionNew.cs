using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Breathing
{
    [RequireComponent(typeof(MicrophoneController))]

    public class BreathingDetectionNew : BreathingDetection
    {
        [Range(0, 0.00001f)]
        public float minAmplitudeThresholdInhale;
        [Range( 0, 0.00001f)]
        public float minAmplitudeThresholdExhale;

        [Range(0, 1024)]
        public int ignoreFrequencyThresholdInhale = 10;
        [Range(0, 1024)]
        public int ignoreFrequencyThresholdExhale = 10;

        //ignore max pitch recorded below a threshold to increase accuracy of the data
        [Range(100, 8000)]
        public float ignoreMaxPitchInhale = 1000;
        [Range(100, 8000)]
        public float ignoreMaxPitchExhale = 1000;

        public float pitchOffsetLenancyInhale = 100f;
        public float pitchOffsetLenancyExhale = 100f;
        protected override void SetUpFSM()
        {
            fsm = new();
            fsm.Add(new InhalingState(fsm, (int)Breathing.INHALE, this));
            fsm.Add(new ExhalingState(fsm, (int)Breathing.EXHALE, this));
            fsm.Add(new SilentState(fsm, (int)Breathing.SILENT, this));

            //fsm.Add(new TestingSilence(fsm, (int)Breathing.TESTING_SILENT,this, micControl));
            //fsm.Add(new TestingInhale(fsm, (int)Breathing.TESTING_INHALE, this, micControl));
            //fsm.Add(new TestingExhale(fsm, (int)Breathing.TESTING_EXHALE, this, micControl));

            fsm.Add(new TestingSilenceNew(fsm, (int)Breathing.TESTING_SILENT, this, micControl));
            fsm.Add(new TestingInhaleNew(fsm, (int)Breathing.TESTING_INHALE, this, micControl));
            fsm.Add(new TestingExhaleNew(fsm, (int)Breathing.TESTING_EXHALE, this, micControl));

            startingState = (int)Breathing.TESTING_SILENT;
        }


        private void Update()
        {
            fsm.Update();
        }

        [ContextMenu("Testing")]
        public void RestartTesting()
        {
            fsm.SetCurrentState((int)(Breathing.TESTING_SILENT));
        }
    }
}