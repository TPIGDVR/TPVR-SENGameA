using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TapInteractorCheck : MonoBehaviour
{
    public TapManager tapManager; // Reference to the TapManager script.
    public ActivateUIOnTap activateUIOnTap;
    private bool rightTap, leftTap;
    public ProximityHapticFeedback proximityHapticFeedback;
    public XRPokeInteractor xRPokeInteractor;

    public InputActionProperty leftPointing;
    public InputActionProperty rightPointing;
    public InputActionProperty leftGrip;
    public InputActionProperty rightGrip;

    private float leftTriggerPressed, rightTriggerPressed;
    private float leftGripPressed, rightGripPressed;
    public Material handMat;
    public Color handColor;



    public float rhythmThreshold = 3f; // Adjust as needed for your rhythm requirement.

    private void Start()
    {
        handMat.color = handColor;
    }

    void Update()
    { 
        if(tapManager.tapCounter > 0)
        { 
            tapManager.tapTimer += Time.deltaTime;
        }

        if (tapManager.tapCounter == 8)
        {
            proximityHapticFeedback.lensDistortion.SetActive(false);
            proximityHapticFeedback.lensFlare.SetActive(false);
            proximityHapticFeedback.timeWithinMaxDistance *= 0.6f;
            tapManager.tapTimer = 0;
        }

        leftTriggerPressed = leftPointing.action.ReadValue<float>();
        rightTriggerPressed = rightPointing.action.ReadValue<float>();
        leftGripPressed = leftGrip.action.ReadValue<float>();
        rightGripPressed = rightGrip.action.ReadValue<float>();
    }

    public void RightTap()
    {
        if (leftTriggerPressed == 1 && leftGripPressed == 0 && rightTap == false)
        {
            tapManager.tapCounter += 1;

            if (tapManager.tapCounter <= 1) //first tap initialized
            {
                handMat.color = Color.green;
            }
            else if (tapManager.tapTimer >= 1 && tapManager.tapTimer <= 2) //tap whitin 1 - 2 seconds, turn green
            {
                handMat.color = Color.green;
            }
            else if (tapManager.tapTimer <= 0.7f && tapManager.tapTimer > 0.31 || tapManager.tapTimer >= 2.3) //0.31 - 0.7 or //2.3 - 3, turn yellow
            {
                handMat.color = Color.yellow;
            }
            else if (tapManager.tapTimer <= 0.3) // 0 - 0.3
            {
                handMat.color = Color.red;
                activateUIOnTap.ResetTaps();
            }

            xRPokeInteractor.pokeHoverRadius += 0.02f;
            rightTap = true;
            tapManager.tapTimer = 0f; // Reset the timer after a successful tap
            Debug.Log("Right Tap Detected");
        }
    }

    public void ExitRightTap()
    {
        if (rightTap)
        {
            rightTap = false;
            xRPokeInteractor.pokeHoverRadius -= 0.02f;
            handMat.color = handColor;
            Debug.Log("Exit Right Tap");
        }
    }

    public void LeftTap()
    {
        if (rightTriggerPressed == 1 && rightGripPressed == 0 && leftTap == false)
        {
            tapManager.tapCounter += 1;

            if (tapManager.tapCounter <= 1) //first tap initialized
            {
                handMat.color = Color.green;
            }
            else if (tapManager.tapTimer >= 1 && tapManager.tapTimer <= 2) //tap whitin 1 - 2 seconds, turn green
            {
                handMat.color = Color.green;
            }
            else if (tapManager.tapTimer <= 0.7f && tapManager.tapTimer > 0.31 || tapManager.tapTimer >= 2.3) //0.31 - 0.7 or //2.3 - 3, turn yellow
            {
                handMat.color = Color.yellow;
            }
            else if(tapManager.tapTimer <= 0.3) // 0 - 0.3
            {
                handMat.color = Color.red;
                activateUIOnTap.ResetTaps();
            }

            xRPokeInteractor.pokeHoverRadius += 0.02f;
            leftTap = true;
            tapManager.tapTimer = 0f; // Reset the timer after a successful tap

            Debug.Log("Left Tap Detected");
        }
    }

    public void ExitLeftTap()
    {
        if (leftTap)
        {
            leftTap = false;
            xRPokeInteractor.pokeHoverRadius -= 0.02f;
            handMat.color = handColor;
            Debug.Log("Exit Left Tap");
        }
    }

    // Add this function to check if the rhythm is maintained.
    public bool IsRhythmMaintained()
    {
        return tapManager.tapTimer <= rhythmThreshold;
    }
}

