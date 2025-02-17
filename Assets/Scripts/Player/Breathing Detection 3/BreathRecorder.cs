using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static AudioAnalyzer;

public class BreathRecorder : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource mic;
    public FFTWindow window;
    public SampleRate sampleRate;
    public SampleSize sampleSize;
    public float gain;

    float[] samples;
    float[] filteredSample;
    float[] originalData; //used for debugging
    public int historyBufferSize;
    Queue<AudioChunk> audioHistory = new Queue<AudioChunk>();


    [Header("Breath Settings")]
    public float rmsMinThres; //both to be calibrated
    public float rmsMaxThres;
    [Range(0,1)]
    public float rmsThresLeniency;
    float rms;
    float zcr;
    float specCentroid;
    public int stateDelay;
    public int delayCount;

    public BreathState state = BreathState.Idle;
    BreathState prevState;
    BreathState prevState2; //non-idle state

    void RetrieveMic()
    {
        mic = gameObject.AddComponent<AudioSource>();
        mic.loop = true;
        //mic.mute = true;

        mic.clip = Microphone.Start(null, true, 1, (int)sampleRate);
        // mic.clip = testClip;
        mic.spatialBlend = 0;
        while (!(Microphone.GetPosition(null) > 0)) { }  // Wait until microphone starts
        mic.Play();
    }

    void Awake()
    {
        RetrieveMic();
    }

    void Update()
    {
        // CreateAudioClip();
        // VisualizeSpectrum();
        text2.text = "rms : " + avgrms.ToString("n6");
        text3.text = "zcr : " + avgzcr.ToString();
        text4.text = "frq : " + avgspec.ToString();
        text.text = "state : " + state.ToString();
        text5.text = "prev state : " + prevState.ToString();
        text6.text = "prev state2 : " + prevState2.ToString();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            print("change clip");
            mic.Stop();
            mic.clip = testClip;
            mic.Play();
            // RetrieveMic();
        }
    }
    public float inhaleRmsMax = 0.0045f;
    public float inhaleZcrMin = 0.08f;
    public float inhaleSpecCentroidMin = 3250;
    public float exhaleRmsMin = 0.0045f;
    public float exhaleZcrMax = 0.08f;
    public float exhaleSpecCentroidMax = 3250;

    void OnAudioFilterRead(float[] data, int channels)
    {
        
        float[] monoData = new float[data.Length / channels];
        for (int i = 0; i < data.Length; i += channels)
        {
            monoData[i / channels] = (data[i] + (channels > 1 ? data[i + 1] : 0)) * 0.5f;
        }

        originalData = new float[monoData.Length]; //remove when not debugging
        monoData.CopyTo(originalData, 0); //to be read in visualizer
        prevState = SpeculatePreviousState();
        delayCount++;
        rms = RMS(monoData);
        zcr = ZCR(data);

        specCentroid = SpectralCentroid(GetSpectrumData(data), (int)sampleRate);
        // bool talkingCondition = (rms > rmsMaxThres - rmsThresLeniency ) && (rms < rmsMaxThres + rmsThresLeniency);
        // if (rms >= rmsMaxThres)
        // {
        //     SwitchState(3);
        // }
        // else
        // {
        //     SwitchState(2);
        // }

        // state = rms >= rmsMaxThres ? BreathState.Talking : BreathState.Idle;
        // if (state == BreathState.Talking || rms < rmsMinThres)
        // {
        //     return;
        // }
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
            // state = BreathState.Inhale;
            SwitchState(0);
        }
        else if (rms >= exhaleRmsMin && zcr < exhaleZcrMax && specCentroid < exhaleSpecCentroidMax && prevState != BreathState.Talking && prevState2 == BreathState.Inhale)
        {
            print("exhale");
            // state = BreathState.Exhale;
            SwitchState(1);
        }


        AddToAudioHistory();
        //for visualization
        lock (audioBuffer)
        {
            audioBuffer.AddRange(monoData);
        }
        filteredSample = monoData; //to be read by visualizer
        
    }

    float ApplyGain(float sample)
    {
        return sample * gain;
    }

    void AddToAudioHistory()
    {
        //removes oldest entry
        if (audioHistory.Count >= historyBufferSize)
        {
            for (int i = 0; i < audioHistory.Count - historyBufferSize; i++)
            {
                audioHistory.Dequeue();
            }
        }

        AudioChunk chunk = new AudioChunk
        {
            rms = rms,
            zcr = zcr,
            specCentroid = specCentroid,
            data = originalData,
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
        if(highestCount != 2) //if it is not idle
        {
            highestCount2 = highestCount;
        }
        prevState2 = (BreathState)highestCount2;
        return (BreathState)highestCount;
    }

    void SwitchState(BreathState goState)
    {
        if(delayCount > stateDelay)
        {
            delayCount = 0;
            state = goState;
        }
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

    //for debugging
    void CreateAudioClip()
    {
        lock (audioBuffer)
        {
            if (audioBuffer.Count == 0)
            {
                Debug.LogWarning("No audio data collected!");
                return;
            }

            // Convert List to array
            float[] finalData = audioBuffer.ToArray();
            audioBuffer.Clear(); // Clear buffer after use

            // Create a new AudioClip
            processedClip = AudioClip.Create("ProcessedAudio", finalData.Length, 1, (int)sampleRate, false);
            processedClip.SetData(finalData, 0);

            // Play the processed audio
            audioSource.clip = processedClip;
            audioSource.Play();
        }
    }

    void VisualizeSpectrum()
    {
        if (filteredSample == null) return;

        visualizer.positionCount = originalData.Length;
        for (int i = 0; i < originalData.Length; i++)
        {
            visualizer.SetPosition(i, new Vector3(i * size, originalData[i] * 5, 0));
        }

        lowPassVisualizer.positionCount = filteredSample.Length;
        for (int i = 0; i < filteredSample.Length; i++)
        {
            lowPassVisualizer.SetPosition(i, new Vector3(i * size, filteredSample[i] * 5, 0));
        }


        float freq = (float)sampleRate / 2f;
        float[] freqSamples = new float[(int)sampleSize];
        audioSource.GetSpectrumData(freqSamples, 1, window);

        int lowBin = Mathf.CeilToInt(lowPassCutoff / freq * freqSamples.Length);
        int highBin = Mathf.FloorToInt(highPassCutoff / freq * freqSamples.Length);
        freqVisualizer.positionCount = lowBin - highBin;
        for (int i = highBin; i < lowBin; i++)
        {
            int vizIndex = i - highBin;
            freqVisualizer.SetPosition(vizIndex, new Vector3(vizIndex * (size + 0.01f), freqSamples[i] * freqGain, 0));
        }
    }

    void OnDestroy()
    {
        Microphone.End(null);
    }

#region Test
    [Header("Testing")]
    public AudioClip testClip;
    public float freqGain = 50;
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;
    public TMP_Text text5;
    public TMP_Text text6;

    public AudioSource audioSource;
    AudioClip processedClip;
    List<float> audioBuffer = new();
    public LineRenderer visualizer;
    public LineRenderer lowPassVisualizer;
    public LineRenderer freqVisualizer;
    [Range(0, 1)]
    public float size;
    public float lowPassCutoff;
    public float highPassCutoff;
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