
namespace Breathing3
{
    using Caress.Examples;
    using UnityEngine;
    public class AudioPlayerBack : MonoBehaviour
    {
        [SerializeField] private MicrophoneRecorder _microphoneRecorder = default;
        [SerializeField] private NoiseReducerHandler _noiseReducerHandler = default;
        [SerializeField] private AudioPlayer _audioPlayer = default;

        private void OnEnable() => _microphoneRecorder.OnAudioReady += OnRecorded;

        private void OnDisable() => _microphoneRecorder.OnAudioReady -= OnRecorded;

        private void OnRecorded(float[] pcm)
        {
            _noiseReducerHandler.ProcessPcm(pcm);
            _audioPlayer.ProcessBuffer(pcm, pcm.Length);
        }
    }
}