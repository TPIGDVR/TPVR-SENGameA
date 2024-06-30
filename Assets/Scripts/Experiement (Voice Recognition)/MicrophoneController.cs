namespace Breathing
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Audio;
    using System.Collections.Generic;
    using TMPro;
    using Unity.Mathematics;
    using UnityEditor.ShaderGraph;
    using static Unity.VisualScripting.Member;
    using System;

    public class MicrophoneController : MonoBehaviour
    {

        [SerializeField]private AudioSource aSource;
        public int samples = 1024;
        private int maxFrequency = 44100;
        private int minFrequency = 0;
        public bool mute = true;
        [HideInInspector]
        public float loudness;
        [SerializeField] private float loudnessMultiplier = 10.0f; //Multiply loudness with this number

        private float[] fftSpectrum;

        private bool isMicrophoneReady = false;
        private AudioMixer aMixer;

        public float highPassCutoff; //Ignores all frequencies above this value
        private float pitchValue;
        private List<float> pastPitches;
        [HideInInspector]
        public int pitchRecordTime = 5;
        private float averagePitch;

        private bool UseFFTCentroid;
        private float centroidValue;

        private bool EnableSavingOfRecordedAudio;

        private float maxPitch = 0.0f; //Delete this, its just for testing

        private AudioClip recordedClip = null;
        //[SerializeField] TextMeshProUGUI dataText;

        //for calculating the pitch and volume of a target volume/pitch
        [SerializeField] private AudioSource calculatingSource;
        private AudioSource testingClipSource;
        private Coroutine currentCoroutine;

        string microphoneDefaultName { get { return Microphone.devices[0]; } }

        public float[] FftSpectrum { get => fftSpectrum; }

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



            testingClipSource = gameObject.AddComponent<AudioSource>();
            testingClipSource.playOnAwake = false;

            fftSpectrum = new float[samples];
            pastPitches = new List<float>();
        }

        //private void Update()
        //{
        //    dataText.text = $"Data \n" +
        //            $"Pitch: {(int)pitchValue} ";
        //}

        void FixedUpdate()
        {
            if (isMicrophoneReady)
            {
                loudness = calculateLoudness();

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

        void OnGUI()
        {
            if (EnableSavingOfRecordedAudio)
            {
                if (GUI.Button(new Rect(10, 10, 50, 50), "Save"))
                {
                    SaveRecordedAudio();
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
                if (aSource.clip == null)
                {
                    aSource.clip = Microphone.Start(Microphone.devices[0], true, 1, maxFrequency);
                    aSource.loop = true;

                    //Wait until microphone starts recording
                    while (!(Microphone.GetPosition(Microphone.devices[0]) > 0))
                    {
                    }
                }
                aSource.Play();
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
            aSource.GetSpectrumData(fftSpectrum, 0, FFTWindow.BlackmanHarris);
            float maxV = 0;
            int maxN = 0;

            // Find the highest sample.
            for (int i = 0; i < fftSpectrum.Length; i++)
            {
                if (fftSpectrum[i] > maxV)
                {
                    maxV = fftSpectrum[i];
                    maxN = i; // maxN is the index of max
                }
            }

            // Pass the index to a float variable
            float freqN = maxN;

            // Convert index to frequency
            //24000 is the sampling frequency for the PC. 24000 / sample = frequency resolution
            // frequency resolution * index of the sample would give the pitch as a result.
            pitchValue = HighPassFilter(freqN * 24000 / samples, highPassCutoff);
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
            aSource.GetSpectrumData(fftSpectrum, 0, FFTWindow.BlackmanHarris);

            float centroid = 0.0f;
            float fftSum = 0.0f;
            float weightedSum = 0.0f;

            for (int i = 0; i < fftSpectrum.Length / 2; i++)
            {
                fftSum += fftSpectrum[i];
                weightedSum += fftSpectrum[i] * i * 24000 / samples;
            }

            pitchValue =/*(24000 / samples) * */(weightedSum / fftSum);
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
            calculatingSource.GetSpectrumData(fftSpectrum, 0, FFTWindow.BlackmanHarris);
        }
        public float CalculateMaxPitchFromRecording()
        {
            //not too sure if this safe performance if I only
            //compute the spectrum data once and use accross different areas.
            if(calculatingSource.clip == null)
            {
                throw new Exception("You cant calculate a pitch as it is not recording");
            }

;            float firstMax = 0;
            float firstMaxValue = fftSpectrum[0];

            for(int i = 1;i < fftSpectrum.Length ; i++)
            {
                var val = fftSpectrum[i];
                if (val > firstMaxValue)
                {
                    firstMax = i;
                    firstMaxValue = val;
                }
            }

            return HighPassFilter((float)firstMax * 24000 / samples, highPassCutoff);
        }

        public float CalculateMinPitchFromRecording(float amplitudeThreshold = 0f , int ignoreIndexBelow = 0)
        {
            if (calculatingSource.clip == null)
            {
                throw new Exception("You cant calculate a pitch as it is not recording");
            }

            int selectedFrequency = -1;
            //PrintArray(fftSpectrum);
            //try get lowest frequency and have 
            for (; ignoreIndexBelow < fftSpectrum.Length; ignoreIndexBelow++)
            {
                
                if(fftSpectrum[ignoreIndexBelow] >= amplitudeThreshold)
                {
                    selectedFrequency = ignoreIndexBelow;
                    break;
                }
            }

            //print(selectedFrequency);

            return HighPassFilter((float)selectedFrequency * 24000 / samples, highPassCutoff);
        }

        public float CalculateVolumeFromRecording()
        {
            if (calculatingSource.clip == null)
            {
                throw new Exception("You cant calculate a pitch as it is not recording");
            }

            float[] microphoneData = new float[samples];
            float sum = 0;

            calculatingSource.GetOutputData(microphoneData, 0);
            for (int i = 0; i < microphoneData.Length; i++)
            {
                sum += Mathf.Pow(microphoneData[i], 2);//Mathf.Abs(microphoneData[i]);
            }

            return Mathf.Sqrt(sum / samples) * loudnessMultiplier;
        }

        public void TestClipFromRecord(AudioClip clip)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            testingClipSource.clip = clip;
            testingClipSource.Play();
            currentCoroutine = StartCoroutine(RemoveClipOnceItIsDone());
        }

        IEnumerator RemoveClipOnceItIsDone()
        {
            while (testingClipSource.isPlaying)
            {
                yield return null;
            }
            testingClipSource.clip=null;
        }

        #region recording

        [ContextMenu("startRecording")]
        public void StartRecording()
        {
            isMicrophoneReady = false;

            if (Microphone.IsRecording(microphoneDefaultName))
            {
                aSource.Stop();
                aSource.clip = null;
                Microphone.End(microphoneDefaultName);
            }


            calculatingSource.clip = Microphone.Start(microphoneDefaultName, true, 1, maxFrequency);
            calculatingSource.loop = true;
            //wait for the microphone to get ready
            while (!(Microphone.GetPosition(Microphone.devices[0]) > 0))
            {
            }

            //this is so that any script can start calculating the source pitch and volume
            calculatingSource.Play();
        }

        public void StartRecording(int seconds)
        {
            if (Microphone.IsRecording(microphoneDefaultName))
            {
                aSource.Stop();
                aSource.clip = null;
                Microphone.End(microphoneDefaultName);
                isMicrophoneReady = false;
            }
            recordedClip = Microphone.Start(microphoneDefaultName, true, 1, maxFrequency);

            //wait for the microphone to get ready
            while (!(Microphone.GetPosition(Microphone.devices[0]) > 0))
            {
            }

            calculatingSource.clip = recordedClip;
            calculatingSource.Play();
        }


        [ContextMenu("stopRecording")]
        public void StopRecording()
        {
            if (Microphone.IsRecording(microphoneDefaultName))
            {
                Microphone.End(microphoneDefaultName);
            }
            calculatingSource.Stop();
            calculatingSource.clip = null;
            recordedClip = null;
            prepareMicrophone();
            //return recordedClip;
        }

        [ContextMenu("restart recording")]
        public void RestartRecording()
        {
            prepareMicrophone();
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
            pastPitches.Add(newPitch);

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
        float calculateLoudness()
        {
            float[] microphoneData = new float[samples];
            float sum = 0;

            aSource.GetOutputData(microphoneData, 0);
            for (int i = 0; i < microphoneData.Length; i++)
            {
                sum += Mathf.Pow(microphoneData[i], 2);//Mathf.Abs(microphoneData[i]);
            }

            return Mathf.Sqrt(sum / samples) * loudnessMultiplier;
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
        void SaveRecordedAudio()
        {
            //EditorUtility.ExtractOggFile (GameObject.Find("TestAudioSource").GetComponent<AudioSource>().clip, Application.streamingAssetsPath);
        }

        void PrintArray(float[] array)
        {
            string arrayString = string.Join(", ", array);
            // Print the string to the console
            Debug.Log(arrayString);
        }
    }

}