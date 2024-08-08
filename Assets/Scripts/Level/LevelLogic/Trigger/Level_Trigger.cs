using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Trigger : Trigger
{
    public enum Type
    {
        ROOM_SWITCH,
    }
    EventManager<LevelEvents> em_l = EventSystem.level;


    [SerializeField]
    Type type;

    [SerializeField]
    Room room;

    public override void ActivateTrigger()
    {
        switch (type)
        {
            case Type.ROOM_SWITCH:
                RoomSwitch();
                break;
            default:
                break;
        }
    }

    void RoomSwitch()
    {
        em_l.TriggerEvent(LevelEvents.ENTER_NEW_ROOM, room);
    }

}
