using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeartbeatHandler : MonoBehaviour
{
    [SerializeField] PulseScript pulse;
    [SerializeField] TMP_Text bpmText;
    float defaultFreq;
    float defaultSpeed;
    float defaultAmp;
    float restingBPM = 60;
    float maxBPM = 200;

    [Range(0,1)]public float anxietyLevel;
    float currFreq;
    float currSpd;
    float currBPM;

    float updateTiming;
    EventManager<Event> em = EventSystem.em;

    private void Start()
    {
        defaultFreq = pulse.Frequency;
        defaultSpeed = pulse.Speed;
        defaultAmp = pulse.Amplitude;
        //StartCoroutine(Heartbeat());
        em.AddListener<float>(Event.ANXIETY_UPDATE,UpdateHeartbeat);
    }
    
    IEnumerator Heartbeat()
    {
        while (true)
        {
            CalculateValues();
            UpdateUI(currFreq,currSpd,currBPM);
            yield return new WaitForSeconds(updateTiming);
        }
    }

    void UpdateHeartbeat(float anxVal)
    {
        anxietyLevel = anxVal;
        CalculateValues();
        UpdateUI(currFreq, currSpd, currBPM);
    }

    void CalculateValues()
    {
        currBPM = Mathf.Lerp(restingBPM, maxBPM, anxietyLevel);
        float bpmPercentage = currBPM / restingBPM;
        float freqMultiplier = 1;
        float spdMultiplier = 1;
        freqMultiplier += bpmPercentage * Random.Range(0.1f,1f);
        spdMultiplier += bpmPercentage * 2;

        currFreq = defaultAmp * freqMultiplier;
        currSpd = defaultSpeed * spdMultiplier;
    }

    void UpdateUI(float freq,float spd,float bpm)
    {
        //pulse.SetFrequency(freq);
        pulse.SetAmplitude(freq);
        pulse.SetSpeed(spd);
        bpmText.text = $"{Mathf.CeilToInt(bpm)} BPM";
    }
}
