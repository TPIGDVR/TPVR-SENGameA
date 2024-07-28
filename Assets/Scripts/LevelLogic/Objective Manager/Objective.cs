using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective",menuName = "Level Objective")]
public class Objective : ScriptableObject
{
    public ObjectiveName Name;
    public int Completed;
    public int NumToComplete;
}

public enum ObjectiveName
{
    KIOSK
}