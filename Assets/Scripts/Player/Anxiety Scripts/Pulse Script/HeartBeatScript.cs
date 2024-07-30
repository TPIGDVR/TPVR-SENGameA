using SoundRelated;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartBeatScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI text_BPM;
    [SerializeField] Image heartBeatImage;
    EventManager<PlayerEvents> em = EventSystem.player;
    float anxiety => em.TriggerEvent<float>(PlayerEvents.HEART_BEAT);

    [Header("Heart beat")]
    [SerializeField] float minHeartBeat = 60;
    [SerializeField] float maxHeartBeat = 180;
    [SerializeField] int heartBeatRand = 1;
    [SerializeField] Gradient colorGradient;

    public void ChangeHeartBeat()
    {
        Debug.Log(anxiety);
        float curAnxiety = anxiety;
        int currBPM = (int) Mathf.Lerp(minHeartBeat, maxHeartBeat, curAnxiety);
        currBPM += UnityEngine.Random.Range(-heartBeatRand, heartBeatRand);
        float speed = currBPM / minHeartBeat;
        animator.speed = speed;

        //set ui component
        text.text = $"{currBPM}";
        heartBeatImage.color = colorGradient.Evaluate(curAnxiety);
        text.color = colorGradient.Evaluate(curAnxiety);
        text_BPM.color = colorGradient.Evaluate(curAnxiety);
        SoundManager.Instance.PlayAudioOneShot(SoundRelated.SFXClip.HEART_BEAT);
    }
}
