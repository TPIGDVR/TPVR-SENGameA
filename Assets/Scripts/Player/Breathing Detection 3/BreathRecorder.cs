using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AudioAnalyzer;

public class BreathRecorder : MonoBehaviour
{
    public bool IsActive;
    [Header("Audio Settings")]
    public AudioSource mic;
    public SampleRate sampleRate;

    public int historyBufferSize;
    Queue<AudioChunk> audioHistory = new Queue<AudioChunk>();

    [Header("Breath Settings")]
    public BreathSettings settings;

    float rms;
    float zcr;
    float specCentroid;
    public int stateDelay;
    public int delayCount;

    public BreathState state = BreathState.Idle;
    BreathState prevState;
    BreathState prevState2; //non-idle state
    public BreathCalibrator calibrator;
    public AudioData captureData;

    void RetrieveMic()
    {
        mic = gameObject.AddComponent<AudioSource>();
        mic.loop = true;

        mic.clip = Microphone.Start(null, true, 1, (int)sampleRate);
        while (!(Microphone.GetPosition(null) > 0)) { }  // Wait until microphone starts
        mic.Play();
    }

    void Awake()
    {
        RetrieveMic();
    }

    void Start()
    {
        EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_BREATHING, CalibrateBreathSettings);
    }


    [ContextMenu("Calibrate")]
    async void CalibrateBreathSettings()
    {
        print("calibrating");
        EventSystem.dialog.RemoveListener(DialogEvents.ACTIVATE_BREATHING, CalibrateBreathSettings);
        settings = await calibrator.BeginCalibrating();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        //if not active then ignore.
        if (IsActive) return;

        float[] monoData = new float[data.Length / channels];
        for (int i = 0; i < data.Length; i += channels)
        {
            monoData[i / channels] = (data[i] + (channels > 1 ? data[i + 1] : 0)) * 0.5f;
        }

        prevState = SpeculatePreviousState();
        delayCount++;
        rms = RMS(monoData);
        zcr = ZCR(monoData);
        float[] spectrumData = GetSpectrumData(monoData);
        specCentroid = SpectralCentroid(spectrumData, (int)sampleRate);
        bool isTalking = rms >= settings.rmsMaxThres && zcr < settings.exZCRMinThres;
        bool isSilent = rms <= settings.rmsMinThres;
        bool isInhale = rms < settings.inRMSMaxThres && zcr > settings.inZCRMinThres && prevState != BreathState.Talking;
        bool isExhale = rms >= settings.exRMSMinThres && rms < settings.exRMSMaxThres && zcr > settings.exZCRMinThres && prevState2 == BreathState.Inhale;

        if (isTalking)
        {
            SwitchState(3);
        }
        else if (isSilent)
        {
            SwitchState(2);
        }
        else if (isInhale)
        {
            SwitchState(0);
        }
        else if (isExhale)
        {
            SwitchState(1);
        }

        AddToAudioHistory(monoData);
        captureData = new AudioData
        {
            PCMData = monoData,
            SpectrumData = spectrumData,
            SampleRate = (int)sampleRate,
            Channels = channels,
            Samples = monoData.Length
        };
    }

    void AddToAudioHistory(float[] data)
    {
        if (audioHistory.Count >= historyBufferSize)
        {
            while (audioHistory.Count > historyBufferSize)
            {
                audioHistory.Dequeue();
            }
        }

        AudioChunk chunk = new AudioChunk
        {
            rms = rms,
            zcr = zcr,
            specCentroid = specCentroid,
            data = data,
            state = state
        };
        audioHistory.Enqueue(chunk);
    }

    float avgrms = 0;
    float avgzcr = 0;
    float avgspec = 0;

    BreathState SpeculatePreviousState()
    {
        int[] stateCount = new int[4];
        float rms = 0;
        float zcr = 0;
        float spec = 0;

        foreach (var entry in audioHistory)
        {
            stateCount[(int)entry.state]++;

            rms += entry.rms;
            zcr += entry.zcr;
            spec += entry.specCentroid;
        }
        avgrms = rms / audioHistory.Count;
        avgzcr = zcr / audioHistory.Count;
        avgspec = spec / audioHistory.Count;

        int highestCount = 0;
        int highestCount2 = (int)prevState2;
        for (int i = 0; i < stateCount.Length; i++)
        {
            if (stateCount[i] > stateCount[highestCount])
            {
                highestCount = i;
            }
        }
        if (highestCount != 2) //if it is not idle
        {
            highestCount2 = highestCount;
        }
        prevState2 = (BreathState)highestCount2;
        return (BreathState)highestCount;
    }

    void SwitchState(int i)
    {
        if (i == (int)state)
            return;

        if (delayCount > stateDelay)
        {
            delayCount = 0;
            state = (BreathState)i;
        }
    }

    void OnDestroy()
    {
        Microphone.End(null);
    }
}

public enum SampleSize
{
    _128 = 128,
    _256 = 256,
    _512 = 512,
    _1024 = 1024,
    _2048 = 2048,
    _4096 = 4096,
    _8192 = 8192,
}

public enum SampleRate
{
    _8000 = 8000,
    _16000 = 16000,
    _44100 = 44100,
    _48000 = 48000,
}

public enum BreathState
{
    Inhale = 0,
    Exhale,
    Idle,
    Talking
}

[Serializable]
public struct AudioChunk
{
    public float rms;
    public float zcr;
    public float specCentroid;
    public float[] data;
    public BreathState state;
}

[Serializable]
public struct BreathSettings
{
    public float rmsMinThres;
    public float rmsMaxThres;
    public float inRMSMinThres;
    public float inRMSMaxThres;
    public float inZCRMinThres;
    public float inZCRMaxThres;
    public float inSCMinThres;
    public float inSCMaxThres;
    public float exRMSMinThres;
    public float exRMSMaxThres;
    public float exZCRMinThres;
    public float exZCRMaxThres;
    public float exSCMinThres;
    public float exSCMaxThres;
}
