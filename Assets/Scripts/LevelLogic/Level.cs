using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    Room[] rooms;
    Level_Door door;

    public void InitializeLevel()
    {
        //initializing room
        rooms = GetComponentsInChildren<Room>();
        foreach (Room room in rooms) 
        {
            room.InitializeRoom();
        }

        //get level door
        door = GetComponentInChildren<Level_Door>();
        door.InitializeDoor();
    }
}
