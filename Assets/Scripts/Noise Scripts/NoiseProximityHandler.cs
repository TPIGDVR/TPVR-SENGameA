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
            float sqrDistance = Mathf.Abs(Vector3.SqrMagnitude(source.Position - Camera.main.transform.position));

            //skip if out of distance
            //if (sqrDistance > _maxSqrDist) continue;

            //lerp to its noise value based off distance;
            float noiseLevel = Mathf.Lerp(source.NoiseValue, 0,sqrDistance / _maxSqrDist);
            totalNoiseLevel += noiseLevel;
            
        }
        Debug.Log(totalNoiseLevel);
        return totalNoiseLevel;
    }

    public float TotalNoiseValue { get => _totalNoiseValue; }
}
