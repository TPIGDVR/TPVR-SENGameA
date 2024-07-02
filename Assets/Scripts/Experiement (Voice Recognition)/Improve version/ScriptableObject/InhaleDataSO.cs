using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "Inhale data", menuName = "breathing Data/Inhale data")]
    public class InhaleDataSO : ScriptableObject, InhaleData
    {
        private float inhaleVolumeThreshold;
        private float inhalePitchLowBound;
        private float inhalePitchUpperBound;
        private float inhaleLoudnessVarance;
        private float inhalePitchOffset;
        private float inhaleVolumeOffset;

        public float InhaleVolumeThreshold { get => inhaleVolumeThreshold; set => inhaleVolumeThreshold = value; }
        public float InhalePitchLowBound { get => inhalePitchLowBound; set => inhalePitchLowBound = value; }
        public float InhalePitchUpperBound { get => inhalePitchUpperBound; set => inhalePitchUpperBound = value; }
        public float InhaleLoudnessVarance { get => inhaleLoudnessVarance; set => inhaleLoudnessVarance = value; }
        public float InhalePitchOffset { get => inhalePitchOffset; set => inhalePitchOffset = value; }
        public float InhaleVolumeOffset { get => inhaleVolumeOffset; set => inhaleVolumeOffset = value; }
    }
}