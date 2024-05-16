using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActivateUIOnTap : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TapInteractorCheck TapInteractorCheck;
    public GameObject[] segments; // Reference to your UI images.
    private int tapCounter = 0;

    private void Start()
    {
        DeactivateAllSegments();
    }

    void Update()
    {
        timeText.text = "Time: " + TapInteractorCheck.tapManager.tapTimer.ToString("F2");

        // Check if the tap counter has increased.
        if (tapCounter < TapInteractorCheck.tapManager.tapCounter)
        {
            // Check if the maximum number of taps has been reached.
            if (tapCounter >= segments.Length)
            {
                // Deactivate all images before resetting tapCounter.
                DeactivateAllSegments();
                tapCounter = 0;
                TapInteractorCheck.tapManager.tapCounter = 0;
            }

            // Activate the next image from left to right.
            if (tapCounter < segments.Length)
            {
                segments[tapCounter].SetActive(true);
                tapCounter++;
            }
        }

        // Check if the tap counter matches the number of segments, and deactivate them in this case.
        if (tapCounter >= segments.Length)
        {
            DeactivateAllSegments();
            tapCounter = 0;
            TapInteractorCheck.tapManager.tapCounter = 0;
        }

        // If a tap occurs, but it doesn't maintain the rhythm, reset the tapCounter and deactivate all images.
        if (TapInteractorCheck.tapManager.tapCounter > 0 && !IsRhythmMaintained())
        {
            ResetTaps();
            StartCoroutine(ChangeColorCoroutine());
        }
    }

    private void DeactivateAllSegments()
    {
        foreach (var segment in segments)
        {
            segment.SetActive(false);
        }
    }

    private bool IsRhythmMaintained()
    {
        // Add your rhythm logic here. For example, you can check the time between the last two taps.
        // Return true if the rhythm is maintained; otherwise, return false.
        return TapInteractorCheck.IsRhythmMaintained();
    }

    public void ResetTaps()
    {
        tapCounter = 0;
        TapInteractorCheck.tapManager.tapCounter = 0;
        TapInteractorCheck.tapManager.tapTimer = 0;
        DeactivateAllSegments();
    }

    IEnumerator ChangeColorCoroutine()
    {
        TapInteractorCheck.handMat.color = Color.red;
        // Wait for a brief moment (adjust the faintDuration as needed)
        yield return new WaitForSeconds(0.1f);
        TapInteractorCheck.handMat.color = TapInteractorCheck.handColor;
    }
}