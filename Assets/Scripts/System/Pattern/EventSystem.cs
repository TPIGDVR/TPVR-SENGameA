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
    HEADPHONES_ON,
    HEADPHONES_OFF,

    //objective related
    OBJECTIVE_UPDATED,

    //death related
    DEATH,//tutorial death 
    DEATH_SCREEN_FADED, //tutorial event death screen faded
    RESTART,//tutorial event death screen faded
    RES_SCREEN_FADED, // tutorial res screen faded

    MOVE_PLAYER_TO_KIOKPOSITION,
    FINISH_MOVING_PLAYER,

    GRAB_DOWN_LEFT,
    GRAB_DOWN_RIGHT,
    GRAB_UP_LEFT,
    GRAB_UP_RIGHT,

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

    //hologram related
    INTERRUPT_HOLOGRAM, //stop the hologram
    HOLOGRAM_SLIDESHOW_PORTABLE_SHOW,
    HOLOGRAM_SLIDESHOW_PORTABLE_HIDE,
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
    ACTIVATE_BREATHING,
    //ACTIVATE_OBJECTIVE,
    COMPLETED_TUTORIAL_DIALOG,
    MOVE_FIRST_AUTOMATON,
}