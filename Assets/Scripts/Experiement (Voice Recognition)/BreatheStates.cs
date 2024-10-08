﻿using PGGE.Patterns;

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
        UNREGISTERED
    }

    #region breathe states

    //V1 was just to figure out what kind of condition
    //that should be used for inhaling and exhaling.
    #region V1 inhale/exhale states
    public class BreatheStateV1 : FSMState
    {
        //u can then swap the provider of this slideshowData to test which one is better suited :)
        protected VolumeProvider provider;
        protected InhaleData inhaleData;
        protected ExhaleData exhaleData;
        protected SilenceData silenceData;

        public BreatheStateV1(FSM fsm, int id, VolumeProvider provider,
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
                //provider.CalculatedVolume < exhaleLoudnessData.ExhaleVolumeThreshold &&
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
                //provider.CalculatedVolumeVariance > exhaleLoudnessData.ExhaleVolumeVaranceThreshold;
                //provider.CalculatePitchVariance > exhaleLoudnessData.ExhalePitchVaranceThreshold;
                //provider.CalculatedPitch > exhaleLoudnessData.ExhalePitchLowBound;
                //(exhaleLoudnessData.ExhalePitchLowBound < provider.CalculatedPitch &&
                //provider.CalculatedPitch < exhaleLoudnessData.ExhalePitchUpperBound
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
    public class InhaleState : BreatheStateV1
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

    public class ExhaleState : BreatheStateV1
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

    public class SilentState : BreatheStateV1
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
    #endregion
    
    //V2 was trying to improve the accuracy of the breathing detection.
    #region V2 inhale/exhale states
    public class BreatheStateV2 : FSMState
    {
        VolumeProvider provider;
        InhaleData inhaleData;
        ExhaleData exhaleData;
        SilenceData silenceData;

        public BreatheStateV2(FSM fsm, int id, VolumeProvider provider,
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
                //provider.CalculatedVolume < exhaleLoudnessData.ExhaleVolumeThreshold &&
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
                //provider.CalculatedVolumeVariance > exhaleLoudnessData.ExhaleVolumeVaranceThreshold;
                //provider.CalculatePitchVariance > exhaleLoudnessData.ExhalePitchVaranceThreshold;
                //provider.CalculatedPitch > exhaleLoudnessData.ExhalePitchLowBound;
                //(exhaleLoudnessData.ExhalePitchLowBound < provider.CalculatedPitch &&
                //provider.CalculatedPitch < exhaleLoudnessData.ExhalePitchUpperBound
                //);
            }
        }

        public bool IsTalking
        {
            get
            {//that means that the player is doing something funny in the background.
                return provider.PitchNoiseCorrelation >
                    Mathf.Max(inhaleData.InhalePitchNoiseCorrelation, exhaleData.ExhalePitchNoiseCorrelation);
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

    public class InhaleStateNew : BreatheStateV2
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

    public class WaitForExhaleState : BreatheStateV2
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

    public class ExhaleStateNew : BreatheStateV2
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

    public class SilentStateNew : BreatheStateV2
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

    //v3 can be the player signalling when they inhale or exhale.
    #region v3 Inhale/exhale states
    /*
     4/5 phases.
    1. Silence phase (for inhale)
    2. Inhaling phase (when the player signals the inhaling and tries to maintain it)
    3. silence phase (for exhaling)
    4. Exhaling phase (when the player signals the exhaling and tries to maintain it)
    5. Talking phase (If the player talks at any point of time, return to silence phase (inhale) as
    the system does not recognise and will not reward the player for wasting it's time.
     */

    public class BreatheStateV3 : FSMState
    {
        VolumeProvider provider;
        InhaleData inhaleData;
        ExhaleData exhaleData;
        SilenceData silenceData;

        public BreatheStateV3(FSM fsm, int id, VolumeProvider provider,
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
                //provider.CalculatedVolume < exhaleLoudnessData.ExhaleVolumeThreshold &&
                //provider.CalculatedVolumeVariance > inhaleData.InhaleLoudnessVarance &&
                //(inhaleData.InhalePitchLowBound < provider.CalculatedPitch &&
                inhaleData.InhalePitchLowBound < provider.CalculatedPitch &&
                provider.CalculatedPitch < inhaleData.InhalePitchUpperBound;
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
                return provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold &&
                //provider.CalculatedVolumeVariance > exhaleLoudnessData.ExhaleVolumeVaranceThreshold;
                //provider.CalculatePitchVariance > exhaleLoudnessData.ExhalePitchVaranceThreshold 
                provider.CalculatedPitch > exhaleData.ExhalePitchLowBound &&
                exhaleData.ExhalePitchLowBound < provider.CalculatedPitch;
                //provider.CalculatedPitch < exhaleLoudnessData.ExhalePitchUpperBound
                //);
            }
        }


        public bool CanMaintainExhale
        {
            get => provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold;
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

        public bool IsTalking
        {
            get { return provider.PitchNoiseCorrelation < 0f; }
        }

        public override void Update()
        {
            AnyStateTransiton();
            TransitonStates();
        }

        protected virtual void TransitonStates()
        {

        }

        protected virtual void AnyStateTransiton()
        {
            if (IsTalking)
            {
                mFsm.SetCurrentState((int)BreathingStates.UNREGISTERED);
            }
        }
    }

    public class WaitForInhaleV3 : BreatheStateV3
    {
        public WaitForInhaleV3(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        protected override void TransitonStates()
        {
            if (CanInhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.INHALE);
            }
        }
    }

    public class InhaleStateV3 : BreatheStateV3
    {
        public InhaleStateV3(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        protected override void TransitonStates()
        {
            if (CanMatainInhale) return;
            else if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.WAIT_FOR_BREATH);
            }
            else if (CanExhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }
    }

    public class WaitForExhaleV3 : BreatheStateV3
    {
        public WaitForExhaleV3(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        protected override void TransitonStates()
        {
            if (CanExhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }
    }

    public class ExhaleStateV3 : BreatheStateV3
    {
        public ExhaleStateV3(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        protected override void TransitonStates()
        {
            if (CanMaintainExhale) return;
            if (IsSilence)
            {
                mFsm.SetCurrentState((int)BreathingStates.SILENT);
            }
        }
    }

    public class UnregisterStateV3 : BreatheStateV3
    {
        float elapseTime = 0f;
        public UnregisterStateV3(FSM fsm, int id, VolumeProvider provider, InhaleData inhaleData, ExhaleData exhaleData, SilenceData silenceData) : base(fsm, id, provider, inhaleData, exhaleData, silenceData)
        {
        }

        public override void Enter()
        {
            elapseTime = 0f;
        }

        protected override void TransitonStates()
        {
            while(elapseTime < 3f)
            {
                elapseTime += Time.deltaTime;
                return;
            }
            mFsm.SetCurrentState((int)BreathingStates.SILENT);

        }

        protected override void AnyStateTransiton()
        {
        }
    }

    #endregion

    #region V4 Inhale/exhale State

    public class BreatheStateV4 : FSMState
    {
        VolumeProvider provider;
        ExhaleData exhaleData;

        public BreatheStateV4(FSM fsm, int id, VolumeProvider provider,
        ExhaleData exhaleData) : base(fsm, id)
        {
            this.provider = provider;
            this.exhaleData = exhaleData;
        }

        public bool CanExhaleTransition
        {
            get
            {
                return provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold &&
                //provider.CalculatedVolumeVariance > exhaleLoudnessData.ExhaleVolumeVaranceThreshold;
                //provider.CalculatePitchVariance > exhaleLoudnessData.ExhalePitchVaranceThreshold 
                provider.CalculatedPitch > exhaleData.ExhalePitchLowBound &&
                //provider.CalculatedPitch < exhaleLoudnessData.ExhalePitchUpperBound &&
                provider.PitchNoiseCorrelation < 0;
                //exhaleLoudnessData.ExhalePitchLowBound < provider.CalculatedPitch;
                //provider.CalculatedPitch < exhaleLoudnessData.ExhalePitchUpperBound
                //);
            }
        }

        public bool CanMaintainExhale
        {
            get => provider.PitchNoiseCorrelation < exhaleData.ExhalePitchNoiseCorrelation 
                && provider.CalculatedVolume > exhaleData.ExhaleVolumeThreshold;
        }
    }

    public class ExhaleStateV4 : BreatheStateV4
    {
        public ExhaleStateV4(FSM fsm, int id, VolumeProvider provider, ExhaleData exhaleData) : base(fsm, 
            id, provider, exhaleData )
        {
        }

        public override void Update()
        {
            if (!CanMaintainExhale)
            {
                mFsm.SetCurrentState((int)BreathingStates.WAIT_FOR_BREATH);
            }
        }
    }

    public class WaitForBreatheV4 : BreatheStateV4
    {
        public WaitForBreatheV4(FSM fsm, int id, VolumeProvider provider, ExhaleData exhaleData) 
            : base(fsm, id, provider, exhaleData)
        {
        }

        public override void Update()
        {
            if (CanExhaleTransition)
            {
                mFsm.SetCurrentState((int)BreathingStates.EXHALE);
            }
        }

    }



    #endregion

    #endregion

    #region test states
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
        protected float totalPitchNoiseCorrelation;
        protected float totalVolumeNoiseCorrelation;
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
            //Debug.Log($"Timer :{timerOffset}");
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

        public TestState SetNextState(BreathingStates state)
        {
            nextState = (int)state;
            return this;
        }

        public TestState setPreviousState(BreathingStates state)
        {
            prevState = (int)state;
            return this;
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
            totalVolumeNoiseCorrelation += volumeProvider.VolumeNoiseCorrelation;
            totalPitchNoiseCorrelation += volumeProvider.PitchNoiseCorrelation;
        }

        protected override void FinishTesting()
        {
            data.InhaleVolumeThreshold = totalVolume / counter + data.InhaleVolumeOffset;
            data.InhalePitchLowBound = totalMinPitch / counter;
            data.InhalePitchUpperBound = totalMaxPitch / counter;
            data.InhaleLoudnessVarance = totalVolumeVarance / counter;
            data.InhalePitchNoiseCorrelation = totalPitchNoiseCorrelation / counter;
            data.InhaleVolumeNoiseCorrelation = totalVolumeNoiseCorrelation / counter;
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
            totalPitchNoiseCorrelation += volumeProvider.PitchNoiseCorrelation;
            totalVolumeNoiseCorrelation += volumeProvider.VolumeNoiseCorrelation;
        }

        protected override void FinishTesting()
        {
            data.ExhaleVolumeThreshold = totalVolume / counter + data.ExhaleVolumeOffset;
            data.ExhalePitchLowBound = totalMinPitch / counter;
            data.ExhalePitchUpperBound = totalMaxPitch / counter;
            data.ExhalePitchVaranceThreshold = totalPitchVarance / counter;
            data.ExhaleVolumeVaranceThreshold = totalVolumeVarance / counter;

            data.ExhalePitchNoiseCorrelation = totalPitchNoiseCorrelation / counter;
            data.ExhaleVolumeNoiseCorrelation = totalVolumeNoiseCorrelation / counter;

            data.ExhalePitchLowBound -= data.ExhalePitchOffset;
            data.ExhalePitchUpperBound += data.ExhalePitchOffset;
        }
    }
    /// <summary>
    /// Legacy, Use if u find a need to. But usally u dont need
    /// since the fsm just need to know when the player is inhaling,
    /// exhaling, or talking.
    /// </summary>
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
    #endregion
}