using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class GameData
{
    public static bool IsInTutorial;
    public static Player player;
    public static Transform playerTransform { get => player.PlayerTransform; }
    public static void ChangeTutorialStatus(bool b)
    {
        IsInTutorial = b;
    }

    public static void UpdatePlayer(Player t)
    {
        player = t;
    }
}
