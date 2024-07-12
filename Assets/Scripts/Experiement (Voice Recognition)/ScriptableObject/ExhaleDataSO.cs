using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "Exhale data", menuName = "breathing Data/Exhale data")]

    public class ExhaleDataSO : ScriptableObject, ExhaleData
    {
        [SerializeField]private float exhaleVolumeThreshold;
        [SerializeField]private float exhalePitchLowBound;
        [SerializeField]private float exhalePitchUpperBound;
        [SerializeField]private float exhalePitchOffset;
        [SerializeField]private float exhaleVolumeOffset;
        [SerializeField]private float exhaleVolumeVaranceThreshold;
        [SerializeField]private float exhalePitchVaranceThreshold;
        [SerializeField] private float exhalePitchNoiseCorrelation;
        public float ExhaleVolumeThreshold { get => exhaleVolumeThreshold; set => exhaleVolumeThreshold = value; }
        public float ExhalePitchLowBound { get => exhalePitchLowBound; set => exhalePitchLowBound = value; }
        public float ExhalePitchUpperBound { get => exhalePitchUpperBound; set => exhalePitchUpperBound = value; }
        public float ExhalePitchOffset { get => exhalePitchOffset; set => exhalePitchOffset = value; }
        public float ExhaleVolumeOffset { get => exhaleVolumeOffset; set => exhaleVolumeOffset = value; }
        public float ExhaleVolumeVaranceThreshold { get => exhaleVolumeVaranceThreshold; set => exhaleVolumeVaranceThreshold = value; }
        public float ExhalePitchVaranceThreshold { get => exhalePitchVaranceThreshold; set => exhalePitchVaranceThreshold = value; }
        public float ExhalePitchNoiseCorrelation { get => exhalePitchNoiseCorrelation; set => exhalePitchNoiseCorrelation = value; }
        public float ExhaleVolumeNoiseCorrelation { get => exhalePitchNoiseCorrelation; set => exhalePitchNoiseCorrelation = value; }
    }
}