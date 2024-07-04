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
    float _maxGlareValue = 1;
    float glareValue;

    [Header("Glare anxiety Reduction")]
    [Range(0,1)]
    [SerializeField] float maxGlareForReduction = 0.1f;
    [SerializeField] float glareAnxietyReduction = 0.1f;

    EventManager<Event> em = EventSystem.em;
    float curAnxiety => _anxietyLevel / _maxAnxietyLevel;
    private void Start()
    {
        _noiseProximityHandler = GetComponent<NoiseProximityHandler>();
        em.AddListener<float>(Event.ANXIETY_BREATHE, Breath);
        em.AddListener<float>(Event.GLARE_UPDATE, Glare);
        em.AddListener<float>(Event.HEART_BEAT, () => curAnxiety);
    }

    private void Update()
    {
        //CalculateAnxietyScaleBasedOffNoiseLevel();

        CalculateAnxietyScaleBasedOffGlareLevel();
        IncrementAnxietyLevel();

        _anxietyLevel = Mathf.Clamp(_anxietyLevel,0, _maxAnxietyLevel);
        //trigger the event after calculating the anxiety level
        em.TriggerEvent(Event.ANXIETY_UPDATE);
        em.TriggerEvent<float>(Event.ANXIETY_UPDATE, _anxietyLevel / _maxAnxietyLevel);
    }

    void CalculateAnxietyScaleBasedOffNoiseLevel()
    {
        _anxietyIncreaseScale = Mathf.Lerp(_minAnxietyIncreaseScale
            ,_maxAnxietyIncreaseScale
            ,_noiseProximityHandler.TotalNoiseValue / _maxNoiseLevel);
    }

    void CalculateAnxietyScaleBasedOffGlareLevel()
    {
        if(glareValue <= maxGlareForReduction)
        {
            //do change this if u adding the anxiety for noise;
            _anxietyIncreaseScale = Mathf.InverseLerp(0, maxGlareForReduction, glareValue) 
                * -glareAnxietyReduction ;
        }
        else
        {
            _anxietyIncreaseScale = Mathf.Lerp(_minAnxietyIncreaseScale
                , _maxAnxietyIncreaseScale
                , glareValue / _maxGlareValue);
        }
    }

    void IncrementAnxietyLevel()
    {
        _anxietyLevel += (Time.deltaTime * _anxietyIncreaseSpeed) * _anxietyIncreaseScale;
    }

    void Breath(float decrease)
    {
        _anxietyLevel *= decrease;
    }

    void Glare(float gv)
    {
        glareValue = gv;
    }

}
