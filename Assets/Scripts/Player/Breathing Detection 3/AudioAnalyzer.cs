using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using UnityEngine;

public static class AudioAnalyzer
{
    public static List<AudioData> audioData;
    public static void Initialize(List<AudioClip> clipsToAnalyze)
    {
        audioData = new List<AudioData>();
        clipsToAnalyze.ForEach(clip =>
        {
            AudioData data = new();
            //get the pcm data
            int sampleCount = clip.samples * clip.channels;
            float[] pcmData = new float[sampleCount];
            clip.GetData(pcmData, 0);
            float[] specData = GetSpectrumData(pcmData);

            //assigning the data
            data.PCMData = pcmData;
            data.SpectrumData = specData;
            data.SampleRate = clip.frequency;
            data.Channels = clip.channels;
            data.Samples = clip.samples;
            data.Length = clip.length;
            audioData.Add(data);
        });
    }


    public static AudioAnalysisResult AnalyzeData()
    {
        AudioAnalysisResult data = new AudioAnalysisResult();
        data.AvgRMS = CalculateAverageRMS();
        data.AvgDerivative = CalculateAverageDerivative();
        data.AvgZCR = CalculateAverageZCR();
        data.AvgSpecCentroid = CalculateAverageSpectralCentroid();

        return data;
    }

    //fft the current time data sets to get the frequency data
    static float[] GetSpectrumData(float[] data)
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

        return spectrum;
    }

    //calculate Root Mean Squared
    //inhale should have lower RMS than exhale
    static float CalculateAverageRMS()
    {
        float RMSSum = 0;
        foreach (var data in audioData)
        {
            var timeData = data.PCMData;
            RMSSum += RMS(timeData);
        }
        return RMSSum / audioData.Count;
    }

    public static float RMS(float[] data)
    {
        float sum = 0;
        foreach (var entry in data)
        {
            sum += entry * entry;
        }
        return Mathf.Sqrt(sum / data.Length);
    }

    //calculate derivatives
    //unsure how each breath should differ
    static float CalculateAverageDerivative()
    {
        float derivativeSum = 0;
        foreach (var data in audioData)
        {
            var timeData = data.PCMData;
            float sum = 0;
            for (int i = 1; i < timeData.Length; i++)
            {
                sum += timeData[i];
            }
            derivativeSum += sum / timeData.Length;
        }

        return derivativeSum / audioData.Count;
    }

    //calculate zero crossing rate
    //inhale should have more ZCR than exhale
    static float CalculateAverageZCR()
    {
        float ZCRSum = 0;
        foreach (var data in audioData)
        {
            var timeData = data.PCMData;
            ZCRSum += ZCR(timeData);
        }

        return ZCRSum / audioData.Count;
    }

    public static float ZCR(float[] data)
    {
        var timeData = data;
        int zc = 0;
        for (int i = 1; i < timeData.Length; i++)
        {
            //maybe add an ignore to reduce noise impact
            bool con1 = timeData[i] > 0 && timeData[i - 1] < 0;
            bool con2 = timeData[i] < 0 && timeData[i - 1] > 0;
            if (con1 || con2)
                zc++;
        }
        return (float)zc / timeData.Length;
    }

    static float CalculateAverageSpectralCentroid()
    {
        float centroidSum = 0f;
        foreach (var data in audioData)
        {
            var specData = data.SpectrumData;
            // Normalize the result
            centroidSum = SpectralCentroid(specData, data.SampleRate);
        }
        return centroidSum / audioData.Count;
    }

    public static float SpectralCentroid(float[] data, int sampleRate)
    {
        float centroid = 0f;
        float totalMagnitude = 0f;
        // Iterate through the spectrum to compute the centroid
        for (int i = 0; i < data.Length; i++)
        {
            // Calculate the frequency of bin 'i'
            float frequency = sampleRate;
            // Weighted sum of frequencies based on their magnitude
            centroid += frequency * data[i];
            totalMagnitude += data[i];
        }
        // Normalize the result
        return totalMagnitude > 0 ? centroid / totalMagnitude : 0f;
    }
}

public struct AudioData
{
    public float[] PCMData;
    public float[] SpectrumData;
    public int SampleRate;
    public int Channels;
    public int Samples;
    public float Length;
}

public struct AudioAnalysisResult
{
    public float AvgRMS;
    public float AvgDerivative;
    public float AvgZCR;
    public float AvgSpecCentroid;
}
