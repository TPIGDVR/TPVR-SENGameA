using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NoiseProducerAlert : MonoBehaviour
{
    public XRRayInteractor rayInteractor;

    private float leftTriggerPressed;
    public InputActionProperty leftPointing;
    private bool rightTap, leftTap;

    public LayerMask noiseLayer;

    public Transform head;
    public float maxDistance = 2f;
    public LayerMask obstructionLayer;
    public GameObject visualizer;
    public Vector3 offset = Vector3.zero; // Offset vector for centering the visualizer.
    public Outline outline;
    public Transform UItransform;

    public Transform newPosition;

    public float minScale = 0.5f; // Minimum scale when farthest from the target
    public float maxScale = 1f; // Maximum scale when closest to the target
    public float scaleSpeed = 0.5f; // Speed of scaling

    // Start is called before the first frame update
    void Start()
    {
        UItransform = GetComponent<Transform>();
    }

    public void FollowPlayer()
    {
        if (head != null)
        {
            // Find the direction vector from UI element to player
            Vector3 direction = head.position - transform.position;

            // Make sure the UI element doesn't tilt upwards/downwards
            direction.y = 0f;

            // Calculate rotation to face the player
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Apply rotation to the UI element
            transform.rotation = rotation;
        }

    }

    IEnumerator SpawnUI() 
    {
        Instantiate(visualizer, UItransform.position, Quaternion.identity);
        yield return null;
    }

    public void SizeChanger()
    {
        float distance = Vector3.Distance(transform.position, UItransform.position);

        // Calculate the desired scale based on the distance
        float desiredScale = Mathf.Lerp(maxScale, minScale, distance / maxScale);

        // Smoothly scale towards the desired scale
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * desiredScale, Time.deltaTime * scaleSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit noisehit;

        leftTriggerPressed = leftPointing.action.ReadValue<float>();
        if (Physics.Raycast(rayInteractor.transform.position, rayInteractor.transform.forward,out noisehit, 10 , noiseLayer))
        {
            GameObject hitObject = noisehit.collider.gameObject;
            hitObject.GetComponent<Outline>().enabled = true;
            if (leftTriggerPressed > 0)
            {
                hitObject.GetComponent<Outline>().enabled = false;
                newPosition = hitObject.transform;
                visualizer.SetActive(true);
                UItransform.position = newPosition.position;
                SpawnUI();
            }
            else
            {
            }
        }
        // Calculate the position of the visualizer based on the head's position, direction, and offset.
        Vector3 playerPos = head.position;
        Vector3 direction = head.forward;
        Vector3 targetPosition = playerPos + direction * maxDistance + offset;

        FollowPlayer();
        SizeChanger();
    }
}
