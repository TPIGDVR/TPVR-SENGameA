using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioAnalyzer;

public class BreathCalibrator : MonoBehaviour
{
    BreathRecorder recorder;
    public float captureInterval = 0.02f;
    public float intervalBetweenInEx = 1f;
    public int calibrationAmt;
    public float inhaleDuration;
    public float exhaleDuration;
    List<float[]> inhaleData = new();
    List<float[]> exhaleData = new();
    // public static 

    IEnumerator BeginCalibrating()
    {
        for (int i = 0; i < calibrationAmt; i++)
        {
            yield return CaptureData(inhaleData, inhaleDuration); //capture inhale data
            yield return new WaitForSeconds(intervalBetweenInEx);
            yield return CaptureData(exhaleData, exhaleDuration); //capture exhale data
            yield return new WaitForSeconds(intervalBetweenInEx);
        }

        yield return CalibrateBreathingDetection();
        print("Calibration complete");
    }

    IEnumerator CaptureData(List<float[]> data, float recordTime)
    {
        float time = 0;
        while (time < recordTime)
        {
            data.Add(recorder.data);
            time += captureInterval;
            yield return new WaitForSeconds(captureInterval);
        }
    }

    IEnumerator CalibrateBreathingDetection()
    {
        //calibrate inhale
        List<float> inhaleRMS = new();
        List<float> inhaleZCR = new();
        float totalRMS = 0;
        float totalZCR = 0;
        foreach (var array in inhaleData)
        {
            //get rms calibration
            float rms = RMS(array);
            totalRMS += rms;
            if(rms > 0) //adjust threshold if needed
                inhaleRMS.Add(rms);
            yield return null;

            //get zcr calibration
            float zcr = ZCR(array);
            totalZCR += zcr;
            if(zcr > 0) //adjust threshold if needed
                inhaleZCR.Add(zcr);
        }
    }
}
