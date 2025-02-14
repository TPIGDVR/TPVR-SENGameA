using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    bool isTalk;

    [Header("Breath Settings")]
    public float minActivationThreshold; //both to be calibrated
    public float maxActivationThreshold;
    public float inhaleZCRMaxThreshold;
    float rms;
    float prevRMS;
    float zcr;
    float prevZCR;

    public BreathState state = BreathState.Idle;

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
        text2.text = rms.ToString();
        if (isTalk)
        {
            text.text = "Talking";
        }
        else
        {
            text.text = "Silent";
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        float[] monoData = new float[data.Length / channels];
        for (int i = 0; i < data.Length; i += channels)
        {
            monoData[i / channels] = (data[i] + (channels > 1 ? data[i + 1] : 0)) * 0.5f;
        }

        originalData = new float[monoData.Length]; //remove when not debugging
        monoData.CopyTo(originalData, 0); //to be read in visualizer

        rms = RMS(monoData);
        isTalk = rms > maxActivationThreshold;
        if (rms < minActivationThreshold || rms > maxActivationThreshold)
        {          
            return;
        }
        print("breath");

        DetectBreathing(data);
        prevRMS = rms;
        prevZCR = zcr;

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


    void DetectBreathing(float[] data)
    {
        float zcr = ZCR(data);
        float specCentroid = SpectralCentroid(GetSpectrumData(data), (int)sampleRate);
        if (rms < 0.0045 && zcr < 0.08 && specCentroid > 3250)
        {
            print("inhale");
        }
        else if (rms >= 0.0045 && zcr < 0.08 && specCentroid < 3250)
        {
            print("exhale");
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
    public TMP_Text text2;
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

public enum BreathState
{
    Inhale,
    Exhale,
    Idle
}
