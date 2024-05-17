using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [Range(0, 10), SerializeField]
    float _noiseValue;
    
    public float NoiseValue { get => _noiseValue; }
    public Vector3 Position { get => transform.position; }
}
