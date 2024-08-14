using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    Transform parentControllerComponent;
    [SerializeField]
    float detectionRadius = 0.05f;

    Vector3 shiftOffset;

    private void Start()
    {
        shiftOffset = transform.position - parentControllerComponent.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentControllerComponent.position + shiftOffset;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.transform == parentControllerComponent)
            {
                Vector3 dir = (transform.position - parentControllerComponent.position).normalized;
                transform.position = parentControllerComponent.position + dir * detectionRadius;

                shiftOffset = transform.position - parentControllerComponent.position;
                break;
            }
        }
    }
}
