using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTutorialText : MonoBehaviour
{
    [SerializeField]
    private Vector3 originalPosition;
    [SerializeField]
    private Vector3 nextPosition;
    
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float lerpSpeed = 1000f;

    private Transform cameraTransform;
    private Vector3 targetPosition;

    private Transform localTransformPosition;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        EventSystem.level.AddListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        
        //we want to have a constant reference to the local position of the controller.
        localTransformPosition = new GameObject("LocalTransformPosition").transform;
        localTransformPosition.SetParent(transform.parent);
        localTransformPosition.localPosition = Vector3.zero;
        localTransformPosition.localRotation = Quaternion.identity;
        localTransformPosition.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
    }

    private void Update()
    {
        targetPosition = originalPosition;

        var worldPosition = localTransformPosition.TransformPoint(originalPosition);
        
        bool hasHit = RaycastFromPointToPoint(worldPosition, cameraTransform.position);
        Debug.DrawLine(worldPosition, cameraTransform.position, hasHit? Color.green : Color.red);
        
        if (!hasHit)
        {
            targetPosition = nextPosition;
        }

        targetPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.fixedDeltaTime * lerpSpeed);
        transform.localPosition = targetPosition;
    }

    private void OnEndTutorial()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        gameObject.SetActive(false);
    }

    private bool RaycastFromPointToPoint(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(from, direction, out RaycastHit hit, distance, mask))
        {
            return hit.collider.CompareTag("Player Head");
        }

        return false;
    }
}
