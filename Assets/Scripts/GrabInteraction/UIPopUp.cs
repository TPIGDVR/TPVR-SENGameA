using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [SerializeField] float rangeDistance = 2f;
    [SerializeField] GameObject instructionToolTip;
    
    Vector3 leftHandPos => GameData.player.LeftHand.position;
    Vector3 rightHandPos => GameData.player.RightHand.position;

    bool isHandsNearby { get
        {
            bool isLefthandNear = Vector3.Distance(leftHandPos, transform.position) < rangeDistance;
            bool isRighthandNear = Vector3.Distance(rightHandPos, transform.position) < rangeDistance;
            //print($"current transform {transform.position}");
            //print($"check left hand nearer {isLefthandNear}. " +
            //    $"distance {Vector3.Distance(leftHandPos, transform.position)} " +
            //    $"position {leftHandPos}" +
            //    $"");
            //print($"check right hand nearer {isRighthandNear}. distance {Vector3.Distance(rightHandPos, transform.position)} " +
            //    $"position {rightHandPos}");

            
            return isLefthandNear || isRighthandNear;
        } }
    bool isDisplayed = false;

    private void Update()
    {
        bool ishandNear_l = isHandsNearby;
        //print($"is hand near {ishandNear_l}");
        if (ishandNear_l && !isDisplayed)
        {
            isDisplayed = true;
            instructionToolTip.SetActive(true);
        }
        else if(!ishandNear_l && isDisplayed)
        {
            isDisplayed = false;
            instructionToolTip.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeDistance);
    }

}
