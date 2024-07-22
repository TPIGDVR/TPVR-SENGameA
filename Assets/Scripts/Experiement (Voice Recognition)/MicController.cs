using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

namespace Breathing3
{
    [RequireComponent(typeof(AudioSource))]
    public class MicController : MonoBehaviour, VolumeProvider
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] AudioSource _audioSource;
        private AudioMixer aMixer;
        
        private float[] _dataContainer;
        private List<float> _pitchContainer;
        private List<float> _volumeContainer;
        [SerializeField] int _pitchRecordTime = 5;
        [SerializeField] int _volumeRecordTime = 5;
        [SerializeField] int _datalength = 1024;
        //mic settings
        private int maxFrequency = 44100;
        private int minFrequency = 0;
        public bool mute = true;

        [Header("Noise Reduction")]
        [SerializeField] float attuniation = 10f;


        [Header("Other settings")]
        [SerializeField] private float loudnessMultiplier = 100f; //Multiply loudness with this number
        [SerializeField] private float highPassCutoff = 10000;
        [SerializeField] private float LowPassCutoff = 100;
        [SerializeField] private bool pitchDebugger = false;

        [SerializeField] private CalculationMethod pitchCalculationMethod = CalculationMethod.MIN;
        [SerializeField] private CalculationMethod volumeCalculationMethod = CalculationMethod.AVG;
        [SerializeField] private string _micphoneName;
        private bool isMicrophoneReady;

        private float prevVolume =0f;
        private float prevPitch = 0f;

        private const float refVal = 0.1f;

        enum CalculationMethod
        {
            REAL_TIME,
            MIN,
            MAX,
            AVG
        }


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
        public float PitchNoiseCorrelation { get ; set; }
        public float VolumeNoiseCorrelation { get; set ; }
        #endregion

        IEnumerator Start()
        {

            aMixer = Resources.Load("MicrophoneMixer") as AudioMixer;
            if (mute)
            {
                aMixer.SetFloat("MicrophoneVolume", -80);
            }
            else
            {
                aMixer.SetFloat("MicrophoneVolume", 0);
            }


            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("No microphone detected.");
            }


            //if using Android or iOS -> request microphone permission
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

                if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
                {
                    Debug.LogWarning("Application does not have microphone permission.");
                    yield break;
                }
            }

            prepareMicrophone();
            
            _audioSource.playOnAwake = false;

            _dataContainer = new float[_datalength];
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
                $"Cal vol: {CalculatedVolume}";
        }

        void prepareMicrophone()
        {
            if (Microphone.devices.Length > 0)
            {
                //Gets the maxFrequency and minFrequency of the device
                _micphoneName = Microphone.devices[0];
                Microphone.GetDeviceCaps(_micphoneName, out minFrequency, out maxFrequency);
                if (maxFrequency == 0)
                {//These 2 kioskData of code are mainly for windows computers
                    maxFrequency = 44100;
                }
                if (_audioSource.clip == null)
                {
                    _audioSource.clip = Microphone.Start(_micphoneName, true, 1, maxFrequency);
                    _audioSource.loop = true;

                    //Wait until microphone starts recording
                    while (!(Microphone.GetPosition(_micphoneName) > 0))
                    {
                    }
                }
                _audioSource.Play();
                isMicrophoneReady = true;

            }
            else
            {
                Debug.LogWarning("No microphone detected.");
            }

        }

        void CalculateVolume()
        {
            _audioSource.GetOutputData(_dataContainer, 0);
            CalculateNormalVolume();
            CalculateMaxMinAverageVolume();
            CalculateVolumeVariance();

            void CalculateVolumeVariance()
            {
                CalculatedVolumeVariance = CalculatedVolume - prevVolume;
                prevVolume = CalculatedVolume;
            }



            void CalculateNormalVolume()
            {
                float sum = 0;
                for (int i = 0; i < _dataContainer.Length; i++)
                {
                    sum += Mathf.Pow(_dataContainer[i], 2);//Mathf.Abs(dataContainer[i]);
                }
                //volume = Mathf.Sqrt(sum / _datalength) * loudnessMultiplier;
                if(sum == 0)
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
                if(_volumeContainer.Count >= _volumeRecordTime)
                {
                    _volumeContainer.RemoveAt(0);
                }

                float minVol = float.MaxValue;
                float maxVol = float.MinValue;
                float avgVol = 0;

                foreach(var vol in _volumeContainer)
                {
                    if(vol < minVol)
                    {
                        minVol = vol;
                    }
                    if(vol > maxVol)
                    {
                        maxVol = vol;
                    }

                    avgVol += vol;
                }

                minVolume = minVol;
                maxVolume = maxVol;
                avgVolume = avgVol / _volumeContainer.Count;
            }
        }

        void CalculatePitch()
        {
            _audioSource.GetSpectrumData(_dataContainer, 0, FFTWindow.BlackmanHarris);
            CalculateNormalPitch();
            CalculateMinMaxAveragePitch();
            CalculationPitchVarance();

            void CalculateNormalPitch()
            {
                int startingCheckingFrequencyIndex = 0;
                int endingCheckingFrequencyIndex = _dataContainer.Length;
                float pitchIncrementor = 24000f / _datalength;
                startingCheckingFrequencyIndex = (int)(LowPassCutoff / pitchIncrementor);
                endingCheckingFrequencyIndex = (int) (highPassCutoff / pitchIncrementor);
                float maxV = 0;
                int maxN = 0;

                // Find the highest sample.
                for (int i = startingCheckingFrequencyIndex; i < endingCheckingFrequencyIndex; i++)
                {
                    if (_dataContainer[i] > maxV)
                    {
                        maxV = _dataContainer[i];
                        maxN = i; // maxN is the index of max
                    }
                }

                // Pass the index to a float variable
                float freqN = maxN;

                // Convert index to frequency
                //24000 is the sampling frequency for the PC. 24000 / sample = frequency resolution
                // frequency resolution * index of the sample would give the pitch as a result.
                //pitch = HighPassFilter(freqN * pitchIncrementor, HighPassCutoff);
                pitch = freqN * pitchIncrementor;
                //print(pitch);
                if (pitchDebugger)
                {
                    print($"Pitch values {pitch} , freqN {freqN}, max N {maxV} " +
                        $"pitch incredmental {pitchIncrementor} " +
                        $"starting Index: {startingCheckingFrequencyIndex}" +
                        $"Ending index: {endingCheckingFrequencyIndex}");
                    //PrintArray(_dataSpectrumContainer);

                }
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

                if(maxPitch == float.MinValue)
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

        void PrintArray(float[] array)
        {
            string arrayString = "";
            for (int i = 0; i < array.Length; i++)
            {
                arrayString += array[i].ToString("F4") + " "; // Format to 4 decimal places for readability
            }
            Debug.Log(arrayString);
        }
    }


    public interface VolumeProvider
    {
        float volume { get; set; }
        float pitch { get; set; }
        float avgVolume { get; set; }
        float avgPitch { get; set; }
        float maxVolume { get; set; }
        float minVolume { get; set; }
        float maxPitch { get; set; }
        float minPitch { get; set; }
        float CalculatedPitch { get; }
        float CalculatedVolume { get; }
        float CalculatedVolumeVariance { get; set; }
        float CalculatePitchVariance { get; set; }

        float PitchNoiseCorrelation { get; set; }
        float VolumeNoiseCorrelation { get; set; }
    }
}