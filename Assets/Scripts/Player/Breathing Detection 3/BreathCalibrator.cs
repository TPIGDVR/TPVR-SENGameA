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
    public AudioDataParameters silentParams;
    public AudioDataParameters speechParams;

    [Header("For Debug")]
    public AudioClip[] inhaleSample;
    public AudioClip[] exhaleSample;
    public AudioClip[] speechSample;
    public AudioClip silenceSample;

    public async Task<BreathSettings> BeginCalibrating()
    {
        inhaleData = new();
        exhaleData = new();
        instructionText.text = "Beginning Calibration...";
        await Countdown(3);

        instructionText.text = "Keep your environment quiet in...";
        await Countdown(3);

        instructionText.text = "Silence...";
        // PlayAudio(silenceSample);
        await CaptureData(silentData, 5);
        // source.Stop();
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        instructionText.text = "Read the incoming text out loud";
        await Countdown(3);
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        instructionText.text = "When life gives you lemons, yeet them at your enemies and assert dominance. 🍋💥";
        // PlayAudio(speechSample);
        await CaptureData(speechData, 5);
        // source.Stop();
        await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

        for (int i = 0; i < calibrationAmt; i++)
        {
            instructionText.text = "Get ready to inhale and exhale";
            await Countdown(3);
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

            instructionText.text = "Inhale";
            // PlayAudio(inhaleSample);
            await CaptureData(inhaleData, inhaleDuration);
            // source.Stop();
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));

            instructionText.text = "Exhale";
            // PlayAudio(exhaleSample);
            await CaptureData(exhaleData, exhaleDuration);
            // source.Stop();
            await Task.Delay(TimeSpan.FromSeconds(intervalBetweenInEx));
        }

        Debug.Log("Calibration complete");

        silentParams = GetAudioParameters(silentData, isSilent : true);
        speechParams = GetAudioParameters(speechData);
        inhaleParams = GetAudioParameters(inhaleData);
        exhaleParams = GetAudioParameters(exhaleData, isExhale : true);



        BreathSettings settings = new()
        {
            inRMSMinThres = inhaleParams.minRMS,

            //being used
            inRMSMaxThres = (inhaleParams.maxRMS + exhaleParams.AverageRMS) / 2, //get the inbetween value of inhale and exhale

            inZCRMaxThres = inhaleParams.maxZCR,

            //being used
            inZCRMinThres = inhaleParams.minZCR, //zcr needs to be higher than silence

            inSCMaxThres = inhaleParams.maxSpecCentroid,
            inSCMinThres = inhaleParams.minSpecCentroid,

            //exhale has issues being detected
            exRMSMaxThres = (exhaleParams.maxRMS + speechParams.AverageRMS) / 2,

            //being used
            exRMSMinThres = (exhaleParams.minRMS + silentParams.AverageRMS) / 2,
            exZCRMaxThres = (exhaleParams.maxZCR + speechParams.AverageZCR) / 2,


            exZCRMinThres = exhaleParams.minZCR,
            exSCMaxThres = exhaleParams.maxSpecCentroid,
            exSCMinThres = exhaleParams.minSpecCentroid,

            //being used
            //minimum value before needing to activate detection
            rmsMinThres = (silentParams.AverageRMS + (inhaleParams.AverageRMS + inhaleParams.minRMS) / 2) / 2, //get the inbetween value of silence and inhale
            rmsMaxThres = (speechParams.AverageRMS + exhaleParams.AverageRMS) / 2, //get the inbetween value of speech and exhale
        };

        // recorder.settings = settings;
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

    private AudioDataParameters GetAudioParameters(List<AudioData> dataList,bool isSilent = false,bool isExhale = false)
    {
        List<float> rmsList = new();
        List<float> zcrList = new();
        List<float> specCentroidList = new();

        float totalRMS = 0, totalZCR = 0, totalSpecCentroid = 0;

        foreach (var data in dataList)
        {           
            float[] pcm = data.PCMData;
            float[] spec = data.SpectrumData;

            GetParameters(pcm, spec, out float rms, out float zcr, out float specCentroid, rmsList, zcrList, specCentroidList,isSilent,isExhale);
            totalRMS += rms;
            totalZCR += zcr;
            totalSpecCentroid += specCentroid;
        }

        return new AudioDataParameters
        {
            AverageRMS = totalRMS / rmsList.Count,
            AverageZCR = totalZCR / zcrList.Count,
            AverageSpecCentroid = totalSpecCentroid / specCentroidList.Count,
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

    private void GetParameters(float[] pcm, float[] spec, out float rms, out float zcr, out float specCentroid,
    List<float> rmsList, List<float> zcrList, List<float> specCentroidList,
    bool isSilent = false, bool isExhale = false)
    {
        rms = RMS(pcm);
        zcr = ZCR(pcm);
        specCentroid = SpectralCentroid(spec, 44100);
        float rmsThres = isSilent? 0 : silentParams.AverageRMS;
        float zcrThres = isSilent? 0 : (silentParams.minZCR + silentParams.AverageZCR) / 2;
        bool rmsCon = rms > rmsThres;
        bool zcrCon = zcr > zcrThres;

        if (isExhale)
        {
            rmsCon = rms > rmsThres && rms < speechParams.maxRMS;
        }

        if (rmsCon) rmsList.Add(rms);
        if (zcrCon) zcrList.Add(zcr);
        if (specCentroid > 0) specCentroidList.Add(specCentroid);
    }

    #region Debug
    AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
       
    }

    void PlayAudio(AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    void PlayAudio(AudioClip[] clips)
    {
        source.Stop();
        int i = UnityEngine.Random.Range(0, clips.Length);
        source.clip = clips[i];
        source.Play();
    }

    [ContextMenu("Breath")]
    async void TestBreath()
    {
        int i = UnityEngine.Random.Range(0, 9);
        // source.clip = inhaleSample[i];
        source.PlayOneShot(inhaleSample[i]);
        await Task.Delay(TimeSpan.FromSeconds(2f));
        i = UnityEngine.Random.Range(0, 9);
        source.PlayOneShot(exhaleSample[i]);
    }

    [ContextMenu("Calibrate")]
    async void Calibrate()
    {
        await BeginCalibrating();
    }
    #endregion

    [Serializable]
    public struct AudioDataParameters
    {
        public float AverageRMS, AverageZCR, AverageSpecCentroid;
        public float minRMS, maxRMS, minZCR, maxZCR, minSpecCentroid, maxSpecCentroid;
        public List<float> RMSList, ZCRList, SpecCentroidList;
    }

}
