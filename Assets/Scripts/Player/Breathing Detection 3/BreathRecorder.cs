using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Haptics;
using TMPro;
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
    public bool ignoreCutOff = false;
    public float lowPassCutoff;
    public float highPassCutoff;
    public float gain;

    public bool filterNoise = true;
    public float minAmpThreshold;
    public float maxAmpThreshold;
    public Vector2 boostHighFrequencyRange;
    public float boostGain;

    float[] samples;
    float[] filteredSample;

    [Header("Visualizer")]
    public LineRenderer visualizer;
    public LineRenderer lowPassVisualizer;
    public LineRenderer outputVisualizer;
    [Range(0, 1)]
    public float size;

    [Header("Visualizer")]
    public TextMeshProUGUI outputText;
    public float visualizerSize = 10f;
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
        prevSampleSize = sampleSize;
    }
    
#region Frequency Domain Analysis
    void ProcessAudio()
    {
        samples = new float[(int)sampleSize];
        mic.GetSpectrumData(samples, 0, window);
        
        CheckIfSampleSizeChange();
        filteredSample = BoostHighFrequency();
        
        //This one I keep for now but dont use
        filteredSample = ApplyLowAndHighPassFilter(samples);
        
        FilterNoise();
        ApplyGaussianSmoothing(filteredSample);
        
        // Pesudo Code
        /*
         * so probably ignore talking o
         * 1. Determine if the spectrum is talking or not through the RMS formula
         * 2. Check exhale if there is a higher pitch in the lower frequency
         * 3. Check inhale if there is a consistent pitch in the band frequency
         * Do the sliding window for the exhaling since the pitch is only instant for that one moment.
         */
        DetermineBreathingType();
    }

    [Header("Inhaling Detection (FREQUENCY DOMAIN)")]
    private float[] inhaleDataDisplay;
    public float maxInhaleAmp;
    public float minInhaleAmp;
    public Vector2 inhaleCutoff;    
    
    
    [Header("Exhaling Detection (FREQUENCY DOMAIN)")]
    public int windowSize = 10;
    
    public Queue<bool> exhalingWindow = new Queue<bool>();
    public TextMeshProUGUI exhalingText;
    public float maxSpikeExhaleAmp;
    public float minSpikeExhaleAmp;
    public Vector2 exhaleSpikeCutoff;
    public int exhaleAcceptableAmount = 3;
    public LineRenderer exhalingSpikeVisualizer;
    
    
    public float maxExhaleAmp;
    public float minExhaleAmp;
    public Vector2 exhaleCutoff;
    public LineRenderer exhalingVisualizer;
    
    
    void DetermineBreathingType()
    {
        if (IsTalkingFrequencyBased(filteredSample))
        {
            outputText.text = "talking";
            return;
        }

        if (DetectExhale())
        {
            outputText.text = "exhaling";
        }
        else
        {
            outputText.text = "";
        }
        
        //determine inhale and exhale

        bool DetectExhale()
        {
            float freq = (float)sampleRate / 2f;
            
            // If we detected an exhale peak previously, check the exhale energy range
            if (exhalingWindow.Contains(true))
            {
                // Convert exhale energy cutoff range to bin indices
                int exhaleStartBin = Mathf.FloorToInt(exhaleCutoff.x / freq * filteredSample.Length);
                int exhaleEndBin = Mathf.CeilToInt(exhaleCutoff.y / freq * filteredSample.Length);
                VisualiseData(exhalingSpikeVisualizer, filteredSample, exhaleStartBin, exhaleEndBin);

                // Compute average energy in exhale range
                float exhaleEnergy = 0;
                int exhaleBinCount = exhaleEndBin - exhaleStartBin + 1;
                
                //visualise it
                for (int i = exhaleStartBin; i <= exhaleEndBin; i++)
                {
                    exhaleEnergy += filteredSample[i];
                }

                exhaleEnergy /= exhaleBinCount;
                
                
                //visualise this 
                exhalingVisualizer.SetPosition(0, Vector3.zero);
                exhalingVisualizer.SetPosition(1, new Vector3(0, exhaleEnergy, 0));

                // Check if the exhale energy is within the expected range
                bool res = exhaleEnergy >= minExhaleAmp && exhaleEnergy <= maxExhaleAmp;
                
                print($"spikeEnergy: {exhaleEnergy}. Peak found? {res}");

                
                //update the window
                EnqueueWindow(res);

                //see if there is more signs that it is exhaling.
                return exhalingWindow.Where(x => x).Count() > exhaleAcceptableAmount;
            }
            else
            {
                // Convert exhale spike cutoff range to bin indices
                int spikeStartBin = Mathf.FloorToInt(exhaleSpikeCutoff.x / freq * filteredSample.Length);
                int spikeEndBin = Mathf.CeilToInt(exhaleSpikeCutoff.y / freq * filteredSample.Length);
                //display the data that is being sampled
                VisualiseData(exhalingSpikeVisualizer, filteredSample, spikeStartBin, spikeEndBin);

                // Compute the average energy in the spike range
                float spikeEnergy = 0;
                // int spikeBinCount = spikeEndBin - spikeStartBin + 1;
                for (int i = spikeStartBin; i <= spikeEndBin; i++)
                {
                    spikeEnergy += filteredSample[i];
                }

                // spikeEnergy /= spikeBinCount; // Normalize
                
                
                //visualise this 
                // exhalingSpikeVisualizer.SetPosition(0, Vector3.zero);
                // exhalingSpikeVisualizer.SetPosition(1, new Vector3(0, spikeEnergy, 0));
        
                // Check if the energy is within the exhale spike amplitude bounds
                bool foundPeak = spikeEnergy >= minSpikeExhaleAmp && spikeEnergy <= maxSpikeExhaleAmp;
                print($"spikeEnergy: {spikeEnergy}. Peak found? {foundPeak}");

                EnqueueWindow(foundPeak);
            }

            void VisualiseData(LineRenderer targetLine, float[] data, int start, int end)
            {
                targetLine.positionCount = end - start + 1;
                
                for (int i = start; i <= end; i++)
                {
                    targetLine.SetPosition(i , new Vector3((i - start) * size, data[i] * visualizerSize, 0));
                }
            }
            
            return false; // No peak or not enough energy in the exhale range
        }
        
        

        void EnqueueWindow(bool res)
        {
            exhalingWindow.Enqueue(res);
            while (exhalingWindow.Count > windowSize)
            {
                exhalingWindow.Dequeue();
            }
            
            //display the text
            exhalingText.text = $"Exhaling True: {exhalingWindow.Where(x=>x).Count()}";
        }
    }

    private SampleSize prevSampleSize;

    void CheckIfSampleSizeChange()
    {
        if (prevSampleSize == sampleSize) return;
        //write code here if the sample size changed
        prevSampleSize = sampleSize;
    }

    float[] ApplyLowAndHighPassFilter(float[] samples)
    {
        if (!ignoreCutOff)
        {
            //if do not ignore cutoff
            float freq = (float)sampleRate/ 2f;
            int lowBin = Mathf.CeilToInt(lowPassCutoff / freq * samples.Length);
            int highBin = Mathf.FloorToInt(highPassCutoff / freq * samples.Length);
            highPassFirstBinIndex = highBin;
            // filteredSample = new float[lowBin - highBin];
            var result = new float[lowBin - highBin];
            for (int i = highBin; i < lowBin; i++)
            {
                result[i - highBin] = samples[i] * gain;
            }
            return result;    
        }
        else
        {
            //do nothing
            highPassFirstBinIndex = 0;
            return samples;
        }
        
    }
    
    List<int> prominentFrequencies;

    void FilterNoise()
    {
        if (!filterNoise) return; 
        int size = filteredSample.Length;
        prominentFrequencies = new List<int>();
        for (int i = 0; i < size; i++)
        {
            if (filteredSample[i] < minAmpThreshold || filteredSample[i] > maxAmpThreshold) continue;
            filteredSample[i] = 0;
        }
    }
    
    float[] boostedSample;
    float[] BoostHighFrequency()
    {
        float freq = (float)sampleRate / 2f;

        int startBin = Mathf.FloorToInt(boostHighFrequencyRange.x /freq * samples.Length);
        int endBin = Mathf.CeilToInt(boostHighFrequencyRange.y /freq * samples.Length);
        // boostedSample = new float[samples.Length];
        // samples.CopyTo(boostedSample, 0);
        var result = new float[samples.Length];
        for (int i = startBin; i < endBin; i++)
        {
            result[i] = boostGain * samples[i];
        }

        return result;
    }

    // Apply Gaussian smoothing to the spectrum

    [Header("Gaussian Smoothing")]
    public bool ApplySmoothing = true;
    public float sigma = 1f;
    void ApplyGaussianSmoothing(float[] spectrum)
    {
        if (!ApplySmoothing) return; 
        float[] smoothed = new float[spectrum.Length];
        int halfWindow = 5;  // Width of the Gaussian window
    
        // Create the Gaussian kernel
        float[] kernel = new float[halfWindow * 2 + 1];
        float sum = 0f;
        for (int i = -halfWindow; i <= halfWindow; i++)
        {
            kernel[i + halfWindow] = Mathf.Exp(-0.5f * Mathf.Pow(i / sigma, 2));
            sum += kernel[i + halfWindow];
        }
    
        // Normalize kernel
        for (int i = 0; i < kernel.Length; i++)
        {
            kernel[i] /= sum;
        }

        // Apply Gaussian kernel to spectrum
        for (int i = 0; i < spectrum.Length; i++)
        {
            float smoothedValue = 0f;
            for (int j = -halfWindow; j <= halfWindow; j++)
            {
                int idx = Mathf.Clamp(i + j, 0, spectrum.Length - 1);
                smoothedValue += spectrum[idx] * kernel[j + halfWindow];
            }
            smoothed[i] = smoothedValue;
        }

        // Copy the result back to the spectrum
        System.Array.Copy(smoothed, spectrum, spectrum.Length);
    }
    
    [Header("Talking Threshold (Frequency Domain)")]
    public float spectralFlatnessThreshold = 0.5f;
    public float minAmpTalkingThreshold = 0.5f;
    bool IsTalkingBySpectralFlatness(float[] spectrum)
    {
        float geometricMean = 1;
        float arithmeticMean = 0;
        int count = spectrum.Length;

        for (int i = 0; i < count; i++)
        {
            geometricMean *= Mathf.Max(spectrum[i], 1e-10f); // Prevent log(0)
            arithmeticMean += spectrum[i];
        }
        geometricMean = Mathf.Pow(geometricMean, 1f / count);
        arithmeticMean /= count;

        float sfm = geometricMean / arithmeticMean; // Spectral flatness ratio (0 to 1)
        return sfm < spectralFlatnessThreshold; // Speech tends to have a lower flatness value
    }
    
    bool IsTalkingFrequencyBased(float[] spectrum)
    {
        float freq = (float)sampleRate / 2f;
        int startBin = Mathf.FloorToInt(85 / freq * spectrum.Length);
        int endBin = Mathf.CeilToInt(3000 / freq * spectrum.Length);

        float power = 0;
        for (int i = startBin; i <= endBin; i++)
        {
            power += spectrum[i]; // Sum power in voice frequency range
        }
        print($"overall power {power}");
        return power > minAmpTalkingThreshold;
    }
    
    
    #endregion

    void Update()
    {
        VisualizeSpectrum();
        VisualizeOutputData();
        if (useAFR) return;

        if (InputSystem.GetDevice<Keyboard>().spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(PerformanceTest.GetFPS(10));
        }
        ProcessAudio();
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

    void VisualizeOutputData()
    {
        //reuse the same array
        var outputData = new float[(int)sampleSize];
        mic.GetOutputData(outputData, 0);
        
        outputVisualizer.positionCount = outputData.Length;
        for (int i = 0; i < outputData.Length; i++)
        {
            outputVisualizer.SetPosition(i , new Vector3(i * size, outputData[i] * 5, 0));
        }

    }
    
    void VisualizeSpectrum()
    {
        if(samples == null) return;
        if(filteredSample == null) return;
        
        visualizer.positionCount = samples.Length;

        for (int i = 0; i < samples.Length; i++)
        {
            visualizer.SetPosition(i , new Vector3(i * size, samples[i] * visualizerSize, 0));
        }
        
        if(prominentFrequencies == null) return;

        lowPassVisualizer.positionCount = filteredSample.Length;
         for (int i = 0; i < filteredSample.Length; i++)
         {
             lowPassVisualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, filteredSample[i] * visualizerSize, 0));
         }

        // visualizer.positionCount = originalData.Length;
        // for (int i = 0; i < originalData.Length; i++)
        // {
        //     visualizer.SetPosition(i, new Vector3(i * size, originalData[i] * 5, 0));
        // }

        // lowPassVisualizer.positionCount = filteredSample.Length;
        // for (int i = 0; i < filteredSample.Length; i++)
        // {
        //     lowPassVisualizer.SetPosition(i, new Vector3(i * size, filteredSample[i] * 5, 0));
        // }

        // lowPassVisualizer.positionCount = filteredSample.Length;
        // for (int i = 0; i < filteredSample.Length; i++)
        // {
        //     if(prominentFrequencies.Contains(i))
        //     {
        //         lowPassVisualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, filteredSample[i] * 5, 0));
        //     }
        //     else
        //     {
        //         lowPassVisualizer.SetPosition(i, new Vector3((i + highPassFirstBinIndex) * size, 0, 0));
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