using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class AnxietyHandler : MonoBehaviour
{
    public float _anxietyLevel = 0;
    NoiseProximityHandler _noiseProximityHandler;

    [SerializeField]
    float _maxNoiseLevel;

    [SerializeField]
    float _maxAnxietyLevel,_anxietyIncreaseSpeed;
    [SerializeField]
    float _minAnxietyIncreaseScale = 0;
    [SerializeField]
    float _maxAnxietyIncreaseScale = 3;
    float _anxietyIncreaseScale = 0;


    EventManager em = EventManager.Instance;

    private void Start()
    {
        _noiseProximityHandler = GetComponent<NoiseProximityHandler>();
        em.AddListener<float>(Event.ANXIETY_BREATHE, Breath);
    }

    private void Update()
    {
        CalculateAnxietyScaleBasedOffNoiseLevel();
        IncrementAnxietyLevel();

        _anxietyLevel = Mathf.Clamp(_anxietyLevel,0, _maxAnxietyLevel);
        //trigger the event after calculating the anxiety level
        em.TriggerEvent<float>(Event.ANXIETY_UPDATE, _anxietyLevel / _maxAnxietyLevel);
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

    void Breath(float decrease)
    {
        _anxietyLevel *= decrease;
    }

}
