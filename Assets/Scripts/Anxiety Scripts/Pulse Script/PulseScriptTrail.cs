using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.UIElements;

public class PulseScriptTrail : MonoBehaviour
{
    //task
    /// <summary>
    /// 1. Bound the trail to a certain width
    /// </summary>
    /// 
    Transform currentTrail;
    [SerializeField] Transform trail;
    [SerializeField] Transform trail2;
    TrailRenderer trailRenderer;
    TrailRenderer trailRenderer2;
    [SerializeField] float zeroOffsetAngle = 10f;
    [Range(-1, 1)]
    [SerializeField] float yInitOffset = -0.5f;
    [SerializeField] float WidthBoundingBox = 10f;
    float halfBoundBox => WidthBoundingBox / 2;

    //starting point is 0,
    [SerializeField] float speed;
    [SerializeField] private float phase;
    [SerializeField] float frequency;
    [SerializeField] float amp;
    Vector3 trailNewPosition;
    private void Start()
    {
        trail.localPosition = new Vector3(-halfBoundBox, 0, 0);
        trailRenderer = trail.GetComponent<TrailRenderer>();

        trail2.localPosition = new Vector3(-halfBoundBox, 0, 0);
        trailRenderer2 = trail2.GetComponent<TrailRenderer>();

        currentTrail = transform;

        phase = 0;
    }

    private void Update()
    {
        currentTrail.localPosition = trail.localPosition;
        float newPhase = speed * Time.deltaTime;

        trailNewPosition.x += newPhase;

        phase += newPhase;
        phase %= 360;
        float yOffset = amp * PulseWave(frequency * phase);
        trailNewPosition.y = yOffset;
        //clamp the new position value
        if(trailNewPosition.x > halfBoundBox)
        {
            
        }
        else
        {
            trailRenderer.emitting = true;
        }
        trail.localPosition = trailNewPosition;
    }

    void UpdateSinWave()
    {
        phase += Time.deltaTime;
        //float increaseConst = 360 / (sizeAccuracy - 1);
        //for (int i = 0; i < sizeAccuracy; ++i)
        //{
        //    float positionForPoint = increaseConst * i;
        //    float yOffset = amp * PulseWave(frequency * (positionForPoint + phase * speed));
        //    positions[i].y = yOffset;
        //}
        //lineRenderer.SetPositions(positions);

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
            return yInitOffset;
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
