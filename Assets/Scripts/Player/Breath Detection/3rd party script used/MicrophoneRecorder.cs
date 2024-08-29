using System;
using UnityEngine;

namespace BreathDetection
{
    public class MicrophoneRecorder : MonoBehaviour
    {
        public event Action<float[]> OnAudioReady;
        private const int SampleRate = 48000;
        private const int RecordLengthSec = 1;
        private AudioClip _microphoneClip;
        private int _clipHead;
        private readonly float[] _processBuffer = new float[480];
        private readonly float[] _microphoneBuffer = new float[RecordLengthSec * SampleRate];

        void Start()
        {
            _microphoneClip = Microphone.Start(Microphone.devices[0], true, RecordLengthSec, SampleRate);
        }

        void Update()
        {
            var curMicPos = Microphone.GetPosition(null);
            //means that the mic has not even begin

            if (curMicPos < 0 || _clipHead == curMicPos) return;

            //store the output data from the microphone and put it in the buffer.
            _microphoneClip.GetData(_microphoneBuffer, 0);

            //if head is more than tail then that is the length, else it means that
            // the head is above the tail and would 
            int GetDataLength(int bufferLength, int head, int tail) =>
                head < tail ? tail - head : bufferLength - head + tail;

            while (GetDataLength(_microphoneBuffer.Length, _clipHead, curMicPos) > _processBuffer.Length)
            {
                var remain = _microphoneBuffer.Length - _clipHead;
                if (remain < _processBuffer.Length)
                {
                    //if the remain clip is more than the process buffer to handle,
                    //loop through the microphone buffer.
                    Array.Copy(_microphoneBuffer, _clipHead, _processBuffer, 0, remain);
                    Array.Copy(_microphoneBuffer, 0, _processBuffer, 0, _processBuffer.Length - remain);
                }
                else
                {
                    Array.Copy(_microphoneBuffer, _clipHead, _processBuffer, 0, _processBuffer.Length);
                }

                OnAudioReady?.Invoke(_processBuffer);
                _clipHead += _processBuffer.Length;
                if (_clipHead > _microphoneBuffer.Length)
                {
                    _clipHead -= _microphoneBuffer.Length;
                }
            }
        }
    }
}