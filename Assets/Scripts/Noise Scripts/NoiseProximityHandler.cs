using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class NoiseProximityHandler : MonoBehaviour
{
    [SerializeField]NoiseSource[] _noiseSources;
    [SerializeField]
    float _maxDistanceFromSource;
    [SerializeField] float _totalNoiseValue;
    float _maxSqrDist;

    //[Header("Experimental Ignore")]
    //[SerializeField]
    //int numberOfSample = 64;
    //[SerializeField]
    //float[] _dataSample;

    //[SerializeField]
    //float sampleSize;
    //List<float> avgNoise; 
    private void Start()
    {
        _noiseSources = FindObjectsOfType<NoiseSource>();
        _maxSqrDist = _maxDistanceFromSource * _maxDistanceFromSource;
        
        
        //_dataSample = new float[numberOfSample];

        
    }

    private void Update()
    {
        _totalNoiseValue = CalculateNoiseLevelBasedOffDistance();

        //Ignore this code (only use if it looks cool :))
        //_totalNoiseValue = CalculateNoiseBasedOnListener();

        //if (avgNoise.Count >= sampleSize)
        //{
        //    avgNoise.RemoveAt(0);
        //}

        //float noise = 0f;
        //avgNoise.Add(TotalNoiseValue);
        //for(int i = 0; i < avgNoise.Count; i++)
        //{
        //    noise += avgNoise[i];
        //}
        //noise /= avgNoise.Count;

        //print($"avg noise {noise}");
    }

    float CalculateNoiseLevelBasedOffDistance()
    {
        float totalNoiseLevel = 0;
        foreach (var source in _noiseSources)
        {
            float sqrDistance = Mathf.Abs(Vector3.SqrMagnitude(source.transform.position - Camera.main.transform.position));

            //skip if out of distance
            //if (sqrDistance > _maxSqrDist) continue;

            //lerp to its noise value based off distance;
            float noiseLevel = Mathf.Lerp(source.NoiseValue, 0,sqrDistance / _maxSqrDist);
            totalNoiseLevel += noiseLevel;
            
        }
        //Debug.Log(totalNoiseLevel);
        return totalNoiseLevel;
    }


    //float CalculateNoiseBasedOnListener()
    //{
    //    AudioListener.GetOutputData(_dataSample,0);
    //    float sum = 0;
    //    for (int i = 0; i < _dataSample.Length; i++)
    //    {
    //        sum += Mathf.Pow(_dataSample[i], 2);//Mathf.Abs(dataContainer[i]);
    //    }
        
    //    return Mathf.Sqrt(sum / numberOfSample) ;
    //}


    public float TotalNoiseValue { get => _totalNoiseValue; }
}
