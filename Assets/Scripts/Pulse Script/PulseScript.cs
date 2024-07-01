using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PulseScript : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int sizeAccuracy = 20;
    [SerializeField] float width = 9f;
    [SerializeField] float phase = 0;

    [Header("wave information")]
    [SerializeField] float amp = 1f;
    [SerializeField] float frequency = 1f;
    [SerializeField] float speed = 1f;
    [SerializeField] float zeroOffsetAngle = 10f;
    //legacy
    //[SerializeField] AnimationCurve pulseWave;
    //[SerializeField] float lowerBoundAmp = 1f;
    //[SerializeField] float upperBoundAmp = 1.1f;
    //[Range(30, 180)]
    //[SerializeField] int middleWayPoint = 30;
    [Range(-1, 1)]
    [SerializeField] float yInitOffset = -0.5f;

    private Vector3[] positions;

    private void Start()
    {
        positions = new Vector3[sizeAccuracy];
        for(int i = 0; i < sizeAccuracy; ++i)
        {
            positions[i] = new Vector3((width/(sizeAccuracy - 1)) * i, 0, 0);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }


    private void Update()
    {
        UpdateSinWave();
    }

    void UpdateSinWave()
    {
        phase += Time.deltaTime;
        float increaseConst = 360 / (sizeAccuracy - 1);
        for(int i = 0; i < sizeAccuracy; ++i)
        {
            float positionForPoint = increaseConst * i;
            float yOffset = amp * PulseWave( frequency * ( positionForPoint + phase * speed));
            positions[i].y = yOffset;
        }
        lineRenderer.SetPositions(positions);

    }

    

    float ArcTooth(float degrees)
    {
        return Mathf.Atan(Mathf.Tan(degrees * 0.01f));
    }

    float PulseWave(float degrees)
    {
        degrees = Mathf.Abs(degrees) % 360;
        if(degrees < zeroOffsetAngle || degrees > (360 - zeroOffsetAngle) )
        {
            //we want the pulse to be zero by then
            return yInitOffset;
        }

        ////this will be the range lor 
        //if(degrees <= middleWayPoint)
        //{
        //    //normalise it
        //    return lowerBoundAmp * ArcTooth(NormaliseAngle(zeroOffsetAngle, middleWayPoint, degrees));
        //}
        //else
        //{
        //    return upperBoundAmp * ArcTooth(NormaliseAngle(middleWayPoint, 360 - zeroOffsetAngle, degrees));

        //}

        return ArcTooth(NormaliseAngle(zeroOffsetAngle, 360 - zeroOffsetAngle, degrees));


    }

    float NormaliseAngle(float minAngle,  float maxAngle , float currentAngle)
    {
        maxAngle -= minAngle;
        currentAngle -= minAngle;
        return 360 * Mathf.InverseLerp(0 , maxAngle, currentAngle);
    }
}

