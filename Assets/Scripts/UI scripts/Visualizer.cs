using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Visualizer : MonoBehaviour
{
    public Transform head;
    public float maxDistance = 2f;
    public LayerMask obstructionLayer;
    public GameObject visualizer;
    public Vector3 offset = Vector3.zero; // Offset vector for centering the visualizer.
    public InputActionProperty showButton;

    void Update()
    {
        if (showButton.action.WasPerformedThisFrame())
        {
            visualizer.SetActive(!visualizer.activeSelf);
        }

        // Run the position of the visualizer based on the head's position, direction, and offset.
        Vector3 playerPos = head.position;
        Vector3 direction = head.forward;
        Vector3 targetPosition = playerPos + direction * maxDistance + offset;

        // Perform a raycast to detect obstructions.
        RaycastHit hit;
        if (Physics.Raycast(playerPos, direction, out hit, maxDistance, obstructionLayer))
        {
            // Adjust the Canvas position to be in front of the obstruction.
            targetPosition = hit.point - direction.normalized * 0.1f;

            // Debug the ray by drawing it.
            Debug.DrawRay(playerPos, direction * hit.distance, Color.red);
        }
        else
        {
            // No obstructions, keep the Canvas at the desired distance from the player.
            Debug.DrawRay(playerPos, direction * maxDistance, Color.green);
        }


        // Update the position of the visualizer to the new target position.
        visualizer.transform.position = targetPosition;

        // Update the rotation of the visualizer to match the head's rotation.
        visualizer.transform.rotation = head.rotation;
    }

}
