using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "silence data", menuName = "breathing Data/Silence data")]

    public class SilentDataSO : ScriptableObject, SilenceData
    {

        private float silenceVolumeThreshold;
        private float silencePitchLowBound;
        private float silencePitchUpperBound;
        private float silencePitchVaranceThreshold;

        public float SilenceVolumeThreshold { get => silenceVolumeThreshold; set => silenceVolumeThreshold = value; }
        public float SilencePitchLowBound { get => silencePitchLowBound; set => silencePitchLowBound = value; }
        public float SilencePitchUpperBound { get => silencePitchUpperBound; set => silencePitchUpperBound = value; }
        public float SilencePitchVaranceThreshold { get => silencePitchVaranceThreshold; set => silencePitchVaranceThreshold = value; }
    }
}