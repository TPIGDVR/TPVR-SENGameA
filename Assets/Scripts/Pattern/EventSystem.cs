using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager<PlayerEvents> player = new();
    public static EventManager<GameEvents> game = new();
    public static EventManager<LevelEvents> level = new();
    public static EventManager<DialogEvents> dialog = new();
    public static EventManager<TutorialEvents> tutorial = new();
}

public enum GameEvents
{
    NONE,
    LOSE,
    WIN,
    ENTER_NEW_SCENE
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

public enum TutorialEvents
{
    NONE,
    INIT_TUTORIAL,
    ACTIVATE_KIOSK,
    FIRST_KIOSK,
}

public enum LevelEvents
{
    KIOSK_CLEARED,
    LEVEL_CLEARED,
    ENTER_DIALOGUE_TRIGGER 
}

public enum DialogEvents
{
    NONE,
    ADD_DIALOG,
    START_DIALOG,
    END_DIALOG,
    NEXT_LINE,

    //tutorial dialogue events
    ACTIVATE_NOISE_INDICATOR,
    ACTIVATE_HEARTRATE,
    ACTIVATE_OBJECTIVE
}