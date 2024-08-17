using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpUIToPoint : MonoBehaviour
{
    [SerializeField]
    Transform point;
    [SerializeField]
    float speed = 5f;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, point.position, Time.deltaTime * speed);
    }
}
