using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.UIElements;
using UnityEngine.Pool;
using Assets.Scripts.pattern;

public class PulseScriptTrail : MonoBehaviour
{
    //task
    /// <summary>
    /// 1. Bound the trail to a certain width
    /// </summary>
    /// 
    TrailRenderer currentTrail;
    Transform currentTrailTransform => currentTrail.transform;
    [SerializeField] TrailRenderer trailPrefab;
    [SerializeField] int numberOfTrail;
    PoolingPattern<TrailRenderer> trailsPool;

    [SerializeField] float originalTime = 0.3f;
    [SerializeField] float reductionTime = 0.01f;

    [SerializeField] float zeroOffsetAngle = 130f;
    [Range(-1, 1)]
    [SerializeField] float WidthBoundingBox = 10f;
    float halfBoundBox => WidthBoundingBox / 2;

    //starting point is 0,
    [SerializeField] float speed;
    [SerializeField] float amp;
    Vector3 trailNewPosition;
    Vector3 originalPos => new Vector3(-halfBoundBox, 0, 0);

    private void Start()
    {
        trailsPool = new PoolingPattern<TrailRenderer>(trailPrefab.gameObject);
        trailsPool.InitWithParent(numberOfTrail, transform);
        currentTrail = trailsPool.Get();
        currentTrail.gameObject.SetActive(true);
    }

    private void Update()
    {
        trailNewPosition = currentTrailTransform.localPosition;
        float newPhase = speed * Time.deltaTime;

        trailNewPosition.x += newPhase;

        float determinePhase = DeterminePhase(trailNewPosition.x);


        float yOffset = amp * PulseWave(determinePhase);
        trailNewPosition.y = yOffset;
        //clamp the new position value
        if(trailNewPosition.x > halfBoundBox)
        {
            ChangeCurrentTrail();
            trailNewPosition.x = -halfBoundBox;
        }

        currentTrailTransform.localPosition = trailNewPosition;
    }

    float DeterminePhase(float x)
    {
        float lengthOfBox = halfBoundBox;
        x = Mathf.Clamp(x, -lengthOfBox, lengthOfBox);
        return Mathf.Lerp(0, 360, Mathf.InverseLerp(-lengthOfBox, lengthOfBox, x));
    }

    void ChangeCurrentTrail()
    {
        currentTrail.emitting = false;
        currentTrail.gameObject.SetActive(false);
        currentTrailTransform.localPosition = originalPos;
        trailsPool.Retrieve(currentTrail);
        
        currentTrail = trailsPool.Get();

        currentTrail.emitting = true;
        currentTrail.time = originalTime - reductionTime * speed;
        currentTrail.gameObject.SetActive(true);

    }

    float ArcTooth(float degrees)
    {
        return Mathf.Atan(Mathf.Tan(degrees * 0.01f));
    }

    float PulseWave(float degrees)
    {
        degrees = Mathf.Abs(degrees) % 360;
        if (degrees < zeroOffsetAngle || degrees > (360 - zeroOffsetAngle))
        {
            //we want the pulse to be zero by then
            return 0;
        }

        return ArcTooth(NormaliseAngle(zeroOffsetAngle, 360 - zeroOffsetAngle, degrees));
    }

    float NormaliseAngle(float minAngle, float maxAngle, float currentAngle)
    {
        maxAngle -= minAngle;
        currentAngle -= minAngle;
        return 360 * Mathf.InverseLerp(0, maxAngle, currentAngle);
    }

}
