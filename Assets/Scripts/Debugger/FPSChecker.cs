using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSChecker : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Reference to a UI Text element to display the FPS

    private int frameCount = 0;
    private float elapsedTime = 0f;
    private float updateInterval = 1f; // Update interval in seconds

    void Update()
    {
        frameCount++;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= updateInterval)
        {
            float fps = frameCount / elapsedTime;
            fpsText.text = "FPS: " + fps.ToString("F2");

            // Reset for the next interval
            frameCount = 0;
            elapsedTime = 0f;
        }
    }
}
