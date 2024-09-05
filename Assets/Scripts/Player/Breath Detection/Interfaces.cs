using System;
using UnityEngine;

namespace BreathDetection
{
    /// <summary>
    /// An interface for determining a result of the breath. Used
    /// for innhale and exhale breath detection
    /// </summary>
    public interface IBreathResult
    {
        /// <summary>
        /// The return is the result if it is successful or not. 
        /// ETC, if inhale is recognise > it will be true. elase false
        /// </summary>
        /// <returns>the value of the IBreathableResult.</returns>
        public bool Result();
    }

    /// <summary>
    /// Used for testing the data
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public interface ITestable<DataType>
    {
        public void Run();
        public void Reset(DataType templateData);
        public DataType Calculate();

    }

    [Serializable]
    public struct SpectrumData
    {
        public float lowPassFilter;
        public float highPassFilter;
        public int minNumberOfCommonPoint;
        public int maxNumberOfCommonPoint;
        public float minAmplitude;
        public float maxAmplitude;
        public float maxDBThreshold;
        public int CounterThreshold;
        //reduce the minnumber of commonpoint when it detects inhaling
        public int reductionOfMinCounter;
        //for experiemental;
        [Range(1, 15)]
        public int numberOfPointsToStop;
    }

    [Serializable]
    public struct LoudnessData
    {
        public float lowPassFilter;
        public float highPassFilter;
        public float volumeThreshold;
        public float minPitchThreshold;
        public float maxPitchThreshold;
    }
}