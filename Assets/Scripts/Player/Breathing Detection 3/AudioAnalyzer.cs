using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using UnityEngine;

public static class AudioAnalyzer
{
    public static List<float[]> timeDataSets;
    public static List<float[]> freqDataSets;

    public static void Initialize(List<AudioClip> clipsToAnalyze)
    {
        timeDataSets = new List<float[]>();
        Debug.Log("getting time data...");
        clipsToAnalyze.ForEach(clip =>
        {
            //get the pcm data
            int sampleCount = clip.samples * clip.channels;
            float[] pcmData = new float[sampleCount];
            clip.GetData(pcmData, 0);

            timeDataSets.Add(pcmData);
        });
        Debug.Log("getting spectrum data...");
        GetSpectrumData();
        Debug.Log("complete initialization");
    }


    public static AudioClipData AnalyzeData()
    {
        AudioClipData data = new AudioClipData();
        data.AvgRMS = CalculateAverageRMS();
        data.AvgDerivative = CalculateAverageDerivative();
        data.AvgZCR = CalculateAverageZCR();

        return data;
    }

    //fft the current time data sets to get the frequency data
    static void GetSpectrumData()
    {
        freqDataSets = new();
        foreach (var data in timeDataSets)
        {
            int length = data.Length;
            Complex[] complexData = new Complex[length];
            //convert into complex class
            for (int i = 0; i < length; i++)
            {
                complexData[i] = new Complex(data[i], 0);
            }

            Fourier.Forward(complexData, FourierOptions.Matlab);

            int spectrumLength = length / 2;
            float[] spectrum = new float[spectrumLength];
            for (int i = 0; i < spectrumLength; i++)
            {
                // Get magnitude (absolute value) of the complex number
                spectrum[i] = (float)complexData[i].Magnitude;
            }

            freqDataSets.Add(spectrum);
        }
    }

    //calculate Root Mean Squared
    //inhale should have lower RMS than exhale
    static float CalculateAverageRMS()
    {
        float RMSSum = 0;
        foreach (var timeData in timeDataSets)
        {
            //calculate RMS
            float sum = 0;
            foreach (var entry in timeData)
            {
                sum += entry * entry;
            }
            RMSSum += Mathf.Sqrt(sum / timeData.Length);
        }
        return RMSSum / timeDataSets.Count;
    }

    //calculate derivatives
    //unsure how each breath should differ
    static float CalculateAverageDerivative()
    {
        float derivativeSum = 0;
        foreach (var timeData in timeDataSets)
        {
            float sum = 0;
            for (int i = 1; i < timeData.Length; i++)
            {
                sum += timeData[i];
            }
            derivativeSum += sum / timeData.Length;
        }

        return derivativeSum / timeDataSets.Count;
    }

    //calculate zero crossing rate
    //inhale should have more ZCR than exhale
    static float CalculateAverageZCR()
    {
        float ZCRSum = 0;
        foreach (var timeData in timeDataSets)
        {
            int zc = 0;
            for (int i = 1; i < timeData.Length; i++)
            {
                //maybe add an ignore to reduce noise impact

                bool con1 = timeData[i] > 0 && timeData[i - 1] < 0;
                bool con2 = timeData[i] < 0 && timeData[i - 1] > 0;
                if (con1 || con2)
                    zc++;
            }
            ZCRSum += (float)zc / timeData.Length;
        }

        return ZCRSum / timeDataSets.Count;
    }
}

public struct AudioClipData
{
    public float AvgRMS;
    public float AvgDerivative;
    public float AvgZCR;
}
