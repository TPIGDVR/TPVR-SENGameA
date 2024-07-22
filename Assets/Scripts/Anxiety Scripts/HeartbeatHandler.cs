using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeartbeatHandler : MonoBehaviour
{
    //[SerializeField] PulseScriptTrailNew pulse;
    //[SerializeField] TMP_Text bpmText;
    //[SerializeField] int heartBeatRand = 2;
    //float restingBPM = 60;
    //float maxBPM = 200;

    //[Range(0,1)]public float anxietyLevel;
    //float currBPM;

    //float updateTiming;
    //EventManager<Event> em = EventSystem.em;

    //private void Start()
    //{
    //    em.AddListener<float>(Event.ANXIETY_UPDATE,UpdateHeartbeat);
    //}

    //void UpdateHeartbeat(float anxVal)
    //{
    //    anxietyLevel = anxVal;
    //    currBPM = Mathf.Lerp(restingBPM, maxBPM, anxietyLevel);
    //    currBPM += Random.Range(-heartBeatRand, heartBeatRand);
    //    UpdateUI();
    //}

    //void UpdateUI()
    //{
    //    bpmText.dialogText = $"{Mathf.CeilToInt(currBPM)} BPM";
    //    //pulse.changeHeartBeat = HeartBeatChange;
    //}

    //void HeartBeatChange()
    //{
    //    pulse.CalculateSpeed(currBPM);
    //    pulse.CalculateAmp(currBPM, restingBPM, maxBPM);
    //    pulse.CalculateFrequency();
    //}
}
