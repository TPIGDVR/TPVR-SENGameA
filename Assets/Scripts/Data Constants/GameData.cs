using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class GameData
{
    public static bool IsInTutorial;

    public static void ChangeTutorialStatus(bool b)
    {
        IsInTutorial = b;
    }
}
