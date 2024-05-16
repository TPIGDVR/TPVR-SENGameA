using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class AnxietyHandler : MonoBehaviour
{
    float _anxietyLevel = 0;
    NoiseProximityHandler _noiseProximityHandler;

    [SerializeField]
    float _maxNoiseLevel;

    [SerializeField]
    float _anxietyIncreaseSpeed;
    [SerializeField]
    float _minAnxietyIncreaseScale = 0;
    [SerializeField]
    float _maxAnxietyIncreaseScale = 3;
    float _anxietyIncreaseScale = 0;



    private void Start()
    {
        _noiseProximityHandler = GetComponent<NoiseProximityHandler>();
    }

    private void Update()
    {
        CalculateAnxietyScaleBasedOffNoiseLevel();
        IncrementAnxietyLevel();
    }

    void CalculateAnxietyScaleBasedOffNoiseLevel()
    {
        _anxietyIncreaseScale = Mathf.Lerp(_minAnxietyIncreaseScale
            ,_maxAnxietyIncreaseScale
            ,_noiseProximityHandler.TotalNoiseValue / _maxNoiseLevel);
    }

    void IncrementAnxietyLevel()
    {
        _anxietyLevel += (Time.deltaTime * _anxietyIncreaseSpeed) * _anxietyIncreaseScale;
    }

    public float AnxietyLevel { get { return _anxietyLevel; } }
}
