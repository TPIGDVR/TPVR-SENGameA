using PGGE.Patterns;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Breathing3
{
    public class BreathingDetection : MonoBehaviour,
        SilenceData,
        InhaleData,
        ExhaleData
    {
        //information about the volume currently
        VolumeProvider audioProvider;
        [Header("Silence")]
        [SerializeField] float silenceVolumeThreshold;
        [SerializeField] float silencePitchUpperBound;
        [SerializeField] float silencePitchLowerBound;
        [SerializeField] float silencePitchVaranceThreshold;
        [Header("Inhale")]
        [SerializeField] float inhaleVolumeThreshold;
        [SerializeField] float inhalePitchUpperBound;
        [SerializeField] float inhalePitchLowerBound;
        [SerializeField] float inhalePitchOffset;
        [SerializeField] float inhaleLoudnessVaranceThreshold;
        [SerializeField] float inhaleVolumeOffset;
        [Header("Exhale")]
        [SerializeField] float exhaleVolumethreshold;
        [SerializeField] float exhalePitchUpperBound;
        [SerializeField] float exhalePitchLowerBound;
        [SerializeField] float exhalePitchOffset;
        [SerializeField] float exhaleVolumeOffset;
        [SerializeField] float exhaleLoudnessVaranceThreshold;
        [SerializeField] float exhalePitchVaranceThreshold;


        [Header("Data Text")]
        public TextMeshProUGUI stateText;

        [Header("other settings")]
        [SerializeField] private float testTimer;
        public InhaleDataSO  presetInhaleData;
        public ExhaleDataSO presetExhaleData;
        public SilentDataSO PresetSilenceData;
        [SerializeField] private bool usedPresetData;
        
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
        #endregion

        FSM fsm;
        private void Start()
        {
            audioProvider = GetComponent<VolumeProvider>();

            fsm = new FSM();
            if (usedPresetData)
            {
                fsm.Add(new SilentState(fsm, (int)States.SILENT, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
                fsm.Add(new InhaleState(fsm, (int)States.INHALE, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
                fsm.Add(new ExhaleState(fsm, (int)States.EXHALE, audioProvider, presetInhaleData, presetExhaleData, PresetSilenceData));
            }
            else
            {
                fsm.Add(new SilentState(fsm, (int)States.SILENT, audioProvider, this, this, this));
                fsm.Add(new InhaleState(fsm, (int)States.INHALE, audioProvider, this, this, this));
                fsm.Add(new ExhaleState(fsm, (int)States.EXHALE, audioProvider, this, this, this));
            }
            //fsm.Add(new TestSilentState(fsm, (int)States.SILENT_TESTING, audioProvider, testTimer, this));
            //fsm.Add(new TestInhaleState(fsm, (int)States.INHALE_TESTING, audioProvider, testTimer, this));
            fsm.Add(new TestInhaleStateNew(fsm, (int)States.INHALE_TESTING, audioProvider, testTimer, this,this));
            fsm.Add(new TestExhaleState(fsm, (int)States.EXHALE_TESTING, audioProvider, testTimer, this));

            if (usedPresetData)
            {
                fsm.SetCurrentState((int)States.SILENT);
            }
            else
            {
                fsm.SetCurrentState((int)States.INHALE_TESTING);
            }
        }


        private void Update()
        {
            fsm.Update();
            stateText.text = $"Current state {(States) fsm.GetCurrentState().ID}";
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();
        }

        [ContextMenu("Testing")]
        public void Testing()
        {
            fsm.SetCurrentState((int)(States.SILENT_TESTING));
        }


        [ContextMenu("Testing new")]
        public void Testing2()
        {
            fsm.SetCurrentState((int)(States.INHALE_TESTING));
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
    }

    public interface SilenceData
    {
        public float SilenceVolumeThreshold { get; set; }
        public float SilencePitchLowBound { get; set; }
        public float SilencePitchUpperBound { get; set; }
        public float SilencePitchVaranceThreshold { get; set; }

        public void CopyData(SilenceData toData)
        {
            toData.SilencePitchLowBound = SilencePitchLowBound;
            toData.SilencePitchUpperBound = SilencePitchUpperBound;
            toData.SilenceVolumeThreshold = SilenceVolumeThreshold;
            toData.SilencePitchVaranceThreshold = SilencePitchVaranceThreshold;
        }
    }

    public interface ExhaleData
    {
        public float ExhaleVolumeThreshold { get; set; }
        public float ExhalePitchLowBound { get; set; }
        public float ExhalePitchUpperBound { get; set; }
        public float ExhaleVolumeVaranceThreshold { get; set; }
        public float ExhalePitchVaranceThreshold { get; set; }
        public float ExhalePitchOffset { get;  }
        public float ExhaleVolumeOffset { get; }


        public void CopyData(ExhaleData toData)
        {
            toData.ExhaleVolumeThreshold = ExhaleVolumeThreshold;
            toData.ExhalePitchLowBound = ExhalePitchLowBound;
            toData.ExhalePitchUpperBound = ExhalePitchUpperBound;
            toData.ExhaleVolumeVaranceThreshold = ExhaleVolumeVaranceThreshold;
            toData.ExhalePitchVaranceThreshold = ExhalePitchVaranceThreshold;
        }
    }

    public interface InhaleData
    {
        public float InhaleVolumeThreshold { get; set; }
        public float InhalePitchLowBound { get; set; }
        public float InhalePitchUpperBound { get; set; }
        public float InhaleLoudnessVarance { get; set; }
        public float InhalePitchOffset { get; }

        public float InhaleVolumeOffset { get; }

        public void CopyData(InhaleData toData)
        {
            toData.InhaleVolumeThreshold = InhaleVolumeThreshold;
            toData.InhalePitchLowBound = InhalePitchLowBound;
            toData.InhalePitchUpperBound = InhalePitchUpperBound;
            toData.InhaleLoudnessVarance = InhaleLoudnessVarance;
        }
    }


}