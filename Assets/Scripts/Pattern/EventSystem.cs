using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager<Event> em = new();
    public static EventManager<PostProcessEvents> postProcess = new();

}

public enum PostProcessEvents
{
    SUNGLASSES_ON
}

public enum Event
{
    ANXIETY_UPDATE,
    ANXIETY_BREATHE
}