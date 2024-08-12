using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager<PlayerEvents> player = new();
    public static EventManager<GameEvents> game = new();
    public static EventManager<LevelEvents> level = new();
    public static EventManager<DialogEvents> dialog = new();
    //public static EventManager<TutorialEvents> tutorial = new();
}

public enum GameEvents
{
    NONE,
    LOSE,
    WIN,
    ENTER_NEW_SCENE,
}

public enum PlayerEvents
{
    ANXIETY_BREATHE,
    REQUEST_LUMTEXTURE,
    HEART_BEAT,
    SUNGLASSES_ON,
    SUNGLASSES_OFF,

    //objective related
    OBJECTIVE_UPDATED,

    //death related
    DEATH,//tutorial death
    DEATH_SCREEN_FADED, //tutorial event death screen faded
    RESTART,//tutorial event death screen faded
    RES_SCREEN_FADED, // tutorial res screen faded
}

public enum LevelEvents
{
    KIOSK_CLEARED,
    LEVEL_CLEARED,
    ENTER_DIALOGUE_TRIGGER,

    //objective system
    ENTER_NEW_ROOM,
    OBJECTIVE_PROGRESSED,
    OBJECTIVE_COMPLETE,

    //tutorial related
    INIT_TUTORIAL,
    FIRST_KIOSK,
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
    ACTIVATE_OBJECTIVE,
    MOVE_FIRST_AUTOMATON,
    HIGHLIGHT_ABUTTON_STOP_HIGHLIGHTING_THUMBSTICKS,
    STOP_HIGHLIGHTING_ABUTTON_HIGHLIGHT_THUMBSTICKS
}