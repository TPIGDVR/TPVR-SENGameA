using PGGE.Patterns;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Breathing3
{
    public class AudioDetector : MonoBehaviour,
        SilenceData,
        InhaleData,
        ExhaleData,
        OutputBreath
    {
        //information about the volume currently
        VolumeProvider audioProvider;

        #region properties
        [Header("Silence")]
        [SerializeField] float silenceVolumeThreshold;
        [SerializeField] float silencePitchUpperBound;
        [SerializeField] float silencePitchLowerBound;
        [SerializeField] float silencePitchVaranceThreshold;
        [SerializeField] float silenceVolumeOffset;
        [Header("Inhale")]
        [SerializeField] float inhaleVolumeThreshold;
        [SerializeField] float inhalePitchUpperBound;
        [SerializeField] float inhalePitchLowerBound;
        [SerializeField] float inhalePitchOffset;
        [SerializeField] float inhaleLoudnessVaranceThreshold;
        [SerializeField] float inhaleVolumeOffset;
        [SerializeField] float inhalePitchNoiseCorrelation;
        [SerializeField] float inhaleVolumeNoiseCorrelation;
        [Header("Exhale")]
        [SerializeField] float exhaleVolumethreshold;
        [SerializeField] float exhalePitchUpperBound;
        [SerializeField] float exhalePitchLowerBound;
        [SerializeField] float exhalePitchOffset;
        [SerializeField] float exhaleVolumeOffset;
        [SerializeField] float exhaleLoudnessVaranceThreshold;
        [SerializeField] float exhalePitchVaranceThreshold;
        [SerializeField] float exhaleVolumeNoiseCorrelation;
        [SerializeField] float exhalePitchNoiseCorrelation;

        [Header("Data Text")]
        public TextMeshProUGUI stateText;

        [Header("other settings")]
        [SerializeField] private float testTimer;
        [SerializeField] private int amountOfTest;
        public InhaleDataSO  presetInhaleData;
        public ExhaleDataSO presetExhaleData;
        public SilentDataSO PresetSilenceData;
        [SerializeField] private bool usedPresetData;
        #endregion

        #region implemented interface
        public float SilenceVolumeThreshold { get => silenceVolumeThreshold; set => silenceVolumeThreshold = value; }
        public float SilencePitchUpperBound { get => silencePitchUpperBound; set => silencePitchUpperBound = value; }
        public float SilencePitchLowBound { get => silencePitchLowerBound; set => silencePitchLowerBound = value; }
        public float SilencePitchVaranceThreshold { get => silencePitchVaranceThreshold; set => silencePitchVaranceThreshold = value; }
        public float InhaleVolumeThreshold { get => inhaleVolumeThreshold; set => inhaleVolumeThreshold = value; }
        public float InhalePitchUpperBound { get => inhalePitchUpperBound; set => inhalePitchUpperBound = value; }
        public float InhalePitchLowBound { get => inhalePitchLowerBound; set => inhalePitchLowerBound = value; }
        public float ExhaleVolumeThreshold { get => exhaleVolumethreshold; set => exhaleVolumethreshold = value; }
        public float ExhalePitchUpperBound { get => exhalePitchUpperBound; set => exhalePitchUpperBound = value; }
        public float ExhalePitchLowBound { get => exhalePitchLowerBound; set => exhalePitchLowerBound = value; }
        public float ExhalePitchOffset { get => exhalePitchOffset;}
        public float ExhaleVolumeOffset => exhaleVolumeOffset;
        public float InhalePitchOffset { get => inhalePitchOffset; }
        public float InhaleLoudnessVarance { get => inhaleLoudnessVaranceThreshold; set => inhaleLoudnessVaranceThreshold = value; }


        public float InhaleVolumeOffset => inhaleVolumeOffset;

        public float ExhaleVolumeVaranceThreshold {get => exhaleLoudnessVaranceThreshold; set => exhaleLoudnessVaranceThreshold = value; }
        public float ExhalePitchVaranceThreshold { get => exhalePitchVaranceThreshold; set => exhalePitchVaranceThreshold = value; }
        public float SilenceVolumeOffset { get => silenceVolumeOffset; set => silenceVolumeOffset =value; }
        public int AmountOfTest { get => amountOfTest; set => amountOfTest = value; }

        public float ExhalePitchNoiseCorrelation { get => exhalePitchNoiseCorrelation; set => exhalePitchNoiseCorrelation = value; }
        public float ExhaleVolumeNoiseCorrelation { get => exhaleVolumeNoiseCorrelation; set => exhaleVolumeNoiseCorrelation = value; }
        public float InhalePitchNoiseCorrelation { get => inhalePitchNoiseCorrelation; set => inhalePitchNoiseCorrelation = value; }
        public float InhaleVolumeNoiseCorrelation { get => inhaleVolumeNoiseCorrelation; set => inhaleVolumeNoiseCorrelation = value; }
        #endregion

        FSM fsm;
        private void Start()
        {
            audioProvider = GetComponent<VolumeProvider>();
            fsm = new FSM();
            SetUpStatesV1();
            //SetUpStateV2();
            //SetUpStateV3();
        }

        private void SetUpStatesV1()
        {
            if (usedPresetData)
            {
                fsm.Add(new SilentStateNew(fsm, (int)BreathingStates.SILENT, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
                fsm.Add(new InhaleStateNew(fsm, (int)BreathingStates.INHALE, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
                fsm.Add(new ExhaleStateNew(fsm, (int)BreathingStates.EXHALE, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
                fsm.Add(new WaitForExhaleState(fsm, (int)BreathingStates.WAIT_FOR_BREATH, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
            }
            else
            {
                //old
                //fsm.Add(new SilentState(fsm, (int)BreathingStates.SILENT, audioProvider, this, this, this));
                //fsm.Add(new InhaleState(fsm, (int)BreathingStates.INHALE, audioProvider, this, this, this));
                //fsm.Add(new ExhaleState(fsm, (int)BreathingStates.EXHALE, audioProvider, this, this, this));

                fsm.Add(new SilentStateNew(fsm, (int)BreathingStates.SILENT, audioProvider, this, this, this));
                fsm.Add(new InhaleStateNew(fsm, (int)BreathingStates.INHALE, audioProvider, this, this, this));
                fsm.Add(new ExhaleStateNew(fsm, (int)BreathingStates.EXHALE, audioProvider, this, this, this));
                fsm.Add(new WaitForExhaleState(fsm, (int)BreathingStates.WAIT_FOR_BREATH, audioProvider, this, this, this));
            }
            //old testing
            //fsm.Add(new TestSilentState(fsm, (int)BreathingStates.SILENT_TESTING, audioProvider, testTimer, this));
            //fsm.Add(new TestInhaleState(fsm, (int)BreathingStates.INHALE_TESTING, audioProvider, testTimer, this));

            fsm.Add(new TestInhaleStateNew(fsm, (int)BreathingStates.INHALE_TESTING, audioProvider, testTimer, this, amountOfTest, this));
            fsm.Add(new TestExhaleState(fsm, (int)BreathingStates.EXHALE_TESTING, audioProvider, testTimer, this, amountOfTest));

            if (usedPresetData)
            {
                fsm.SetCurrentState((int)BreathingStates.SILENT);
            }
            else
            {
                fsm.SetCurrentState((int)BreathingStates.INHALE_TESTING);
            }
        }

        private void SetUpStateV2()
        {
            

            fsm.Add(new WaitForInhaleV3(fsm, (int)BreathingStates.SILENT, audioProvider, this, this, this));
            fsm.Add(new InhaleStateV3(fsm, (int)BreathingStates.INHALE, audioProvider, this, this, this));
            fsm.Add(new ExhaleStateV3(fsm, (int)BreathingStates.EXHALE, audioProvider, this, this, this));
            fsm.Add(new WaitForExhaleV3(fsm, (int)BreathingStates.WAIT_FOR_BREATH, audioProvider, this, this, this));
            fsm.Add(new UnregisterStateV3(fsm, (int)BreathingStates.UNREGISTERED, audioProvider, this, this, this));

            fsm.Add(new TestInhaleStateNew(fsm, (int)BreathingStates.INHALE_TESTING, audioProvider, testTimer, this, amountOfTest, this));
            fsm.Add(new TestExhaleState(fsm, (int)BreathingStates.EXHALE_TESTING, audioProvider, testTimer, this, amountOfTest));

            
            fsm.SetCurrentState((int)BreathingStates.INHALE_TESTING);
            
        }

        private void SetUpStateV3()
        {
            fsm.Add(new TestInhaleStateNew(fsm, (int)BreathingStates.INHALE_TESTING, audioProvider, testTimer, this, amountOfTest, this));
            fsm.Add(new TestExhaleState(fsm, 
                (int)BreathingStates.EXHALE_TESTING, 
                audioProvider, testTimer, this, amountOfTest).SetNextState(BreathingStates.WAIT_FOR_BREATH));


            fsm.Add(new WaitForBreatheV4(fsm, (int)BreathingStates.WAIT_FOR_BREATH, audioProvider, this));
            fsm.Add(new ExhaleStateV4(fsm, (int)BreathingStates.EXHALE, audioProvider, this));

            fsm.SetCurrentState((int)BreathingStates.INHALE_TESTING);

        }
        private void Update()
        {
            stateText.text = ((BreathingStates)fsm.GetCurrentState().ID).ToString();
            fsm.Update();
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();
        }

        //[ContextMenu("Testing")]
        //public void Testing()
        //{
        //    fsm.SetCurrentState((int)(BreathingStates.SILENT_TESTING));
        //}


        [ContextMenu("Testing new")]
        public void Testing2()
        {
            SilenceData curDataS = (SilenceData)this;
            ExhaleData curDataE = (ExhaleData)this;
            InhaleData curDataI = (InhaleData)this;

            curDataS.EmptyData();
            curDataE.EmptyData();
            curDataI.EmptyData();

            fsm.SetCurrentState((int)(BreathingStates.INHALE_TESTING));
        }

        [ContextMenu("Copy Data")]
        public void CopyData()
        {
            SilenceData curDataS = (SilenceData)this;
            ExhaleData  curDataE = (ExhaleData) this;
            InhaleData  curDataI = (InhaleData) this;

            curDataI.CopyData(presetInhaleData);
            curDataE.CopyData(presetExhaleData);
            curDataS.CopyData(PresetSilenceData);
        }

        public BreathingStates PlayerBreathState()
        {
            BreathingStates currentState = (BreathingStates)fsm.GetCurrentState().ID;

            if(currentState == BreathingStates.INHALE_TESTING || 
                currentState == BreathingStates.EXHALE_TESTING ||
                currentState == BreathingStates.SILENT_TESTING)
            {
                return BreathingStates.SILENT;
            }

            return currentState;
        }
    }

    public interface SilenceData
    {
        public float SilenceVolumeThreshold { get; set; }
        public float SilencePitchLowBound { get; set; }
        public float SilencePitchUpperBound { get; set; }
        public float SilencePitchVaranceThreshold { get; set; }

        public float SilenceVolumeOffset { get; set; }

        public void CopyData(SilenceData toData)
        {
            toData.SilencePitchLowBound = SilencePitchLowBound;
            toData.SilencePitchUpperBound = SilencePitchUpperBound;
            toData.SilenceVolumeThreshold = SilenceVolumeThreshold;
            toData.SilencePitchVaranceThreshold = SilencePitchVaranceThreshold;
        }

        public void EmptyData()
        {
            SilenceVolumeThreshold = 0f;
            SilencePitchLowBound = 0f;
            SilencePitchUpperBound = 0f;
            SilencePitchVaranceThreshold = 0f;
        }
    }

    public interface ExhaleData
    {
        public float ExhaleVolumeThreshold { get; set; }
        public float ExhalePitchLowBound { get; set; }
        public float ExhalePitchUpperBound { get; set; }
        public float ExhaleVolumeVaranceThreshold { get; set; }
        public float ExhalePitchVaranceThreshold { get; set; }
        public float ExhalePitchNoiseCorrelation { get; set; }
        public float ExhaleVolumeNoiseCorrelation { get; set; }
        public float ExhalePitchOffset { get;  }
        public float ExhaleVolumeOffset { get; }

        public void CopyData(ExhaleData toData)
        {
            toData.ExhaleVolumeThreshold = ExhaleVolumeThreshold;
            toData.ExhalePitchLowBound = ExhalePitchLowBound;
            toData.ExhalePitchUpperBound = ExhalePitchUpperBound;
            toData.ExhaleVolumeVaranceThreshold = ExhaleVolumeVaranceThreshold;
            toData.ExhalePitchVaranceThreshold = ExhalePitchVaranceThreshold;
            toData.ExhalePitchNoiseCorrelation = ExhalePitchNoiseCorrelation;
            toData.ExhaleVolumeNoiseCorrelation = ExhaleVolumeNoiseCorrelation;
        }

        public void EmptyData()
        {
            ExhaleVolumeThreshold = 0f;
            ExhalePitchLowBound = 0f;
            ExhalePitchUpperBound = 0f;
            ExhaleVolumeVaranceThreshold = 0f;
            ExhalePitchVaranceThreshold = 0f ;
            ExhalePitchNoiseCorrelation = 0f;
            ExhaleVolumeNoiseCorrelation = 0f;
        }

    }

    public interface InhaleData
    {
        public float InhaleVolumeThreshold { get; set; }
        public float InhalePitchLowBound { get; set; }
        public float InhalePitchUpperBound { get; set; }
        public float InhaleLoudnessVarance { get; set; }
        public float InhalePitchNoiseCorrelation { get; set; }
        public float InhaleVolumeNoiseCorrelation { get; set; }
        public float InhalePitchOffset { get; }
        public float InhaleVolumeOffset { get; }

        public void CopyData(InhaleData toData)
        {
            toData.InhaleVolumeThreshold = InhaleVolumeThreshold;
            toData.InhalePitchLowBound = InhalePitchLowBound;
            toData.InhalePitchUpperBound = InhalePitchUpperBound;
            toData.InhaleLoudnessVarance = InhaleLoudnessVarance;
            toData.InhalePitchNoiseCorrelation = InhalePitchNoiseCorrelation;
            toData.InhaleVolumeNoiseCorrelation = InhaleVolumeNoiseCorrelation;
        }

        public void EmptyData()
        {
            InhaleVolumeThreshold = 0f;
            InhalePitchLowBound = 0f;
            InhalePitchUpperBound = 0f;
            InhaleLoudnessVarance = 0f;
            InhalePitchNoiseCorrelation = 0f;
            InhaleVolumeNoiseCorrelation = 0f;
        }

    }

    public interface OutputBreath
    {
        public BreathingStates PlayerBreathState();
    }
}