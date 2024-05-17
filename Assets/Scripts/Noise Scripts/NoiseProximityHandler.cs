using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoiseProximityHandler : MonoBehaviour
{
    NoiseSource[] _noiseSources;
    [SerializeField]
    float _maxDistanceFromSource;

    float _totalNoiseValue;
    float _maxSqrDist;
    private void Start()
    {
        _noiseSources = FindObjectsOfType<NoiseSource>();
        _maxSqrDist = _maxDistanceFromSource * _maxDistanceFromSource;

    }

    private void Update()
    {
        _totalNoiseValue = CalculateNoiseLevelBasedOffDistance();
    }

    float CalculateNoiseLevelBasedOffDistance()
    {
        float totalNoiseLevel = 0;
        foreach (var source in _noiseSources)
        {
            float sqrDistance = Vector3.SqrMagnitude(source.Position - transform.position);

            //skip if out of distance

            //lerp to its noise value based off distance;
            float noiseLevel = Mathf.Lerp(0,source.NoiseValue,sqrDistance / _maxSqrDist);
            totalNoiseLevel += noiseLevel;
        }

        return totalNoiseLevel;
    }

    public float TotalNoiseValue { get => _totalNoiseValue; }
}
