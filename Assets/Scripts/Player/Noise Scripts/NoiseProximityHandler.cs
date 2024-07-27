using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class NoiseProximityHandler : MonoBehaviour
{
    [SerializeField]NoiseSource[] _noiseSources;

    [SerializeField] float _totalNoiseValue;

    private void Start()
    {
        _noiseSources = FindObjectsOfType<NoiseSource>();        
    }

    private void Update()
    {
        _totalNoiseValue = CalculateNoiseLevelBasedOffDistance();

    }

    float CalculateNoiseLevelBasedOffDistance()
    {
        float totalNoiseLevel = 0;
        Transform camTrans = Camera.main.transform;
        foreach (var source in _noiseSources)
        {
            float dist = Vector3.Distance(camTrans.position,source.transform.position);

            if (source.CheckIfBlockedOrOutOfRange())
                continue;

            float noiseLevel = Mathf.Lerp(source.NoiseValue, 0, dist/source.NoiseRangeScaled);
            totalNoiseLevel += noiseLevel;          
        }

        return totalNoiseLevel;
    }

    public float TotalNoiseValue { get => _totalNoiseValue; }
}
