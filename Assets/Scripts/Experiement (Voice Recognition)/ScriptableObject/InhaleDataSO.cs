using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "Inhale data", menuName = "breathing Data/Inhale data")]
    public class InhaleDataSO : ScriptableObject, InhaleData
    {
        [SerializeField] private float inhaleVolumeThreshold;
        [SerializeField]private float inhalePitchLowBound;
        [SerializeField]private float inhalePitchUpperBound;
        [SerializeField]private float inhaleLoudnessVarance;
        [SerializeField]private float inhalePitchOffset;
        [SerializeField]private float inhaleVolumeOffset;
        [SerializeField] private float inhalePitchNoiseCorrelation;
        [SerializeField] private float inhaleVolumeNoiseCorrelation;
        public float InhaleVolumeThreshold { get => inhaleVolumeThreshold; set => inhaleVolumeThreshold = value; }
        public float InhalePitchLowBound { get => inhalePitchLowBound; set => inhalePitchLowBound = value; }
        public float InhalePitchUpperBound { get => inhalePitchUpperBound; set => inhalePitchUpperBound = value; }
        public float InhaleLoudnessVarance { get => inhaleLoudnessVarance; set => inhaleLoudnessVarance = value; }
        public float InhalePitchOffset { get => inhalePitchOffset; set => inhalePitchOffset = value; }
        public float InhaleVolumeOffset { get => inhaleVolumeOffset; set => inhaleVolumeOffset = value; }
        public float InhalePitchNoiseCorrelation { get => inhalePitchNoiseCorrelation; set => inhalePitchNoiseCorrelation = value; }
        public float InhaleVolumeNoiseCorrelation { get => inhaleVolumeNoiseCorrelation; set => inhaleVolumeNoiseCorrelation = value; }
    }
}