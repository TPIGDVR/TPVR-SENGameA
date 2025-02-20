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
    public float rmsMinThres; //both to be calibrated
    public float rmsMaxThres;
    public float inhaleRmsMax = 0.0045f;
    public float inhaleZcrMin = 0.08f;
    public float inhaleSpecCentroidMin = 3250;
    public float exhaleRmsMin = 0.0045f;
    public float exhaleZcrMax = 0.08f;
    public float exhaleSpecCentroidMax = 3250;

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
    public BreathSettings settings;
    void RetrieveMic()
    {
        mic = gameObject.AddComponent<AudioSource>();
        mic.loop = true;
        //mic.mute = true;

        mic.clip = Microphone.Start(null, true, 1, (int)sampleRate);
        // mic.clip = testClip;
        // mic.spatialBlend = 0;
        // while (!(Microphone.GetPosition(null) > 0)) { }  // Wait until microphone starts
        mic.Play();

    }

    void Awake()
    {
        RetrieveMic();
    }

    [ContextMenu("Calibrate")]
    async void CalibrateBreathSettings()
    {
        // settings = await calibrator.BeginCalibrating();
        rmsMinThres = settings.rmsMinThres;
        rmsMaxThres = settings.rmsMaxThres;
        inhaleRmsMax = settings.inRMSMaxThres;
        inhaleZcrMin = settings.inZCRMinThres;
        inhaleSpecCentroidMin = settings.inSCMinThres;

        //EXHALE HAS ISSUES
        exhaleRmsMin = settings.exRMSMinThres;
        exhaleZcrMax = settings.exZCRMaxThres;
        exhaleSpecCentroidMax = settings.exSCMaxThres;
    }

    void Update()
    {
        text2.text = "rms : " + avgrms.ToString("n6");
        text3.text = "zcr : " + avgzcr.ToString();
        text4.text = "frq : " + avgspec.ToString();
        text.text = "state : " + state.ToString();
        text5.text = "prev state : " + prevState.ToString();
        text6.text = "prev state2 : " + prevState2.ToString();
        rmsBar.fillAmount = rms;
        zcrBar.fillAmount = zcr;
        sil1.localPosition = new Vector3(sil1.localPosition.x, rmsMinThres, 0);
        in1.localPosition = new Vector3(in1.localPosition.x, inhaleRmsMax, 0);
        sp1.localPosition = new Vector3(sp1.localPosition.x, rmsMaxThres, 0);
        sil2.localPosition = new Vector3(sil2.localPosition.x, 0, 0);
        in2.localPosition = new Vector3(in2.localPosition.x, inhaleZcrMin, 0);
        sp2.localPosition = new Vector3(sp2.localPosition.x, exhaleZcrMax, 0);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
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

        if (rms >= rmsMaxThres)
        {
            SwitchState(3);
        }
        else if (rms <= rmsMinThres)
        {
            SwitchState(2);
        }
        else if (rms < inhaleRmsMax && zcr > inhaleZcrMin && specCentroid > inhaleSpecCentroidMin && prevState != BreathState.Talking)
        {
            print("inhale");
            SwitchState(0);
        }
        else if (rms >= exhaleRmsMin && zcr < exhaleZcrMax && specCentroid < exhaleSpecCentroidMax && prevState2 == BreathState.Inhale)
        {
            print("exhale");
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
        //removes oldest entry
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
        //gets the average of last few samples to determine the state
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

    #region Test
    [Header("Testing")]
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;
    public TMP_Text text5;
    public TMP_Text text6;
    public Image rmsBar;
    public Image zcrBar;
    public RectTransform sil1, in1, sp1, sil2, in2, sp2;
    #endregion

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