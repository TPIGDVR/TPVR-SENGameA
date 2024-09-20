using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LerpUIToPoint : MonoBehaviour
{
    [SerializeField]
    Transform point;
    [SerializeField]
    float speed = 5f;

    void Start()
    {
        transform.position = point.position;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, point.position, Time.deltaTime * speed);

    }
}
