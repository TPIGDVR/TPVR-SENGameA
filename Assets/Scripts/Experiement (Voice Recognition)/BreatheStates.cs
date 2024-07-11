using PGGE.Patterns;

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
        WAIT_FOR_BREATH,
    }

    #region breathe states
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
                //provider.CalculatedPitch > inhaleData.InhalePitchLowBound;
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
                //provider.CalculatedPitch > exhaleData.ExhalePitchLowBound;
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
                    provider.CalculatedPitch < silenceData.SilencePitchUpperBound;
                    //provider.CalculatePitchVariance < silenceData.SilencePitchVaranceThreshold;
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
            if (IsInhale) return;
            else if (IsExhale)
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
            if(IsSilence) return;
            if (IsExhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
            if (IsInhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.INHALE);
            }
        }

    }

    public class BreatheStateNew : FSMState
    {
        VolumeProvider provider;
        InhaleData inhaleData;
        ExhaleData exhaleData;
        SilenceData silenceData;

        public BreatheStateNew(FSM fsm, int id, VolumeProvider provider,
        InhaleData inhaleData,
        ExhaleData exhaleData,
        SilenceData silenceData) : base(fsm, id)
        {
            this.provider = provider;
            this.inhaleData = inhaleData;
            this.exhaleData = exhaleData;
            this.silenceData = silenceData;
        }

        public bool CanInhaleTransition
        {
            get
            {
                return provider.CalculatedVolume > inhaleData.InhaleVolumeThreshold &&
                //provider.CalculatedPitch > inhaleData.InhalePitchLowBound;
                //provider.CalculatedVolume < exhaleData.ExhaleVolumeThreshold &&
                //provider.CalculatedVolumeVariance > inhaleData.InhaleLoudnessVarance &&
                //(inhaleData.InhalePitchLowBound < provider.CalculatedPitch &&
                //inhaleData.InhalePitchLowBound < provider.CalculatedPitch &&
                provider.maxPitch < inhaleData.InhalePitchUpperBound;
            }
        }

        public bool CanMatainInhale
        {
            get
            {
                return provider.CalculatedVolume > inhaleData.InhaleVolumeThreshold;
            }
        }

        public bool CanExhaleTransition
        {
            get
            {
                return provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold;
                //provider.CalculatedVolumeVariance > exhaleData.ExhaleVolumeVaranceThreshold;
                //provider.CalculatePitchVariance > exhaleData.ExhalePitchVaranceThreshold;
                //provider.CalculatedPitch > exhaleData.ExhalePitchLowBound;
                //(exhaleData.ExhalePitchLowBound < provider.CalculatedPitch &&
                //provider.CalculatedPitch < exhaleData.ExhalePitchUpperBound
                //);
            }
        }

        public bool CanMainExhale
        {
            get => CanExhaleTransition;
        }

        public bool IsSilence
        {
            get
            {
                return provider.CalculatedVolume < silenceData.SilenceVolumeThreshold &&
                    provider.CalculatedPitch < silenceData.SilencePitchUpperBound;
                //provider.CalculatePitchVariance < silenceData.SilencePitchVaranceThreshold;
            }
        }


    }

    public class InhaleStateNew : BreatheStateNew
    {

        public InhaleStateNew(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if (CanMatainInhale) return;
            if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.WAIT_FOR_BREATH);
            }
            else if(CanExhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }
    }

    public class WaitForExhaleState : BreatheStateNew
    {
        public WaitForExhaleState(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if(CanExhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }
    }

    public class ExhaleStateNew : BreatheStateNew
    {
        public ExhaleStateNew(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Update()
        {
            if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.SILENT);
            }
        }
    }

    public class SilentStateNew : BreatheStateNew
    {
        float elapseTime = 0f;
        float coolDownTime = 0.1f;
        public SilentStateNew(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Enter()
        {
            elapseTime = 0f;
        }

        public override void Update()
        {//will wait for inhale
            while(coolDownTime > elapseTime)
            {
                elapseTime += Time.deltaTime;
                return;
            }
            if (IsSilence) return;
            if (CanInhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.INHALE);
            }
        }


    }

    #endregion
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
        protected int prevState;
        protected int amountToTest; 
        int testCounter;
        bool haveFinishTesting = false;
        public TestState(FSM fsm, int id, VolumeProvider provider, 
            float timer, 
            int amountToTest) : base(fsm, id)
        {
            this.timer = timer;
            this.volumeProvider = provider;
            this.amountToTest = amountToTest;
        }

        public override void Enter()
        {
            if (testCounter < amountToTest)
            {
                testCounter++;
            }
            else
            {
                haveFinishTesting = true;
            }
            elapseTimer = 0;
        }

        private void ResetVariables()
        {
            totalVolume = 0;
            totalMinPitch = 0;
            totalMaxPitch = 0;
            totalVolumeVarance = 0;
            totalPitchVarance = 0;
            counter = 0;
        }

        public override void Update()
        {
            //Debug.Log($"Timer :{timer}");
            while (timer > elapseTimer)
            {
                elapseTimer += Time.deltaTime;
                return;
            }
            if (haveFinishTesting)
            {
                haveFinishTesting = false;
                testCounter = 0;
                FinishTesting();
                ResetVariables();
                mFsm.SetCurrentState(nextState);
            }
            else
            {
                mFsm.SetCurrentState(prevState);
            }
        }

        protected virtual void FinishTesting()
        {

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
        public TestInhaleState(FSM fsm, int id, VolumeProvider provider, float timer, InhaleData data, int amountToTest) : base(fsm, id, provider, timer, amountToTest)
        {
            this.data = data;
            nextState = (int)BreathingStates.EXHALE_TESTING;
            prevState = (int)BreathingStates.SILENT_TESTING;
        }

        protected override void CalculatingTotals()
        {
            base.CalculatingTotals();
            totalVolume += volumeProvider.CalculatedVolume;
            totalMaxPitch += volumeProvider.maxPitch;
            totalMinPitch += volumeProvider.minPitch;
            totalVolumeVarance += volumeProvider.CalculatedVolumeVariance;
        }

        public override void Exit()
        {
            //data.InhaleVolumeThreshold += totalVolume / counter + data.InhaleVolumeOffset;
            //data.InhalePitchLowBound += totalMinPitch / counter;
            //data.InhalePitchUpperBound += totalMaxPitch / counter;
            //data.InhaleLoudnessVarance += totalVolumeVarance / counter;
            //data.InhalePitchLowBound -= data.InhalePitchOffset;
            //data.InhalePitchUpperBound += data.InhalePitchOffset;
        }

        protected override void FinishTesting()
        {
            //data.InhaleVolumeThreshold /= amountToTest;
            //data.InhalePitchLowBound /= amountToTest;
            //data.InhalePitchUpperBound /= amountToTest;
            //data.InhaleLoudnessVarance /= amountToTest;

            data.InhaleVolumeThreshold = totalVolume / counter + data.InhaleVolumeOffset;
            data.InhalePitchLowBound = totalMinPitch / counter;
            data.InhalePitchUpperBound+= totalMaxPitch / counter;
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
            int amountToTest,
            SilenceData data2) : base(fsm, id, provider, timer, data,amountToTest)
        {
            silentData = data2;
            prevState = (int)BreathingStates.EXHALE_TESTING;
            nextState = (int)BreathingStates.EXHALE_TESTING;
        }

        public override void Exit()
        {
            base.Exit();

        }

        protected override void FinishTesting()
        {
            base.FinishTesting();
            silentData.SilenceVolumeThreshold = data.InhaleVolumeThreshold + silentData.SilenceVolumeOffset;
            silentData.SilencePitchUpperBound = data.InhalePitchLowBound;
            silentData.SilencePitchLowBound = 0;
        }
    }

    public class TestSilentState : TestState
    {
        public SilenceData Data;

        public TestSilentState(FSM fsm, int id, VolumeProvider provider, float timer , SilenceData data, int amountToTest) : 
            base(fsm, id, provider, timer, amountToTest)
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
        public TestExhaleState(FSM fsm, int id, VolumeProvider provider, float timer, ExhaleData data, int amountToTest) : 
            base(fsm, id, provider, timer, amountToTest)
        {
            this.data = data;
            nextState = (int)BreathingStates.SILENT;
            prevState = (int)BreathingStates.INHALE_TESTING;
        }

        protected override void CalculatingTotals()
        {
            base.CalculatingTotals();
            totalVolume += volumeProvider.CalculatedVolume;
            totalMaxPitch += volumeProvider.maxPitch;
            totalMinPitch += volumeProvider.minPitch;
            totalPitchVarance += volumeProvider.CalculatePitchVariance;
            totalVolumeVarance += volumeProvider.CalculatedVolumeVariance;
        }

        protected override void FinishTesting()
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