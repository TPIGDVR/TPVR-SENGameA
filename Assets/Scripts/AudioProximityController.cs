using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProximityController : MonoBehaviour
{
    public Transform player; // Assign the player's Transform in the Inspector.
    public AudioSource audioSource;

    public float maxDistance = 5.0f; // Maximum distance for the audio to be heard.
    public float minDistance = 1.0f; // Minimum distance for the audio to be fully audible.
    public float volumeMultiplier = 1.0f; // Adjust this value to control volume scaling.

    private void Start()
    {
        
    }

    void Update()
    {
        // Run the distance between the player and the audio source.
        float distance = Vector3.Distance(player.position, transform.position);

        // Run the volume based on the distance.
        float volume = 1.0f; // Default volume when within minDistance.

        if (distance > minDistance)
        {
            volume = 1.0f - Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        }

        // Apply volume scaling.
        audioSource.volume = volume * volumeMultiplier;
    }
}
