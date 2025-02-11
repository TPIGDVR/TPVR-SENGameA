using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float freqGain = 50;

    //for debug
    int highPassFirstBinIndex;

    void RetrieveMic()
    {
        mic = gameObject.AddComponent<AudioSource>();
        mic.loop = true;
        // mic.mute = true;

        // mic.clip = Microphone.Start(null, true, 1, (int)sampleRate);
        mic.clip = testClip;
        mic.spatialBlend = 0;
        // while (!(Microphone.GetPosition(null) > 0)) { }  // Wait until microphone starts
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
        CreateAudioClip();
        VisualizeSpectrum();
        if (useAFR) return;

        if (InputSystem.GetDevice<Keyboard>().spaceKey.wasPressedThisFrame)
        {
            // StartCoroutine(PerformanceTest.GetFPS(10));
        }
        // ProcessAudio();
    }

    float[] originalData;

    [Header("Time Domain")]
    //biquad filter parameters
    [Range(0,5000)]
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
    AudioClip processedClip;
    List<float> audioBuffer = new();
    public AudioSource audioSource;
    public LineRenderer freqVisualizer;
    public bool filter, full, envelopee;

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!useAFR) return;

        //microphone currently outputs in stereo
        //convert to mono
        GetFilterCoefficients();
        float[] monoData = new float[data.Length / channels];

        for (int i = 0; i < data.Length; i += channels)
        {
            monoData[i / channels] = (data[i] + (channels > 1 ? data[i + 1] : 0)) * 0.5f;
        }

        originalData = monoData;
        // monoData.CopyTo(originalData, 0); //to be read in visualizer

        float envelope = 0;
        float prevSample = 0;
        
        float[] derivatives;

        for (int i = 0; i < monoData.Length; i++)
        {
            float sample = monoData[i];
            //gain step
            sample = ApplyGain(sample);

            if(filter)
                sample = ApplyBiquadFilter(sample);

            //apply full wave rectification
            if(full)
                sample = ApplyFullWaveRectification(sample);

            //apply envelope smoothing
            if(envelopee)
            {
                envelope = ApplyEnvelopeSmoothing(envelope, sample);
                sample = envelope;
            }

            //detecting derivative



            //assign back to be read
            monoData[i] = sample;
        }


        //for visualization
        lock(audioBuffer)
        {
            audioBuffer.AddRange(monoData);
        }
        filteredSample = monoData;
    }

    float ApplyGain(float sample)
    {
        return sample * gain;
    }

    float ApplyBiquadFilter(float sample)
    {
        float x0 = sample;  // Current input sample
        float y0 = b0 * x0 + b1 * x1 + b2 * x2 - a1 * y1 - a2 * y2; //biquad filter equation
        sample = y0; //store the data back

        // Shift states
        x2 = x1;
        x1 = x0;
        y2 = y1;
        y1 = y0;

        return sample;
    }

    float ApplyFullWaveRectification(float sample)
    {
        return math.abs(sample);
    }

    float ApplyEnvelopeSmoothing(float envelope,float sample)
    {
        return smoothingFactor * envelope + (1 - smoothingFactor) * sample;
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


        float freq = (float)sampleRate/ 2f;
        float[] freqSamples = new float[(int)sampleSize];
        audioSource.GetSpectrumData(freqSamples,1,window);
        
        int lowBin = Mathf.CeilToInt(lowPassCutoff / freq * freqSamples.Length);
        int highBin = Mathf.FloorToInt(highPassCutoff / freq * freqSamples.Length);
        freqVisualizer.positionCount = lowBin - highBin;
        for (int i = highBin; i < lowBin; i++)
        {
            int vizIndex = i - highBin;
            freqVisualizer.SetPosition(vizIndex, new Vector3(vizIndex * (size + 0.01f), freqSamples[i] * freqGain, 0));
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

