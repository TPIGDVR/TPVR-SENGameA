using System;

namespace BreathDetection
{
    public interface IBreathResult
    {
        public bool Result();
    }

    

    [Serializable]
    public struct InhaleData
    {
        public float lowPassFilter;
        public float highPassFilter;
        public int minNumberOfCommonPoint;
        public int maxNumberOfCommonPoint;
        public float minAmplitude;
        public float maxAmplitude;
        public float maxDBThreshold;
        public int inhaleCounterThreshold;
        //reduce the minnumber of commonpoint when it detects inhaling
        public int reductionOfMinCounter; 
    }

    [Serializable]
    public struct ExhaleData
    {
        public float lowPassFilter;
        public float highPassFilter;
        public float volumeThreshold;
        public float minPitchThreshold;
        public float maxPitchThreshold;
    }


}