using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "Level Objective")]
public class Objective : ScriptableObject
{
    public ObjectiveName Name;
    public int Completed;
    public int NumToComplete;
    public Room_Door_Tag doorToOpen;

    public bool IsComplete => Completed == NumToComplete;
}

public enum ObjectiveName
{
    KIOSK
}