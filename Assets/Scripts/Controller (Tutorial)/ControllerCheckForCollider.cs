using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    SphereCollider sphereCollider;
    [SerializeField]
    Transform parentControllerComponent;
    [SerializeField]
    float detectionRadius = 0.001f;

    Vector3 shiftOffset;

    private void Start()
    {
        shiftOffset = transform.position - sphereCollider.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sphereCollider.transform.position + shiftOffset;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider == sphereCollider)
            {
                Vector3 dir = (transform.position - sphereCollider.transform.position).normalized;
                transform.position = sphereCollider.transform.position + dir * detectionRadius;

                shiftOffset = transform.position - sphereCollider.transform.position;
                break;
            }
        }
    }
}
