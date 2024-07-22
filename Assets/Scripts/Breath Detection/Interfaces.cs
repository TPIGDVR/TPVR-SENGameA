using System;
using UnityEngine;

namespace BreathDetection
{
    public interface IBreathResult
    {
        public bool Result();
    }

    public interface ITestable<DataType>
    {
        public void Run();
        public void Reset(DataType templateData);
        public DataType Calculate();

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

        //for experiemental;
        [Range(1, 15)]
        public int numberOfPointsToStop;
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