using System.Collections;
using UnityEngine;

namespace Breathing3
{
    [CreateAssetMenu(fileName = "silence data", menuName = "breathing Data/Silence data")]

    public class SilentDataSO : ScriptableObject, SilenceData
    {

        [SerializeField] private float silenceVolumeThreshold;
        [SerializeField]private float silencePitchLowBound;
        [SerializeField]private float silencePitchUpperBound;
        [SerializeField]private float silencePitchVaranceThreshold;
        [SerializeField] private float silenceVolumeOffset;

        public float SilenceVolumeThreshold { get => silenceVolumeThreshold; set => silenceVolumeThreshold = value; }
        public float SilencePitchLowBound { get => silencePitchLowBound; set => silencePitchLowBound = value; }
        public float SilencePitchUpperBound { get => silencePitchUpperBound; set => silencePitchUpperBound = value; }
        public float SilencePitchVaranceThreshold { get => silencePitchVaranceThreshold; set => silencePitchVaranceThreshold = value; }
        public float SilenceVolumeOffset { get => silenceVolumeOffset; set => silenceVolumeOffset = value; }
    }
}