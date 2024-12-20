﻿using System;
using UnityEngine;

namespace Caress.Examples
{
    public class NoiseReducedPlayback : MonoBehaviour
    {
        //get the audio clip from the microphone recorder.
        [SerializeField] private MicrophoneRecorder _microphoneRecorder = default;
        [SerializeField] private NoiseReducerHandler _noiseReducerHandler = default;
        [SerializeField] private AudioPlayer _noiseReducedAudioPlayer = default;
        [SerializeField] private AudioPlayer _audioPlayer = default;

        private void OnEnable() => _microphoneRecorder.OnAudioReady += OnRecorded;

        private void OnDisable() => _microphoneRecorder.OnAudioReady -= OnRecorded;

        private void OnRecorded(float[] pcm)
        {
            var original = new float[pcm.Length];
            Array.Copy(pcm, original, pcm.Length);
            //make sure to not destroy the original PCM.
            _noiseReducerHandler.ProcessPcm(pcm);
            _audioPlayer.ProcessBuffer(original, original.Length);
            _noiseReducedAudioPlayer.ProcessBuffer(pcm, pcm.Length);
        }
    }
}