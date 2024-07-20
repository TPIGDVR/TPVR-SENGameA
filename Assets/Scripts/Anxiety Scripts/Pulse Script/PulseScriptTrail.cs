using Assets.Scripts.pattern;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class PulseScriptTrail : MonoBehaviour
{
    //TrailRenderer currentTrail;
    
    /// <summary>
    /// emission time determine how long the trail will last.
    /// NextPhaseRelease is the next pulse after the first pulse reach
    /// a certain phase
    /// Speed determine how fast the trail should be.
    /// </summary>

    [Header("trail details")]
    [SerializeField] TrailRenderer trailPrefab;
    [SerializeField] float emissionReduction = 0.2f;

    [SerializeField] float lengthOfTrail = 4;
    [SerializeField] float speed = 1;
    [Range(0, 360)]
    [SerializeField] float phaseReachBeforeNextTrail = 280;

    [SerializeField] int numberOfTrail;
    PoolingPattern<TrailRenderer> trails;

    [Header("Properties")]
    [SerializeField] float zeroOffsetAngle = 130f;
    [SerializeField] float WidthBoundingBox = 10f;
    
    [Header("Amp")]
    [SerializeField] float minAmp = 0.5f;
    [SerializeField] float maxAmp = 1f;
    [SerializeField] float randAmpRange = 0.3f;
    [SerializeField] float ampOffset;

    [Header("Frequency")]
    [SerializeField] float minFreq = 1f;
    [SerializeField] float maxFreq = 1.2f;

    [Header("Heart Related")]
    [SerializeField] float restingBPM = 60;
    [SerializeField] float maxBPM = 200;
    [SerializeField] int heartBeatRand = 2;
    [SerializeField] TMP_Text bpmText;
    EventManager<PlayerEvents> em = EventSystem.player;

    [Header("Pulse debugging")]
    //[SerializeField] private float phase;
    //[SerializeField] float frequency = 1f;
    //[SerializeField] float amp;
    //[SerializeField] int numberOfWave = 1;
    private bool hasActivatedEmit = false;
    private void Start()
    {
        trails = new(trailPrefab.gameObject);
        //add the color
        trails.InitWithParent(numberOfTrail, transform, true, transform.position);
        //currentTrail = trails.Get();
        //currentTrail.emitting = false;
        StartCoroutine(StartHeartBeatTrail());
    }

    IEnumerator StartHeartBeatTrail()
    {
        float anxiety = em.TriggerEvent<float>(PlayerEvents.HEART_BEAT);
        float currBPM = Mathf.Lerp(restingBPM, maxBPM, anxiety);
        currBPM += UnityEngine.Random.Range(-heartBeatRand, heartBeatRand);
        print(currBPM);
        int numberOfWaves = CalculateWave(currBPM);
        float speed = CalculateSpeed(currBPM);
        float frequency = CalculateFrequency();
        float amp = CalculateAmp(currBPM);
        bpmText.text = $"{Mathf.CeilToInt(currBPM)} BPM";

        //after calculating the speed,frequency and waves then we can plot it down;
        TrailRenderer renderer = trails.Get();
        Transform renderTransform = renderer.transform;
        renderTransform.localPosition = Vector3.zero;
        yield return null;
        renderer.emitting = true;
        renderer.time = CalculateEmissionTime(speed);

        bool hasSendOutNextTrail = false;

        while (renderTransform.localPosition.x < lengthOfTrail)
        {
            float phase = DeterminePhase(renderTransform.localPosition.x);

            if(phase > phaseReachBeforeNextTrail && !hasSendOutNextTrail)
            {
                hasSendOutNextTrail = true;
                StartCoroutine(StartHeartBeatTrail());
            }


            renderTransform.localPosition = new Vector3(
                renderTransform.localPosition.x + speed * Time.deltaTime,
                PulseWave(phase,numberOfWaves,frequency),
                0
                );
                
            yield return null;
        }
        if (!hasSendOutNextTrail)
        {
            StartCoroutine(StartHeartBeatTrail());
        }
        //keep the trail;
        renderer.emitting = false;
        trails.Retrieve(renderer);
    }
    float DeterminePhase(float x)
    {
        x = Mathf.Clamp(x,0, lengthOfTrail);
        return x / lengthOfTrail * 360;
    }



    #region sin wave related
    float ArcTooth(float degrees)
    {
        return Mathf.Atan(Mathf.Tan(degrees / 2 * Mathf.Deg2Rad));
    }

    float PulseWave(float degree , int numberOfWave , float frequency)
    {
        float maxdegree = 360 / numberOfWave;
        degree %= maxdegree;
        return HeartBeatPulse( NormaliseAngle(0, maxdegree, degree) , frequency);
    }

    float HeartBeatPulse(float degree, float frequency)
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


    #endregion

    /// <summary>
    /// calculate the speed based on 
    /// </summary>
    /// <param name="numberOfBeatPerMin"></param>
    public float CalculateSpeed(float numberOfBeatPerMin)
    {
        float weightOfSpeed = numberOfBeatPerMin / maxBPM;
        return weightOfSpeed * speed;
    }

    public float CalculateEmissionTime(float speed)
    {
        float estTimeToComplete = lengthOfTrail / speed;
        estTimeToComplete -= emissionReduction / speed;
        return Math.Max(estTimeToComplete, 0.1f);
    }

    public float CalculateAmp(float numberOfBeatPerMin)
    {
        float calAmp = Mathf.Lerp(minAmp,
            maxAmp,
            Mathf.InverseLerp(restingBPM, maxBPM, numberOfBeatPerMin));

        calAmp *= UnityEngine.Random.Range(0.5f, randAmpRange);
        return calAmp * ampOffset;
    }
    public int CalculateWave(float numberOfBeatPerMin)
    {
         return Math.Clamp((int)(numberOfBeatPerMin / 60f) , 1 ,3 );
    }

    public float CalculateFrequency()
    {
        return UnityEngine.Random.Range(minFreq, maxFreq);
    }
    
}
