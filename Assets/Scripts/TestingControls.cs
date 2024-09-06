using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestingControls : MonoBehaviour
{
    public TMP_Text debugtext;

    public void A(){
        print("A");
    }

    public void B(){
        print("B");
    }
    public void X(){
        print("X");
    }

    public void Y(){
        print("Y");
    }
    
    public void LeftTrigger(){
        debugtext.text = "Left Trigger";
        print("Left Trigger");
    }

    public void RightTrigger(){
        debugtext.text = "Right Trigger";
        print("Right Trigger");
    }

    public void LeftGrip(){
        debugtext.text = "Left Grip";
        print("Left Grip");
    }

    public void RightGrip(){
        debugtext.text = "Right Grip";
        print("Right Grip");
    }
}
