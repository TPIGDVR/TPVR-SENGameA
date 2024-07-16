using Assets.Scripts.pattern;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class PulseScriptTrail : MonoBehaviour
{
    TrailRenderer currentTrail;
    Transform trailTransform => currentTrail.transform;
    [Header("trail details")]
    [SerializeField] TrailRenderer trailPrefab;
    [SerializeField] float originalEmittingTime = 0.3f;
    [SerializeField] float emittingReduction = 0.2f;
    [SerializeField] float maxReductionEmittingTIme = 0.1f;
    [SerializeField] int numberOfTrail;
    PoolingPattern<TrailRenderer> trails;


    [Header("Properties")]
    [SerializeField] float zeroOffsetAngle = 130f;
    [SerializeField] float WidthBoundingBox = 10f;
    
    [Header("Amp")]
    [SerializeField] float minAmp = 0.5f;
    [SerializeField] float maxAmp = 1f;
    [SerializeField] float randAmpRange = 0.3f;

    [Header("Frequency")]
    [SerializeField] float minFreq = 1f;
    [SerializeField] float maxFreq = 1.2f;

    [Header("Heart Related")]
    [SerializeField] float restingBPM = 60;
    [SerializeField] float maxBPM = 200;
    [SerializeField] int heartBeatRand = 2;
    [SerializeField] TMP_Text bpmText;
    EventManager<PlayerEvents> em = EventSystem.player;

    float halfBoundBox => WidthBoundingBox / 2;

    [Header("Pulse debugging")]
    [SerializeField] float speed;
    [SerializeField] private float phase;
    [SerializeField] float frequency = 1f;
    [SerializeField] float amp;
    
    Vector3 trailNewPosition;
    Vector3 originalPos => new Vector3(-halfBoundBox, 0, 0);

    [SerializeField] int numberOfWave = 1;
    private bool hasActivatedEmit = false;
    private void Start()
    {
        trails = new(trailPrefab.gameObject);
        trails.InitWithParent(numberOfTrail, transform, true);
        phase = 0;
        currentTrail = trails.Get();
        currentTrail.emitting = false;
    }

    private void Update()
    {
        if(!hasActivatedEmit)
        {
            currentTrail.emitting = true;
            hasActivatedEmit = true;
        }
        trailNewPosition = trailTransform.localPosition;
        trailNewPosition.x += speed * Time.deltaTime;

        //clamp the new position value
        if (trailNewPosition.x > halfBoundBox)
        {
            ChangeCurrentTrail();
            trailNewPosition.x = -halfBoundBox;
        }

        DeterminePhase(trailNewPosition.x);
        float yOffset = amp * PulseWave(phase);
        trailNewPosition.y = yOffset;

        trailTransform.localPosition = trailNewPosition;
    }

    void ChangeCurrentTrail()
    {
        currentTrail.emitting = false;

        trails.Retrieve(currentTrail);
        currentTrail = trails.Get();
        trailTransform.localPosition = originalPos;
        currentTrail.time = originalEmittingTime - Mathf.Min(maxReductionEmittingTIme, emittingReduction * speed);

        hasActivatedEmit = false;

        //calculate the currentBPM
        float anxiety = em.TriggerEvent<float>(PlayerEvents.HEART_BEAT);
        CalBeat(anxiety);

    }
    #region sin wave related
    float ArcTooth(float degrees)
    {
        return Mathf.Atan(Mathf.Tan(degrees / 2 * Mathf.Deg2Rad));
    }

    float PulseWave(float degree)
    {
        float maxdegree = 360 / numberOfWave;
        degree %= maxdegree;
        return HeartBeatPulse( NormaliseAngle(0, maxdegree, degree));
    }

    float HeartBeatPulse(float degree)
    {
        if (degree < zeroOffsetAngle || degree > (360 - zeroOffsetAngle))
        {
            //we want the pulse to be zero by then
            return 0;
        }

        return ArcTooth(frequency * NormaliseAngle(zeroOffsetAngle, 360 - zeroOffsetAngle, degree));
    }

    float NormaliseAngle(float minAngle, float maxAngle, float currentAngle)
    {
        maxAngle -= minAngle;
        currentAngle -= minAngle;
        return Mathf.InverseLerp(0, maxAngle, currentAngle) * 360;
    }

    void DeterminePhase(float x)
    {
        float width = halfBoundBox;

        phase = 360 *
            Mathf.InverseLerp(-width, width,
            Mathf.Clamp(x, -width, width));
    }
    #endregion

    /// <summary>
    /// calculate the speed based on 
    /// </summary>
    /// <param name="numberOfBeatPerMin"></param>
    public void CalculateSpeed(float numberOfBeatPerMin)
    {
        float timeToCompleteOneBeat = 60 / numberOfBeatPerMin;
        speed = WidthBoundingBox / timeToCompleteOneBeat;
    }

    public void CalculateAmp(float numberOfBeatPerMin, float minHeartBeat ,float maxHeartBeat) 
    {
        float calAmp = Mathf.Lerp(minAmp,
            maxAmp, 
            Mathf.InverseLerp(minHeartBeat, maxHeartBeat, numberOfBeatPerMin));

        calAmp += UnityEngine.Random.Range(-randAmpRange, randAmpRange);
        amp = calAmp;
    }
    public void CalculateWave(float numberOfBeatPerMin)
    {
        numberOfWave = (int)(numberOfBeatPerMin / 60f);
    }

    public void CalculateWidthBounding(float numberOfBeatPerMin)
    {
        WidthBoundingBox = numberOfBeatPerMin / 60f;
    }

    public void CalculateFrequency()
    {
        frequency = UnityEngine.Random.Range(minFreq, maxFreq);
    }

    void CalBeat(float anxiety)
    {
        float currBPM = Mathf.Lerp(restingBPM, maxBPM, anxiety);
        CalculateWave(currBPM);
        CalculateWidthBounding(currBPM);
        currBPM += UnityEngine.Random.Range(-heartBeatRand, heartBeatRand);

        CalculateSpeed(currBPM);
        CalculateAmp(currBPM, restingBPM, maxBPM);
        CalculateFrequency();
        


        bpmText.text = $"{Mathf.CeilToInt(currBPM)} BPM"; ;
    }
}
