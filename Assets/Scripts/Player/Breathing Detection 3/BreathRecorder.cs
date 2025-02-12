using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    void Update()
    {
        CreateAudioClip();
        VisualizeSpectrum();
        if (isTalk)
        {
            text.text = "Talking";
        }
        else
        {
            text.text = "Silent";
        }
    }

    float[] originalData;

    [Header("Time Domain")]
    [Range(0, 1)]
    public float inhaleThreshold;
    [Range(0, 1)]
    public float exhaleThreshold;
    public bool isInhale, isExhale;
    //biquad filter parameters
    [Range(0, 5000)]
    public float FilterFrequency;
    [Range(0, 5)]
    public float Q; //the lower the Q, the wider the bandwidth

    //envelope smoothing
    [Range(0, 1)]
    public float smoothingFactor;


    float a0, a1, a2, b0, b1, b2; //biquad filter coefficients
    //filter states
    float x1, x2, y1, y2;
    public bool filter, full, envelopee;
    public float rmsThreshold;

    float prevSample;
    float[] derivatives;

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
        GetFilterCoefficients();

        float[] monoData = new float[data.Length / channels];
        for (int i = 0; i < data.Length; i += channels)
        {
            monoData[i / channels] = (data[i] + (channels > 1 ? data[i + 1] : 0)) * 0.5f;
        }


        originalData = new float[monoData.Length]; //remove when not debugging
        monoData.CopyTo(originalData, 0); //to be read in visualizer

        float envelope = 0;
        derivatives = new float[monoData.Length];

        for (int i = 0; i < monoData.Length; i++)
        {
            //convert stereo to mono
            float sample = monoData[i];

            //gain step
            sample = ApplyGain(sample);
            //apply biquad filter
            if (filter) sample = ApplyBiquadFilter(sample);
            //apply full wave rectification
            if (full) sample = ApplyFullWaveRectification(sample);

            //apply envelope smoothing
            if (envelopee)
            {
                envelope = ApplyEnvelopeSmoothing(envelope, sample);
                sample = envelope;
            }

            //detecting derivative
            derivatives[i] = sample - prevSample;
            prevSample = sample;

            //assign back to be read
            monoData[i] = sample;
        }

        float rms = ComputeRMS(monoData);
        isTalk = rms > rmsThreshold;

        if (!isTalk)
        {
            DetectBreathing();
        }

        //for visualization
        lock (audioBuffer)
        {
            audioBuffer.AddRange(monoData);
        }
        filteredSample = monoData;
    }

    #region Time-based filters
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

    float ApplyEnvelopeSmoothing(float envelope, float sample)
    {
        return smoothingFactor * envelope + (1 - smoothingFactor) * sample;
    }
    #endregion

    bool isTalk;

    float ComputeRMS(float[] data)
    {
        float sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i] * data[i];
        }
        return Mathf.Sqrt(sum / data.Length);
    }

    public float derivativeAvgMultiplier;
    public int minSamplesBetweenStateChanges = 80; // Adjust as needed (depends on sample rate)
    public int samplesSinceStateChange = 0;
    public float derivativeAvg = 0;
    
    void DetectBreathing()
    {
        samplesSinceStateChange++;
        float derivativeSum = 0;
        for (int i = 1; i < derivatives.Length; i++)
        {
            derivativeSum += derivatives[i];
        }
        //get the average derivative
        derivativeAvg = derivativeSum / derivatives.Length * derivativeAvgMultiplier;

        // Prevent state changes from happening too frequently
        if (samplesSinceStateChange < minSamplesBetweenStateChanges)
            return;

        
        if (!isInhale && derivativeAvg > inhaleThreshold)
        {
            isInhale = true;
            isExhale = false;
            samplesSinceStateChange = 0;
            Debug.Log("Inhale detected!");
        }
        else if (!isExhale && derivativeAvg < -exhaleThreshold)
        {
            isExhale = true;
            isInhale = false;
            samplesSinceStateChange = 0;
            Debug.Log("Exhale detected!");
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


    [Header("Testing")]
    public AudioClip testClip;
    public float freqGain = 50;
    public TMP_Text text;
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
