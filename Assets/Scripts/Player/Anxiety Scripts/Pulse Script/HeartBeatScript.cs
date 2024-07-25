using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartBeatScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI text;
    EventManager<PlayerEvents> em = EventSystem.player;
    float anxiety => em.TriggerEvent<float>(PlayerEvents.HEART_BEAT);

    [Header("Heart beat")]
    [SerializeField] float minHeartBeat = 60;
    [SerializeField] float maxHeartBeat = 180;
    [SerializeField] int heartBeatRand = 1;

    public void ChangeHeartBeat()
    {
        int currBPM = (int) Mathf.Lerp(minHeartBeat, maxHeartBeat, anxiety);
        currBPM += UnityEngine.Random.Range(-heartBeatRand, heartBeatRand);
        print($"CurrBPM {currBPM}");
        float speed = currBPM / minHeartBeat;
        animator.speed = speed;
        text.text = $"{currBPM} BPM";
    }
}
