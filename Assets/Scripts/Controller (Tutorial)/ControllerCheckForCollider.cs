using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    SphereCollider sphereCollider;
    [SerializeField]
    Transform parentControllerComponent;

    Vector3 shiftOffset;

    private void Start()
    {
        shiftOffset = transform.position - parentControllerComponent.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentControllerComponent.position + shiftOffset;
        Vector3 closestPoint = sphereCollider.ClosestPoint(transform.position);
        if (Vector3.Distance(transform.position, closestPoint) < 0.001f)
        {
            transform.position = closestPoint;
        }
        shiftOffset = transform.position - parentControllerComponent.position;
    }
}
