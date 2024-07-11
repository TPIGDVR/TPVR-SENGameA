using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager<PlayerEvents> player = new();
    public static EventManager<GameEvents> game = new();

}

public enum GameEvents
{
    LOSE,
    WIN
}

public enum PlayerEvents
{
    ANXIETY_UPDATE,
    ANXIETY_BREATHE,
    START_GAME,
    REQUEST_LUMTEXTURE,
    GLARE_UPDATE,
    HEART_BEAT,
    SUNGLASSES_ON,
    SUNGLASSES_OFF,
}