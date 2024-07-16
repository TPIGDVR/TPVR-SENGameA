using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [SerializeField] Transform noiseIndicator;
    [SerializeField] CapsuleCollider c;
    public float NoiseRange = 10;
    public float NoiseValue = 5;
    public float NoiseRangeScaled;
    AudioSource audio;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
        noiseIndicator.localScale = new Vector3(NoiseRange, NoiseRange, NoiseRange);
        NoiseRangeScaled = (NoiseRange + c.radius) * LevelConstants.Scale;
        audio.maxDistance = NoiseRangeScaled;
        audio.minDistance = 0;
    }
}
