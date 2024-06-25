using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager em = new();
    public static EventManager postProcess = new();

}

public enum PostProcessEvents
{

}

public enum Event
{
    ANXIETY_UPDATE,
    ANXIETY_BREATHE
}