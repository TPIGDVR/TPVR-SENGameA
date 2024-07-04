using PGGE.Patterns;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Breathing3
{

    public enum BreathingStates
    {
        INHALE,
        EXHALE,
        SILENT,
        INHALE_TESTING,
        EXHALE_TESTING,
        SILENT_TESTING,
    }
    public class BreatheState : FSMState
    {
        //u can then swap the provider of this data to test which one is better suited :)
        protected VolumeProvider provider;
        protected InhaleData inhaleData;
        protected ExhaleData exhaleData;
        protected SilenceData silenceData;

        public BreatheState(FSM fsm, int id, VolumeProvider provider,
            InhaleData inhaleData,
            ExhaleData exhaleData,
            SilenceData silenceData ) : base(fsm, id)
        {
            this.provider = provider;
            this.inhaleData = inhaleData;
            this.exhaleData = exhaleData;
            this.silenceData = silenceData;
        }

        public bool IsInhale
        {
            get
            {
                return provider.CalculatedVolume > inhaleData.InhaleVolumeThreshold &&
                    //provider.CalculatedVolume < exhaleData.ExhaleVolumeThreshold &&
                    //provider.CalculatedVolumeVariance < inhaleData.InhaleLoudnessVarance &&
                    //(inhaleData.InhalePitchLowBound < provider.CalculatedPitch &&
                    provider.CalculatedPitch < inhaleData.InhalePitchUpperBound;
            }
        }

        //addd pitch and volume varance to test whether that can be used to 
        //determine the exhale state
        public bool IsExhale
        {
            get
            {
                return provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold;
                    //provider.CalculatedVolumeVariance > exhaleData.ExhaleVolumeVaranceThreshold;
                    //provider.CalculatePitchVariance > exhaleData.ExhalePitchVaranceThreshold;
                    //(exhaleData.ExhalePitchLowBound < provider.CalculatedPitch &&
                    //provider.CalculatedPitch < exhaleData.ExhalePitchUpperBound
                    //);
            }
        }

        public bool IsSilence
        {
            get
            {
                return provider.CalculatedVolume < silenceData.SilenceVolumeThreshold &&
                    provider.CalculatedPitch < silenceData.SilencePitchUpperBound &&
                    provider.CalculatePitchVariance < silenceData.SilencePitchVaranceThreshold;
            }
        }

        public override void Update()
        {
            Debug.Log($"Inhale: {IsInhale} Exhale: {IsExhale} Silence: {IsSilence}");
        }

    }

    public class InhaleState : BreatheState
    {
        public InhaleState(FSM fsm, 
            int id,
            VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if(IsExhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
            else if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.SILENT);
            }
            
        }

    }

    public class ExhaleState : BreatheState
    {
        public ExhaleState(FSM fsm,
            int id,
            VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if (IsExhale) return;
            if (IsInhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.INHALE);
            }
            else if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.SILENT);
            }
        }
    }

    public class SilentState : BreatheState
    {
        public SilentState(FSM fsm,
            int id,
            VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if (IsInhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.INHALE);
            }
            else if (IsExhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }

    }


    public class TestState : FSMState
    {
        protected VolumeProvider volumeProvider;

        float timer;
        float elapseTimer;

        protected float totalVolume;
        protected float totalMinPitch;
        protected float totalMaxPitch;
        protected float totalVolumeVarance;
        protected float totalPitchVarance;
        protected int counter;
        protected int nextState;

        public TestState(FSM fsm, int id, VolumeProvider provider, float timer) : base(fsm, id)
        {
            this.timer = timer;
            this.volumeProvider = provider;
        }

        public override void Enter()
        {
            totalVolume = 0;
            totalMinPitch = 0;
            totalMaxPitch = 0;
            counter = 0;
            elapseTimer = 0;
            totalVolumeVarance = 0;
            totalPitchVarance = 0;
        }

        public override void Update()
        {
            while (timer > elapseTimer)
            {
                elapseTimer += Time.deltaTime;
                return;
            }
            mFsm.SetCurrentState(nextState);
        }

        public override void FixedUpdate()
        {
            CalculatingTotals();
            
        }

        protected virtual void CalculatingTotals()
        {
            counter++;
            //add your other stuff here
        }
    }

    public class TestInhaleState : TestState
    {
        protected InhaleData data;
        public TestInhaleState(FSM fsm, int id, VolumeProvider provider, float timer, InhaleData data) : base(fsm, id, provider, timer)
        {
            this.data = data;
            nextState = (int)BreathingStates.EXHALE_TESTING;
        }

        protected override void CalculatingTotals()
        {
            base.CalculatingTotals();
            totalVolume += volumeProvider.CalculatedVolume;
            totalMaxPitch += volumeProvider.maxPitch;
            totalMinPitch += volumeProvider.minPitch > 50 ? volumeProvider.minPitch : volumeProvider.avgPitch;
            totalVolumeVarance += volumeProvider.CalculatedVolumeVariance;
        }

        public override void Exit()
        {
            data.InhaleVolumeThreshold = totalVolume / counter + data.InhaleVolumeOffset;
            data.InhalePitchLowBound = totalMinPitch / counter;
            data.InhalePitchUpperBound = totalMaxPitch / counter;
            data.InhaleLoudnessVarance = totalVolumeVarance / counter;
            data.InhalePitchLowBound -= data.InhalePitchOffset;
            data.InhalePitchUpperBound += data.InhalePitchOffset;
        }
    }

    public class TestInhaleStateNew : TestInhaleState
    {
        SilenceData silentData;
        public TestInhaleStateNew(FSM fsm, 
            int id, 
            VolumeProvider provider, 
            float timer, 
            InhaleData data,
            SilenceData data2) : base(fsm, id, provider, timer, data)
        {
            silentData = data2;
        }

        public override void Exit()
        {
            base.Exit();
            silentData.SilenceVolumeThreshold = data.InhaleVolumeThreshold;
            silentData.SilencePitchUpperBound = data.InhalePitchLowBound;
            silentData.SilencePitchLowBound = 0;
        }
    }

    public class TestSilentState : TestState
    {
        public SilenceData Data;


        public TestSilentState(FSM fsm, int id, VolumeProvider provider, float timer , SilenceData data) : base(fsm, id, provider, timer)
        {
            nextState = (int)BreathingStates.INHALE_TESTING;
            Data = data;
        }

        protected override void CalculatingTotals()
        {
            base.CalculatingTotals();
            totalVolume += volumeProvider.CalculatedVolume;
            totalMaxPitch += volumeProvider.maxPitch;
            totalPitchVarance += volumeProvider.CalculatePitchVariance;
        }


        public override void Exit()
        {
            Data.SilenceVolumeThreshold = totalVolume / counter;
            Data.SilencePitchLowBound = 0;
            Data.SilencePitchUpperBound = totalMaxPitch / counter;
            //Data.SilencePitchVaranceThreshold = totalPitchVarance / counter;
        }
    }


    public class TestExhaleState : TestState
    {
        ExhaleData data;
        public TestExhaleState(FSM fsm, int id, VolumeProvider provider, float timer, ExhaleData data) : base(fsm, id, provider, timer)
        {
            this.data = data;
            nextState = (int)BreathingStates.SILENT;
        }

        protected override void CalculatingTotals()
        {
            base.CalculatingTotals();
            totalVolume += volumeProvider.CalculatedVolume;
            totalMaxPitch += volumeProvider.maxPitch;
            totalMinPitch += volumeProvider.minPitch > 50 ? volumeProvider.minPitch : volumeProvider.avgPitch;
            totalPitchVarance += volumeProvider.CalculatePitchVariance;
            totalVolumeVarance += volumeProvider.CalculatedVolumeVariance;
        }

        public override void Exit()
        {
            data.ExhaleVolumeThreshold = totalVolume / counter + data.ExhaleVolumeOffset;
            data.ExhalePitchLowBound = totalMinPitch / counter;
            data.ExhalePitchUpperBound = totalMaxPitch / counter;
            data.ExhalePitchVaranceThreshold = totalPitchVarance / counter;
            data.ExhaleVolumeVaranceThreshold = totalVolumeVarance / counter;
            data.ExhalePitchLowBound -= data.ExhalePitchOffset;
            data.ExhalePitchUpperBound += data.ExhalePitchOffset;
        }
    }

}