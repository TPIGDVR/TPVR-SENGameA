using Assets.Scripts.pattern;
using UnityEngine;
public class PulseScriptTrail : MonoBehaviour
{
    //task
    /// <summary>
    /// 1. Bound the trail to a certain width
    /// </summary>
    /// 
    TrailRenderer currentTrail;
    Transform trailTransform => currentTrail.transform;
    [SerializeField] TrailRenderer trailPrefab;
    [SerializeField] int numberOfTrail;
    PoolingPattern<TrailRenderer> trails;

    [SerializeField] float zeroOffsetAngle = 130f;
    [SerializeField] float WidthBoundingBox = 10f;
    float halfBoundBox => WidthBoundingBox / 2;

    //starting point is 0,
    [SerializeField] float speed;
    [SerializeField] private float phase;
    [SerializeField] float frequency;
    [SerializeField] float amp;
    Vector3 trailNewPosition;
    Vector3 originalPos => new Vector3(-halfBoundBox, 0, 0);
    private void Start()
    {
        trails = new(trailPrefab.gameObject);
        trails.InitWithParent(numberOfTrail, transform);
        phase = 0;
        currentTrail = trails.Get();
    }

    private void Update()
    {
        trailNewPosition = trailTransform.localPosition;
        trailNewPosition.x += speed * Time.deltaTime;

        //clamp the new position value
        if (trailNewPosition.x > halfBoundBox)
        {
            ChangeCurrentTrail();
            trailNewPosition.x = -WidthBoundingBox;
        }

        DeterminePhase(trailNewPosition.x);
        float yOffset = amp * PulseWave(phase);
        trailNewPosition.y = yOffset;

        trailTransform.localPosition = trailNewPosition;
    }

    void ChangeCurrentTrail()
    {
        currentTrail.gameObject.SetActive(false);
        trailTransform.localPosition = originalPos;

        trails.Retrieve(currentTrail);
        currentTrail = trails.Get();

        currentTrail.gameObject.SetActive(true);
    }

    float ArcTooth(float degrees)
    {

        return Mathf.Atan(Mathf.Tan(degrees / 2 * Mathf.Deg2Rad));
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
        return Mathf.InverseLerp(0, maxAngle, currentAngle) * 360;
    }


    void DeterminePhase(float x)
    {
        float width = halfBoundBox;

        phase = 360 *
            Mathf.InverseLerp(-width, width,
            Mathf.Clamp(x, -width, width));
    }
}
