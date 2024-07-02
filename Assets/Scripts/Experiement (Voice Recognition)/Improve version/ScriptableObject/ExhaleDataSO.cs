using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "Exhale data", menuName = "breathing Data/Exhale data")]

    public class ExhaleDataSO : ScriptableObject, ExhaleData
    {
        private float exhaleVolumeThreshold;
        private float exhalePitchLowBound;
        private float exhalePitchUpperBound;
        private float exhalePitchOffset;
        private float exhaleVolumeOffset;

        public float ExhaleVolumeThreshold { get => exhaleVolumeThreshold; set => exhaleVolumeThreshold = value; }
        public float ExhalePitchLowBound { get => exhalePitchLowBound; set => exhalePitchLowBound = value; }
        public float ExhalePitchUpperBound { get => exhalePitchUpperBound; set => exhalePitchUpperBound = value; }
        public float ExhalePitchOffset { get => exhalePitchOffset; set => exhalePitchOffset = value; }
        public float ExhaleVolumeOffset { get => exhaleVolumeOffset; set => exhaleVolumeOffset = value; }
    }
}