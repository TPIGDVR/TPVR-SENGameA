using PGGE.Patterns;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Breathing
{
    [RequireComponent(typeof(MicrophoneController))]

    public class BreathingDetectionNew : BreathingDetection
    {
        protected override void SetUpFSM()
        {
            fsm = new();
            fsm.Add(new InhalingState(fsm, (int)Breathing.INHALE, this, micControl));
            fsm.Add(new ExhalingState(fsm, (int)Breathing.EXHALE, this, micControl));
            fsm.Add(new SilentState(fsm, (int)Breathing.SILENT, this, micControl));
            fsm.Add(new TestingSilent(fsm, (int)Breathing.TESTING_SILENT, this, micControl));
            fsm.Add(new TestingInhaling(fsm, (int)Breathing.TESTING_INHALE, this, micControl));
            fsm.Add(new TestingExhaling(fsm, (int)Breathing.TESTING_EXHALE, this,micControl));

            fsm.SetCurrentState((int)Breathing.TESTING_SILENT);

        }


        private void Update()
        {
            fsm.Update();
        }

    }
}