using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_WinCondition : MonoBehaviour
{
    int numKioskCleared = 0;

    [SerializeField]
    int numKioskNeededToClear;
    EventManager<LevelEvents> em_l = EventSystem.level;

    void Start()
    {
        em_l.AddListener(LevelEvents.KIOSK_CLEARED,IncrementNumOfKioskCleared);
    }

    void Update()
    {
        if (CheckWinConditions())
        {
            //unlock door at end to win?
        }   
    }

    void IncrementNumOfKioskCleared()
    {
        numKioskCleared++;
    }

    bool CheckWinConditions()
    {
        return numKioskCleared >= numKioskNeededToClear;
    }
}
