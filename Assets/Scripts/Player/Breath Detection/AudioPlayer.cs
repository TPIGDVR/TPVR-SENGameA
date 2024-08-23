using System;
using UnityEngine;

namespace BreathDetection
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private const NumChannels Channels = NumChannels.Mono;
        private const SampleRate sampleRate = SampleRate._48000;
        private const int AudioClipLength = 1024 * 6;
        private AudioSource _source;
        private int _clipHead;
        private float[] _audioClipData;

        private void OnEnable()
        {
            _source = GetComponent<AudioSource>();
            _source.clip = AudioClip.Create("buffer", AudioClipLength, (int) Channels, (int) sampleRate, false);
            _source.loop = true;
        }

        private void OnDisable()
        {
            _source.Stop();
            _source.clip = null;
        }

        /// <summary>
        /// will basiacally read through the audio clip frequency data and 
        /// overwrite the current clip.
        /// </summary>
        /// <param name="pcm"></param>
        /// <param name="pcmLength"></param>
        public void ProcessBuffer(float[] pcm, int pcmLength)
        {
            if (_audioClipData == null || _audioClipData.Length != pcmLength)
            {
                _audioClipData = new float[pcmLength];
            }

            Array.Copy(pcm, _audioClipData, pcmLength);
            _source.clip.SetData(_audioClipData, _clipHead);
            _clipHead += pcmLength;
            if (!_source.isPlaying && _clipHead > AudioClipLength / 2)
            {
                _source.Play();
            }

            _clipHead %= AudioClipLength;
        }
    }

    public enum NumChannels : int
    {
        Mono = 1,
        Stereo = 2
    }

    public enum SampleRate : int
    {
        _8000 = 8000,
        _12000 = 12000,
        _16000 = 16000,
        _24000 = 24000,
        _48000 = 48000
    }
}