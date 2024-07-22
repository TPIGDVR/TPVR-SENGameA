using Caress.Examples;
using System;
using System.Collections;
using UnityEngine;

namespace Breathing3
{
    public class NoiseReducerInserter : MonoBehaviour
    {
        [SerializeField] double attuniation = 5f;
        public double Attuniation { get => attuniation; set => attuniation = value; }

        //get the audio clip from the microphone recorder.
        [SerializeField] private MicrophoneRecorder _microphoneRecorder = default;
        [SerializeField] private NoiseReducerHandler _noiseReducerHandler = default;
        [SerializeField] private AudioPlayer _audioPlayer = default;
        [SerializeField] bool useReducer;

        private void OnEnable() => _microphoneRecorder.OnAudioReady += OnRecorded;

        private void OnDisable() => _microphoneRecorder.OnAudioReady -= OnRecorded;

        private void OnRecorded(float[] pcm)
        {
            var original = new float[pcm.Length];
            //make sure to not destroy the original PCM.
            if (useReducer)
            {
                Array.Copy(pcm, original, pcm.Length);
                _noiseReducerHandler.ProcessPcm(original);
                _audioPlayer.ProcessBuffer(original, original.Length);
            }
            else
            {
                _audioPlayer.ProcessBuffer(pcm, original.Length);
            }
            
        }

        public void SetAtt()
        {
            _noiseReducerHandler.NoiseReducer.SetAttenuation(attuniation);
        }

    }
}