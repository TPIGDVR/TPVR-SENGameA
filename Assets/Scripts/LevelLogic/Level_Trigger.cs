using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Triggers : MonoBehaviour
{
    public enum EventType //add more if we have more in EventSystem
    {
        GAME,
        PLAYER,
        LEVEL,
        DIALOGUE,
        TUTORIAL,
    }

    [SerializeField]
    EventType Type;
    
    void DetermineEventType()
    {

    }
}
