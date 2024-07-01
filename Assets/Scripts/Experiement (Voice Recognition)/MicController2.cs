namespace Breathing
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Audio;
    using System.Collections.Generic;
    using System;

    public class MicController2 : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        public int samplesSize = 1024;
        private int maxFrequency = 44100;
        private int minFrequency = 0;
        public bool mute = true;
        [HideInInspector]
        public float loudness;
        [SerializeField] private float loudnessMultiplier = 10.0f; //Multiply loudness with this number

        //stores all the data for spectrum, volume.
        public static float[] dataContainer;

        private bool isMicrophoneReady = false;
        private AudioMixer aMixer;

        public float highPassCutoff = 10000; //Ignores all frequencies above this value
        private float pitchValue;
        private List<float> pastPitches;
        [HideInInspector]
        public int pitchRecordTime = 5;
        private float averagePitch;

        private bool UseFFTCentroid;
        private float centroidValue;

        private bool EnableSavingOfRecordedAudio;

        private float maxPitch = 0.0f; //Delete this, its just for testing

        //for calculating the pitch and volume of a target volume/pitch
        private string microphoneDefaultName;
        public bool IsMicrophoneReady { get => isMicrophoneReady; set => isMicrophoneReady = value; }
        public bool IsScriptRunned { get; private set; } = false;

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

            dataContainer = new float[samplesSize];
            pastPitches = new List<float>();
            IsScriptRunned = true;
        }

        void FixedUpdate()
        {
            if (isMicrophoneReady)
            {
                print("mic working");
                loudness = CalculateLoudness();

                if (UseFFTCentroid)
                {
                    calculateFFTCentroid();
                }
                else
                {
                    calculatePitch();
                }
            }
        }

        void prepareMicrophone()
        {
            if (Microphone.devices.Length > 0)
            {
                //Gets the maxFrequency and minFrequency of the device
                Microphone.GetDeviceCaps(Microphone.devices[0], out minFrequency, out maxFrequency);
                if (maxFrequency == 0)
                {//These 2 lines of code are mainly for windows computers
                    maxFrequency = 44100;
                }
                if (audioSource.clip == null)
                {
                    audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, maxFrequency);
                    audioSource.loop = true;

                    //Wait until microphone starts recording
                    while (!(Microphone.GetPosition(Microphone.devices[0]) > 0))
                    {
                    }
                }

                microphoneDefaultName = Microphone.devices[0];

                audioSource.Play();
                isMicrophoneReady = true;

            }
            else
            {
                Debug.LogWarning("No microphone detected.");
            }

        }

        void calculatePitch()
        {
            // Gets the sound spectrum.
            audioSource.GetSpectrumData(dataContainer, 0, FFTWindow.BlackmanHarris);
            float maxV = 0;
            int maxN = 0;

            // Find the highest sample.
            for (int i = 0; i < dataContainer.Length; i++)
            {
                if (dataContainer[i] > maxV)
                {
                    maxV = dataContainer[i];
                    maxN = i; // maxN is the index of max
                }
            }

            // Pass the index to a float variable
            float freqN = maxN;

            // Convert index to frequency
            //24000 is the sampling frequency for the PC. 24000 / sample = frequency resolution
            // frequency resolution * index of the sample would give the pitch as a result.
            pitchValue = HighPassFilter(freqN * 24000 / samplesSize, highPassCutoff);
            updatePastPitches(pitchValue);

            //update the max pitch
            if (pitchValue > maxPitch)
            {
                maxPitch = pitchValue;
                //Debug.Log ("MaxPitch: " + maxPitch);

            }
            /*	if (pitchValue > 750 && pitchValue < 3000) {
                    Debug.Log ("Pitch could be exhale");
                }*/
        }

        void calculateFFTCentroid()
        {
            audioSource.GetSpectrumData(dataContainer, 0, FFTWindow.BlackmanHarris);

            float centroid = 0.0f;
            float fftSum = 0.0f;
            float weightedSum = 0.0f;

            for (int i = 0; i < dataContainer.Length / 2; i++)
            {
                fftSum += dataContainer[i];
                weightedSum += dataContainer[i] * i * 24000 / samplesSize;
            }

            pitchValue =/*(24000 / samplesSize) * */(weightedSum / fftSum);
            updatePastPitches(pitchValue);

            Debug.Log("Centroid: " + pitchValue);
        }

        //https://discussions.unity.com/t/getoutputdata-and-getspectrumdata-what-represent-the-values-returned/27063/2
        /// <summary>
        /// GetSpectrumData returns a array of float that contains the amplitude of the the freqency
        /// The frequency (Hz) is represented by the index of the array. THe amplitude shows the
        /// most common frequency in the spectrum.
        /// It appear u need to call this every frame to get the average min and max pitch.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="minPitchVal"></param>
        /// <param name="maxPitchVal"></param>
        /// 


        //need to call this before u can call the other pitch related methods
        public void CalculateSpectrumDataFromRecording()
        {
            audioSource.GetSpectrumData(dataContainer, 0, FFTWindow.BlackmanHarris);
        }

        public float CalculateMaxPitchFromRecording(int startindex = 0)
        {
            float firstMax = float.NegativeInfinity;
            float firstMaxValue = float.NegativeInfinity;
            //PrintArray(dataContainer);

            for (int i = startindex; i < dataContainer.Length; i++)
            {
                var val = dataContainer[i];
                if (val > firstMaxValue)
                {
                    firstMax = i;
                    firstMaxValue = val;
                }
            }

            return HighPassFilter((float)firstMax * 24000 / samplesSize, highPassCutoff);
        }

        public float CalculateMinPitchFromRecording(float amplitudeThreshold = 0f, int ignoreIndexBelow = 0)
        {
            int selectedFrequency = -1;
            //PrintArray(dataContainer);
            //try get lowest frequency and have 
            for (; ignoreIndexBelow < dataContainer.Length; ignoreIndexBelow++)
            {

                if (dataContainer[ignoreIndexBelow] >= amplitudeThreshold)
                {
                    selectedFrequency = ignoreIndexBelow;
                    break;
                }
            }

            //print(selectedFrequency);

            return HighPassFilter((float)selectedFrequency * 24000 / samplesSize, highPassCutoff);
        }
        #region recording

        [ContextMenu("startRecording")]
        public void StartRecording()
        {
            isMicrophoneReady = false;
            //so the other script can access the data and calculate the data
            //manually
        }

        [ContextMenu("stopRecording")]
        public void StopRecording()
        {
            isMicrophoneReady = true;
            //return recordedClip;
        }

        #endregion

        float HighPassFilter(float pitch, float cutOff)
        {
            if (pitch > cutOff)
            {
                return 0;
            }
            else
            {
                return pitch;
            }
        }

        /// <summary>
        /// Add a pitch to the array
        /// </summary>
        /// <param name="newPitch">the pitch that is added</param>
        void updatePastPitches(float newPitch)
        {
            if (pastPitches.Count > pitchRecordTime)
            {
                pastPitches.RemoveAt(0);
            }

            averagePitch = 0;
            foreach (float num in pastPitches)
            {
                averagePitch += num;
            }
            averagePitch /= pastPitches.Count;
            //Debug.Log ("Average pitch: " + averagePitch);
        }


        /// <summary>
        /// Get data from a the source
        /// </summary>
        /// <returns></returns>
       public float CalculateLoudness()
        {
            float sum = 0;
            audioSource.GetOutputData(dataContainer, 0);
            for (int i = 0; i < dataContainer.Length; i++)
            {
                sum += Mathf.Pow(dataContainer[i], 2);//Mathf.Abs(dataContainer[i]);
            }

            return Mathf.Sqrt(sum / samplesSize) * loudnessMultiplier;
        }

        #region getter
        public float getPitch()
        {
            return pitchValue;
        }

        public float getAveragePitch()
        {
            return averagePitch;
        }

        public float getCentroid()
        {
            return centroidValue;
        }
        #endregion

        void PrintArray(float[] array)
        {
            string arrayString = string.Join(", ", array);
            // Print the string to the console
            Debug.Log(arrayString);
        }
    }
}