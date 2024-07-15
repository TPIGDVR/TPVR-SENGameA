using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.Processors;

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
    [SerializeField]
    float _deathTime;
    float _anxietyIncreaseScale = 0;
    float _maxGlareValue = 1;
    float glareValue;
    [SerializeField]
    float deathTimer;
    bool isDead;

    
    [Range(0,1)]
    [SerializeField] float maxGlareForReduction = 0.1f;
    


    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<GameEvents> em_g = EventSystem.game;
    float curAnxiety => _anxietyLevel / _maxAnxietyLevel;

    [Header("reduction anxiety")]
    [SerializeField] float maxTimeReduction;
    [SerializeField] float anxietyReduction;
    float reduceElapseTime = 0f;
    private void Start()
    {
        _noiseProximityHandler = GetComponent<NoiseProximityHandler>();
        em_p.AddListener<float>(PlayerEvents.ANXIETY_BREATHE, Breath);
        em_p.AddListener<float>(PlayerEvents.GLARE_UPDATE, Glare);
        em_p.AddListener<float>(PlayerEvents.HEART_BEAT, () => curAnxiety);
    }

    private void Update()
    {
        if (_anxietyLevel >= _maxAnxietyLevel)
        {
            deathTimer += Time.deltaTime;
        }
        else
        {
            deathTimer -= Time.deltaTime;
        }
        deathTimer = Mathf.Clamp(deathTimer, 0, _deathTime);

        if (deathTimer >= _deathTime && !isDead)
        {
            isDead = true;
            em_g.TriggerEvent(GameEvents.LOSE);
        }


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
        em_p.TriggerEvent(PlayerEvents.ANXIETY_UPDATE);
        em_p.TriggerEvent<float>(PlayerEvents.ANXIETY_UPDATE, _anxietyLevel / _maxAnxietyLevel);
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
