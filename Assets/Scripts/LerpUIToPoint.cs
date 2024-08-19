using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LerpUIToPoint : MonoBehaviour
{
    [SerializeField]
    Transform point;
    [SerializeField]
    float speed = 5f;
    public float timeElapsed = 0f;
    float timeToReachTarget = 1f;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, point.position, Time.deltaTime * speed);

        // if (timeElapsed < timeToReachTarget)
        // {
        //     float deltaTime = Time.deltaTime * speed;
        //     timeElapsed += deltaTime;
        //     float t = EasingFunctions.EaseInExpo(timeElapsed / timeToReachTarget);
        //     transform.position = Vector3.Lerp(transform.position, point.position, t);
        // }

        // if (Vector3.Distance(transform.position, point.position) > 0 && timeElapsed >= timeToReachTarget)
        //     timeElapsed = 0f;
    }
}
