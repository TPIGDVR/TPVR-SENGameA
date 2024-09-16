using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    //[SerializeField] float rangeDistance = 2f;
    [SerializeField] GameObject instructionToolTip;
    [SerializeField] Interactable connectedInterable;


    bool isDisplayed = false;

    Vector3 leftHandPos => GameData.player.LeftHand.position;
    Vector3 rightHandPos => GameData.player.RightHand.position;
    float rangeDistance = 0.2f;
    bool isHandsHoverring
    {
        get
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
        }
    }

    private void Update()
    {
        bool isHandHoverring_l = isHandsHoverring;


        print($"is hoverring {isHandHoverring_l} is grabbing {connectedInterable.isGrab} is displayed {isDisplayed}");
        if (isHandHoverring_l || connectedInterable.isGrab && !isDisplayed)
        {
            instructionToolTip.SetActive(true);
            isDisplayed = true;
        }
        else if ((!isHandHoverring_l || !connectedInterable.isGrab) && isDisplayed)
        {
            instructionToolTip.SetActive(false);
            isDisplayed = false;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeDistance);
    }

}
