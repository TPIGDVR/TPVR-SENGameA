using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [SerializeField] Transform noiseIndicator;
    //[SerializeField] Material noiseIndicatorMaterial;
    public float NoiseValue = 10;
    AudioSource audio;
    private void Start()
    {
        audio = GetComponent<AudioSource>();
        noiseIndicator.localScale = new Vector3(NoiseValue, NoiseValue, NoiseValue);
        audio.maxDistance = NoiseValue * LevelConstants.Scale;
        audio.minDistance = 0;
    }
}
