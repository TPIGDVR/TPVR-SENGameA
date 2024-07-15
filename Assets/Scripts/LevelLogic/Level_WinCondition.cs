using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_WinCondition : MonoBehaviour
{
    public int numKioskCleared = 0;

    [SerializeField]
    int numKioskNeededToClear;
    EventManager<LevelEvents> em_l = EventSystem.level;

    void Start()
    {
        em_l.AddListener(LevelEvents.KIOSK_CLEARED,IncrementNumOfKioskCleared);
    }

    void Update()
    {
           
    }

    void IncrementNumOfKioskCleared()
    {
        numKioskCleared++;

        if (CheckWinConditions())
        {
            em_l.TriggerEvent(LevelEvents.LEVEL_CLEARED);
        }
    }

    bool CheckWinConditions()
    {
        return numKioskCleared >= numKioskNeededToClear;
    }
}
