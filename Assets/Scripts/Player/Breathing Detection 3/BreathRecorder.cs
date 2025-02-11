using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

public class BreathRecorder : MonoBehaviour
{
    public AudioSource mic;
    public FFTWindow window;
    public SampleRate sampleRate;
    public SampleSize sampleSize;
    public float lowPassCutoff;
    public float highPassCutoff;
    public float gain;
    public float minAmpThreshold;
    public float maxAmpThreshold;
    public Vector2 boostHighFrequencyRange;
    public float boostGain;

    float[] samples;
    float[] filteredSample;

    [Header("Visualizer")]
    public LineRenderer visualizer;
    public LineRenderer lowPassVisualizer;
    [Range(0, 1)]
    public float size;


    [Header("Testing")]
    public bool useAFR = false;
    public AudioClip testClip;
    //for debug
    int highPassFirstBinIndex;

    void RetrieveMic()
    {
        mic = gameObject.AddComponent<AudioSource>();
        mic.loop = true;
        // mic.mute = true;

        mic.clip = Microphone.Start(null, true, 1, (int)sampleRate);
        // mic.clip = testClip;
        mic.spatialBlend = 0;
        while (!(Microphone.GetPosition(null) > 0)) { }  // Wait until microphone starts
        mic.Play();
    }

    void Awake()
    {
        RetrieveMic();
        // GetFilterCoefficients();
    }

#region Frequency Domain Analysis
    void ProcessAudio()
    {
        samples = new float[(int)sampleSize];
        mic.GetSpectrumData(samples, 0, window);
        ApplyLowAndHighPassFilter(samples);
        BoostHighFrequency();
        FilterNoise();
    }

    void ApplyLowAndHighPassFilter(float[] samples)
    {
        float freq = (float)sampleRate/ 2f;
        int lowBin = Mathf.CeilToInt(lowPassCutoff / freq * samples.Length);
        int highBin = Mathf.FloorToInt(highPassCutoff / freq * samples.Length);
        highPassFirstBinIndex = highBin;
        filteredSample = new float[lowBin - highBin];
        for (int i = highBin; i < lowBin; i++)
        {
            filteredSample[i - highBin] = samples[i] * gain;
        }
    }
    List<int> prominentFrequencies;

    void FilterNoise()
    {
        int size = filteredSample.Length;
        prominentFrequencies = new List<int>();
        for (int i = 0; i < size; i++)
        {
            if (filteredSample[i] < minAmpThreshold || filteredSample[i] > maxAmpThreshold) continue;

            prominentFrequencies.Add(i);
        }
    }
    float[] boostedSample;
    void BoostHighFrequency()
    {
        float freq = (float)sampleRate / 2f;

        int startBin = Mathf.FloorToInt(boostHighFrequencyRange.x /freq * samples.Length);
        int endBin = Mathf.CeilToInt(boostHighFrequencyRange.y /freq * samples.Length);
        boostedSample = new float[filteredSample.Length];
        filteredSample.CopyTo(boostedSample, 0);
        for (int i = startBin; i < endBin; i++)
        {
            boostedSample[i] *= boostGain;
        }
    }
#endregion

    void Update()
    {
        VisualizeSpectrum();
        if (useAFR) return;

        if (InputSystem.GetDevice<Keyboard>().spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(PerformanceTest.GetFPS(10));
        }
        // ProcessAudio();
    }

    float[] originalData;

    [Header("Time Domain")]
    //biquad filter parameters
    [Range(0,25600)]
    public float FilterFrequency;
    [Range(0,5)]
    public float Q; //the lower the Q, the wider the bandwidth

    //envelope smoothing
    [Range(0,1)]
    public float smoothingFactor;


    float a0, a1, a2, b0, b1, b2; //biquad filter coefficients
    //filter states
    float x1, x2, y1, y2;

    void GetFilterCoefficients()
    {
        float omega = 2.0f * Mathf.PI * FilterFrequency / (int)sampleRate;
        float alpha = Mathf.Sin(omega) * (Q / 2.0f);

        b0 = alpha;
        b1 = 0;
        b2 = -alpha;
        a0 = 1 + alpha;
        a1 = -2 * Mathf.Cos(omega);
        a2 = 1 - alpha;

        // Normalize coefficients
        b0 /= a0;
        b1 /= a0;
        b2 /= a0;
        a1 /= a0;
        a2 /= a0;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!useAFR) return;

        //microphone currently outputs in stereo
        //convert to mono
        GetFilterCoefficients();
        float[] rawData;
        if(channels == 2)
        {
            print("stereo");
            rawData = new float[data.Length / 2];
            for (int i = 0; i < data.Length; i += 2)
            {
                float monoSample = (data[i] + data[i + 1]) * 0.5f; // Average L & R
                rawData[i/2] = monoSample;
            }

        }
        else //keep original data if mono
        {
            rawData = data;
        }
        originalData = new float[rawData.Length];
        rawData.CopyTo(originalData, 0); //to be read in visualizer

        float envelope = 0;
        for (int i = 0; i < rawData.Length; i++)
        {
            float sample = rawData[i];
            //gain step
            sample *= gain;

            //biquad filter
            float x0 = sample;  // Current input sample
            float y0 = b0 * x0 + b1 * x1 + b2 * x2 - a1 * y1 - a2 * y2; //biquad filter equation
            sample = y0; //store the data back

            // Shift states
            x2 = x1;
            x1 = x0;
            y2 = y1;
            y1 = y0;

            //apply full wave rectification
            sample = math.abs(sample);
            
            //apply envelope smoothing
            envelope = smoothingFactor * envelope + (1 - smoothingFactor) * sample;

            //assign back to be read
            rawData[i] = envelope;
        }



        filteredSample = rawData;
    }

    void VisualizeSpectrum()
    {
        // visualizer.positionCount = samples.Length;

        // for (int i = 0; i < samples.Length; i++)
        // {
        //     visualizer.SetPosition(i , new Vector3(i * size, samples[i] * 5, 0));
        // }

        if(filteredSample == null) return;
        // if(prominentFrequencies == null) return;

        // lowPassVisualizer.positionCount = filteredSample.Length;
        // for (int i = 0; i < filteredSample.Length; i++)
        // {
        //     lowPassVisualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, filteredSample[i] * 5, 0));
        // }

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

        // visualizer.positionCount = filteredSample.Length;
        // for (int i = 0; i < filteredSample.Length; i++)
        // {
        //     if(prominentFrequencies.Contains(i))
        //     {
        //         visualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, boostedSample[i] * 5, 0));
        //     }
        //     else
        //     {
        //         visualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, 0, 0));
        //     }
        // }
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

