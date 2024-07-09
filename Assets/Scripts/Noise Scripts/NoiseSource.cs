using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [SerializeField] Transform noiseIndicator;
    public float NoiseValue = 10 ;

    private void Start()
    {
        
        noiseIndicator.localScale = new Vector3(NoiseValue, 0, NoiseValue) / transform.lossyScale.x;
    }
}
