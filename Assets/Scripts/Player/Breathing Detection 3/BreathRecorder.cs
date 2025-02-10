using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathRecorder : MonoBehaviour
{
    public AudioSource mic;
    public FFTWindow window;
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


    //for debug
    int highPassFirstBinIndex;


    void ProcessAudio()
    {
        samples = new float[(int)sampleSize];
        mic.GetSpectrumData(samples, 0, window);
        ApplyLowAndHighPassFilter();
        BoostHighFrequency();
        FilterNoise();


        VisualizeSpectrum();
    }

    void ApplyLowAndHighPassFilter()
    {
        float freq = (float)AudioSettings.outputSampleRate / 2f;
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
        float freq = (float)AudioSettings.outputSampleRate / 2f;

        int startBin = Mathf.FloorToInt(boostHighFrequencyRange.x /freq * samples.Length);
        int endBin = Mathf.CeilToInt(boostHighFrequencyRange.y /freq * samples.Length);
        boostedSample = new float[filteredSample.Length];
        filteredSample.CopyTo(boostedSample, 0);
        for (int i = startBin; i < endBin; i++)
        {
            boostedSample[i] *= boostGain;
        }
    }

    void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(PerformanceTest.GetFPS(10));
        }
        ProcessAudio();
    }

    void VisualizeSpectrum()
    {
        // visualizer.positionCount = samples.Length;

        // for (int i = 0; i < samples.Length; i++)
        // {
        //     visualizer.SetPosition(i , new Vector3(i * size, samples[i] * 5, 0));
        // }

        lowPassVisualizer.positionCount = filteredSample.Length;
        for (int i = 0; i < filteredSample.Length; i++)
        {
            lowPassVisualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, filteredSample[i] * 5, 0));
        }

        visualizer.positionCount = filteredSample.Length;
        for (int i = 0; i < filteredSample.Length; i++)
        {
            if(prominentFrequencies.Contains(i))
            {
                visualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, boostedSample[i] * 5, 0));
            }
            else
            {
                visualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, 0, 0));
            }
        }
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


