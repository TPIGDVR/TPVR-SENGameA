using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    Transform parentControllerComponent;
    [SerializeField]
    float detectionRadius = 0.001f;

    [SerializeField]
    float maxDistanceFromController = 0.1f;
    [SerializeField]
    float distLimiter = 10f;
    Camera mainCamera;
    [SerializeField]
    Vector3 shiftOffset;
    Ray ray;
    [SerializeField]
    LayerMask layerMask;
    void Start()
    {
        mainCamera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 dirAwayFromController = (transform.position - parentControllerComponent.position).normalized;
        dirAwayFromController /= distLimiter;
        Vector3 dirTowardsCamera = (mainCamera.transform.position - transform.position).normalized;
        ray = new Ray(mainCamera.transform.position, -dirTowardsCamera);
        dirTowardsCamera += shiftOffset;
        dirTowardsCamera.Normalize();
        dirTowardsCamera /= 20f;
        bool hasHit = Physics.Raycast(ray, out RaycastHit hitinfo, 5f, layerMask);
        //if(hasHit)
        //    Debug.Log(hitinfo.collider.name);
        hasHit = false; //Band-aid patch for now
        Vector3 upwardOffset = hasHit? Vector3.up * 0.2f : Vector3.zero;
        //transform.localPosition = Vector3.Lerp(transform.position, parentControllerComponent.position + dirAwayFromController + dirTowardsCamera + upwardOffset,math.saturate(Time.deltaTime * 5f));
        transform.localPosition = parentControllerComponent.position + shiftOffset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}

