using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Door : Door
{
    [SerializeField]
    Room leadingRoom;
    [SerializeField]
    Room_Door_Tag doorTag;

    public bool CheckIfSameDoor(Room_Door_Tag tag)
    {
        return doorTag == tag;
    }
}

public enum Room_Door_Tag
{
    A,
    B,
    C
}
