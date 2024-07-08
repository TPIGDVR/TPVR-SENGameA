using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class AnxietyHandler : MonoBehaviour
{
    public float _anxietyLevel = 0;
    NoiseProximityHandler _noiseProximityHandler;
    
    [SerializeField]
    float _maxAnxietyLevel,_anxietyIncreaseSpeed;
    [SerializeField]
    float _minAnxietyIncreaseScale = 0;
    [SerializeField]
    float _maxAnxietyIncreaseScale = 3;
    float _anxietyIncreaseScale = 0;
    float _maxGlareValue = 1;
    float glareValue;

    
    [Range(0,1)]
    [SerializeField] float maxGlareForReduction = 0.1f;
    


    EventManager<Event> em = EventSystem.em;
    float curAnxiety => _anxietyLevel / _maxAnxietyLevel;

    [Header("reduction anxiety")]
    [SerializeField] float maxTimeReduction;
    [SerializeField] float anxietyReduction;
    float reduceElapseTime = 0f;
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
        _anxietyIncreaseScale = 0f;
        _anxietyIncreaseScale += CalculateAnxietyScaleBasedOffGlareLevel();
        _anxietyIncreaseScale += CalculateAnxietyScaleBasedOffNoiseLevel();

        if(_anxietyIncreaseScale < 0.01f)
        {
            reduceElapseTime += Time.deltaTime;
            if(reduceElapseTime > maxTimeReduction)
            {
                ReduceAnxietyLevel();        
            }
        }
        else
        {
            reduceElapseTime = 0;
            IncrementAnxietyLevel();
        }
        _anxietyLevel = Mathf.Clamp(_anxietyLevel,0, _maxAnxietyLevel);

        //trigger the event after calculating the anxiety level
        em.TriggerEvent(Event.ANXIETY_UPDATE);
        em.TriggerEvent<float>(Event.ANXIETY_UPDATE, _anxietyLevel / _maxAnxietyLevel);
    }

    float CalculateAnxietyScaleBasedOffNoiseLevel()
    {
        return Mathf.Lerp(_minAnxietyIncreaseScale
            ,_maxAnxietyIncreaseScale
            ,_noiseProximityHandler.TotalNoiseValue);
    }

    float CalculateAnxietyScaleBasedOffGlareLevel()
    {
        if(glareValue <= maxGlareForReduction)
        {
            //do change this if u adding the anxiety for noise;
            return 0f;
        }
        else
        {
            return Mathf.Lerp(_minAnxietyIncreaseScale
                , _maxAnxietyIncreaseScale
                , glareValue / _maxGlareValue);
        }
    }

    void IncrementAnxietyLevel()
    {
        _anxietyLevel += (Time.deltaTime * _anxietyIncreaseSpeed) * _anxietyIncreaseScale;
    }

    void ReduceAnxietyLevel()
    {
        _anxietyLevel -= Time.deltaTime * anxietyReduction;
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
