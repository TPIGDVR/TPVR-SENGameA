using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using static AudioAnalyzer;

public class BreathCalibrator : MonoBehaviour
{
    [Header("Values")]
    public BreathRecorder recorder;
    public float captureInterval = 0.02f;
    public float intervalBetweenInEx = 1f;
    public int calibrationAmt;
    public float inhaleDuration;
    public float exhaleDuration;
    List<AudioData> inhaleData = new();
    List<AudioData> exhaleData = new();
    List<AudioData> silentData = new();
    List<AudioData> speechData = new();

    [Header("References")]
    public TMP_Text instructionText;
    public TMP_Text timerText;

    [Header("Data")]
    public AudioDataParameters inhaleParams;
    public AudioDataParameters exhaleParams;
    


    public async Task<BreathSettings> BeginCalibrating()
    {
        inhaleData = new();
        exhaleData = new();
        instructionText.text = "Beginning Calibration...";
        await Countdown(3);

        instructionText.text = "Keep your environment quiet in...";
        await Countdown(3);

        instructionText.text = "Silence...";
        await CaptureData(silentData, 5);
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        instructionText.text = "Read the incoming text out loud";
        await Countdown(3);
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        instructionText.text = "When life gives you lemons, yeet them at your enemies and assert dominance. 🍋💥";
        await CaptureData(speechData, 5);
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        for (int i = 0; i < calibrationAmt; i++)
        {
            instructionText.text = "Get ready to inhale";
            await Countdown(3);
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

            instructionText.text = "Inhale";
            await CaptureData(inhaleData, inhaleDuration);
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

            instructionText.text = "Exhale";
            await CaptureData(exhaleData, exhaleDuration);
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));
        }

        Debug.Log("Calibration complete");

        inhaleParams = GetAudioParameters(inhaleData);
        exhaleParams = GetAudioParameters(exhaleData);
        var silentParams = GetAudioParameters(silentData);
        var speechParams = GetAudioParameters(speechData);
        BreathSettings settings = new()
        {
            inRMSMinThres = inhaleParams.minRMS,
            inRMSMaxThres = inhaleParams.maxRMS,
            inZCRMaxThres = inhaleParams.maxZCR,
            inZCRMinThres = inhaleParams.minZCR,
            inSCMaxThres = inhaleParams.maxSpecCentroid,
            inSCMinThres = inhaleParams.minSpecCentroid,

            //exhale has issues being detected
            exRMSMaxThres = exhaleParams.maxRMS,
            exRMSMinThres = exhaleParams.minRMS,
            exZCRMaxThres = exhaleParams.maxZCR,
            exZCRMinThres = exhaleParams.minZCR,
            exSCMaxThres = exhaleParams.maxSpecCentroid,
            exSCMinThres = exhaleParams.minSpecCentroid,

            rmsMinThres = silentParams.maxRMS,
            rmsMaxThres = speechParams.AverageRMS,
        };


        return settings;
    }

    private async Task Countdown(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            timerText.text = i.ToString() + "...";
            await Task.Delay(1000);
        }
    }

    private async Task CaptureData(List<AudioData> data, float recordTime)
    {
        float time = recordTime;

        while (time > 0)
        {
            timerText.text = time.ToString("n2");
            data.Add(recorder.captureData);
            time -= captureInterval;
            await Task.Delay(TimeSpan.FromSeconds(captureInterval));
        }
    }

    private AudioDataParameters GetAudioParameters(List<AudioData> dataList)
    {
        List<float> rmsList = new();
        List<float> zcrList = new();
        List<float> specCentroidList = new();

        float totalRMS = 0, totalZCR = 0, totalSpecCentroid = 0;

        foreach (var data in dataList)
        {
            float[] pcm = data.PCMData;
            float[] spec = data.SpectrumData;

            GetParameters(pcm, spec, out float rms, out float zcr, out float specCentroid, rmsList, zcrList, specCentroidList);
            totalRMS += rms;
            totalZCR += zcr;
            totalSpecCentroid += specCentroid;
        }

        return new AudioDataParameters
        {
            AverageRMS = totalRMS / dataList.Count,
            AverageZCR = totalZCR / dataList.Count,
            AverageSpecCentroid = totalSpecCentroid / dataList.Count,
            minRMS = rmsList.Min(),
            maxRMS = rmsList.Max(),
            minZCR = zcrList.Min(),
            maxZCR = zcrList.Max(),
            minSpecCentroid = specCentroidList.Min(),
            maxSpecCentroid = specCentroidList.Max(),
            RMSList = rmsList,
            ZCRList = zcrList,
            SpecCentroidList = specCentroidList
        };
    }

    private void GetParameters(float[] pcm, float[] spec, out float rms, out float zcr, out float specCentroid, List<float> rmsList, List<float> zcrList, List<float> specCentroidList)
    {
        rms = RMS(pcm);
        zcr = ZCR(pcm);
        specCentroid = SpectralCentroid(spec, 44100);

        if (rms > 0) rmsList.Add(rms);
        if (zcr > 0) zcrList.Add(zcr);
        if (specCentroid > 0) specCentroidList.Add(specCentroid);
    }

    [Serializable]
    public struct AudioDataParameters
    {
        public float AverageRMS, AverageZCR, AverageSpecCentroid;
        public float minRMS, maxRMS, minZCR, maxZCR, minSpecCentroid, maxSpecCentroid;
        public List<float> RMSList, ZCRList, SpecCentroidList;
    }

}
