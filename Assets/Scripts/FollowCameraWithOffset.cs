using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraWithOffset : MonoBehaviour
{
    private Transform mainCameraTransform;
    public float distance = 2.0f; // Adjust the fixed distance as needed.
    public float rightOffset = 0.5f; // Adjust the right offset as needed.

    private void Start()
    {
        // Find the main camera in the scene.
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (mainCameraTransform != null)
        {
            // Calculate the new position by moving distance units in front of the camera and to the right.
            Vector3 newPosition = mainCameraTransform.position + mainCameraTransform.forward * distance + mainCameraTransform.right * rightOffset;
            transform.position = newPosition;

            // Make the child object face the camera.
            transform.LookAt(mainCameraTransform);
        }
    }
}

