using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class NoiseProximityHandler : MonoBehaviour
{
    NoiseSource[] _noiseSources;
    [SerializeField]
    float _maxDistanceFromSource;

    float _totalNoiseValue;
    float _maxSqrDist;
    [SerializeField]
    int numberOfSample = 64;
    [SerializeField]
    float[] _dataSample;

    private void Start()
    {
        _dataSample = new float[numberOfSample];
        _noiseSources = FindObjectsOfType<NoiseSource>();
        _maxSqrDist = _maxDistanceFromSource * _maxDistanceFromSource;
    }

    private void Update()
    {
        _totalNoiseValue = CalculateNoiseBasedOnListener();
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
        //Debug.Log(totalNoiseLevel);
        return totalNoiseLevel;
    }


    float CalculateNoiseBasedOnListener()
    {
        AudioListener.GetOutputData(_dataSample,0);
        float sum = 0;
        for (int i = 0; i < _dataSample.Length; i++)
        {
            sum += Mathf.Pow(_dataSample[i], 2);//Mathf.Abs(dataContainer[i]);
        }
        
        return Mathf.Sqrt(sum / numberOfSample) ;
    }


    public float TotalNoiseValue { get => _totalNoiseValue; }
}
