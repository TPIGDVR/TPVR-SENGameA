
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class ScaleFromAudioClip : MonoBehaviour
{
    public AudioLoudnessDetection detector;
    public GameObject visualiserPrefab;
    public int sampleSize = 100;
    public float offset = 1f;
    private GameObject[] prefabReference;
    [SerializeField] float VoiceScale;

    Queue<float> loudnessQueue;
    //public Vector3 minScale;
    //public Vector3 maxScale;

    [SerializeField] TextMeshProUGUI text;
    public float breatheInThreshHold;
    public float breathOutThreshHold;
    //[SerializeField] float 

    [Header("Testing")]
    [SerializeField] bool isSampling = true;

    private void Start()
    {
        loudnessQueue = new();
        prefabReference = new GameObject[sampleSize];
        for(int i = 0; i < sampleSize; i++)
        {
            GameObject box = Instantiate(visualiserPrefab , transform);
            box.transform.localPosition = new Vector3( i * offset , 0, 0);
            prefabReference[i] = box;
        }
    }

    private void Update()
    {
        float loudness = detector.GetLoudnessFromMicroPhone() * VoiceScale;
        if (loudnessQueue.Count >= sampleSize)
        {
            loudnessQueue.Dequeue();
        }
        loudnessQueue.Enqueue(loudness);
        
        
        var copyArray = loudnessQueue.ToArray();

        DisplayVoice(copyArray);

    }

    private void DisplayVoice(float[] copyArray)
    {
        for (int i = 0; i < copyArray.Length; i++)
        {
            print(i);
            float yScale = copyArray[i];
            prefabReference[i].transform.localScale = new Vector3(1, yScale, 1f);
        }
    }
}
