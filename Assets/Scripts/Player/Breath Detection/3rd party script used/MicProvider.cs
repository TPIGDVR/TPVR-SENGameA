using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace BreathDetection
{
    public class MicProvider : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSourceOrignal;

        public float[] _dataSpectrumContainer { get; private set; }
        public float[] _dataOutputContainer { get; private set; }
        private List<float> _pitchContainer;
        private List<float> _volumeContainer;
        [SerializeField] int _pitchRecordTime = 5;
        [SerializeField] int _volumeRecordTime = 5;
        [SerializeField] int _datalength = 1024;
        //mic settings

        // public bool mute = true;

        #region volume provider implementation
        public float volume { get; set; }
        public float pitch { get; set; }
        public float avgVolume { get; set; }
        public float avgPitch { get; set; }
        public float maxVolume { get; set; }
        public float minVolume { get; set; }
        public float maxPitch { get; set; }
        public float minPitch { get; set; }

        public float CalculatedPitch
        {
            get
            {
                switch (pitchCalculationMethod)
                {
                    case CalculationMethod.REAL_TIME:
                        return pitch;
                    case CalculationMethod.MIN:
                        return minPitch;
                    case CalculationMethod.MAX:
                        return maxPitch;
                    case CalculationMethod.AVG:
                        return avgPitch;
                    default:
                        return avgPitch;
                }
            }
        }

        public float CalculatedVolume
        {
            get
            {
                switch (pitchCalculationMethod)
                {
                    case CalculationMethod.REAL_TIME:
                        return volume;
                    case CalculationMethod.MIN:
                        return minVolume;
                    case CalculationMethod.MAX:
                        return maxVolume;
                    case CalculationMethod.AVG:
                        return avgVolume;
                    default:
                        return avgVolume;
                }
            }
        }

        public float CalculatedVolumeVariance { get; set; }

        public float CalculatePitchVariance { get; set; }
        public float PitchNoiseCorrelation { get; set; }
        public float VolumeNoiseCorrelation { get; set; }
        #endregion

        [Header("Other settings")]
        [SerializeField] private float HighPassCutoff = 10000;
        [SerializeField] private float LowPassCutoff = 100;
        [SerializeField] private bool pitchDebugger = false;
        [SerializeField] bool usedNoiseReducer = false;

        [SerializeField] private CalculationMethod pitchCalculationMethod = CalculationMethod.MIN;
        [SerializeField] private CalculationMethod volumeCalculationMethod = CalculationMethod.AVG;

        private float prevVolume = 0f;
        private float prevPitch = 0f;
        private const float refVal = 0.1f;

        public float pitchIncrementor => 24000f / _datalength;
        [SerializeField] TextMeshProUGUI text;
        enum CalculationMethod
        {
            REAL_TIME,
            MIN,
            MAX,
            AVG
        }

        private void Start()
        {
            _dataSpectrumContainer = new float[_datalength];
            _dataOutputContainer = new float[_datalength];
            _pitchContainer = new();
            _volumeContainer = new();

        }

        private void Update()
        {
            CalculateVolume();
            CalculatePitch();

            text.text = $"Volume: {volume}\n" +
                $"Min Volume: {minVolume}\n" +
                $"Max Volume: {maxVolume} \n" +
                $"Avg Volume: {avgVolume} \n" +
                $"Pitch: {pitch}\n" +
                $"Min pitch: {minPitch}\n" +
                $"Max pitch: {maxPitch} \n" +
                $"Avg pitch: {avgPitch}\n" +
                $"Cal pitch: {CalculatedPitch}\n" +
                $"Cal vol: {CalculatedVolume}\n" +
                $"Pitch correlation {PitchNoiseCorrelation}\n" +
                $"volume correlation {VolumeNoiseCorrelation}\n";
        }
        void CalculateVolume()
        {
            _audioSourceOrignal.GetOutputData(_dataOutputContainer, 0);

            CalculateNormalVolume();
            CalculateMaxMinAverageVolume();
            CalculateVolumeVariance();

            void CalculateNormalVolume()
            {
                float sum = 0;
                for (int i = 0; i < _dataOutputContainer.Length; i++)
                {
                    //all this are positive value.
                    //max is 1
                    sum += Mathf.Pow(_dataOutputContainer[i], 2);
                }
                //volume = Mathf.Sqrt(sum / _datalength) * loudnessMultiplier;
                if (sum == 0)
                {
                    volume = 0;
                    return;
                }


                volume = 20 * Mathf.Log10(Mathf.Sqrt(sum / _datalength) / refVal);
                //print($"{volume} , {sum}");
            }
            void CalculateMaxMinAverageVolume()
            {
                _volumeContainer.Add(volume);
                if (_volumeContainer.Count >= _volumeRecordTime)
                {
                    _volumeContainer.RemoveAt(0);
                }

                float minVol = float.MaxValue;
                float maxVol = float.MinValue;
                float avgVol = 0;

                foreach (var vol in _volumeContainer)
                {
                    if (vol < minVol)
                    {
                        minVol = vol;
                    }
                    if (vol > maxVol)
                    {
                        maxVol = vol;
                    }

                    avgVol += vol;
                }

                minVolume = minVol;
                maxVolume = maxVol;
                avgVolume = avgVol / _volumeContainer.Count;
            }
            void CalculateVolumeVariance()
            {
                CalculatedVolumeVariance = CalculatedVolume - prevVolume;
                prevVolume = CalculatedVolume;
            }
        }

        void CalculatePitch()
        {
            _audioSourceOrignal.GetSpectrumData(_dataSpectrumContainer, 0, FFTWindow.BlackmanHarris);
            CalculateNormalPitch();
            CalculateMinMaxAveragePitch();
            CalculationPitchVarance();

            void CalculateNormalPitch()
            {
                pitch = ReturnCommonFreq(_dataSpectrumContainer, LowPassCutoff, HighPassCutoff);

                float ReturnCommonFreq(float[] container, float lowPassFilter, float highPassFilter)
                {
                    int startingCheckingFrequencyIndex = 0;
                    int endingCheckingFrequencyIndex = _dataSpectrumContainer.Length;

                    startingCheckingFrequencyIndex = (int)(lowPassFilter / pitchIncrementor);
                    endingCheckingFrequencyIndex = (int)(highPassFilter / pitchIncrementor);
                    float maxV = 0;
                    int maxN = 0;

                    // Find the highest sample.
                    for (int i = startingCheckingFrequencyIndex; i < endingCheckingFrequencyIndex; i++)
                    {
                        if (_dataSpectrumContainer[i] > maxV)
                        {
                            maxV = container[i];
                            maxN = i; // maxN is the index of max
                        }
                    }
                    // Convert index to frequency
                    //24000 is the sampling frequency for the PC. 24000 / sample = frequency resolution
                    // frequency resolution * index of the sample would give the pitch as a result.
                    // Pass the index to a float variable
                    return (float)maxN * pitchIncrementor;

                }


                ////print(pitch);
                //if (pitchDebugger)
                //{
                //    print($"Pitch values {pitch} , freqN {freqN}, max N {maxV} " +
                //        $"pitch incredmental {pitchIncrementor} " +
                //        $"starting Index: {startingCheckingFrequencyIndex}" +
                //        $"Ending index: {endingCheckingFrequencyIndex}");
                //    //PrintArray(_dataSpectrumContainer);
                //}
            }

            void CalculateMinMaxAveragePitch()
            {
                _pitchContainer.Add(pitch);
                if (_pitchContainer.Count >= _pitchRecordTime)
                {
                    _pitchContainer.RemoveAt(0);
                }

                float minPitch = float.MaxValue;
                float maxPitch = float.MinValue;
                float avgPitch = 0;

                foreach (var pit in _pitchContainer)
                {
                    if (pit < minPitch)
                    {
                        minPitch = pit;
                    }
                    if (pit > maxPitch)
                    {
                        maxPitch = pit;
                    }

                    avgPitch += pit;
                }

                if (maxPitch == float.MinValue)
                {
                    Debug.Log("something is wrong");
                }

                this.minPitch = minPitch;
                this.maxPitch = maxPitch;
                this.avgPitch = avgPitch / _pitchContainer.Count;
            }

            void CalculationPitchVarance()
            {
                CalculatePitchVariance = CalculatedPitch - prevPitch;
                prevPitch = CalculatedPitch;
            }
        }
    }
}