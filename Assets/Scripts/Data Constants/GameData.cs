using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class GameData
{
    public static bool IsInTutorial;
    public static Player player;
   
    public static Transform playerTransform { get => player.PlayerTransform; }

    public static Hologram_Portable playerHologram;

    public static void ChangeTutorialStatus(bool b)
    {
        IsInTutorial = b;
    }

    public static void UpdatePlayer(Player t)
    {
        player = t;
    }

    
    public static GameObject hologramSlideShow => Resources.Load<GameObject>("Prefabs/Hologram/Hologram slideshow/Slide show hologram");
    public static GameObject hologram3D => Resources.Load<GameObject>("Prefabs/Hologram/Hologram 3D/3D hologram");




}
