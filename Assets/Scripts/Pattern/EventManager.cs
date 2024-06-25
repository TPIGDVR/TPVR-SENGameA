using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager
{
    //private static EventManager instance;

    //public static EventManager Instance // making the eventmanager singleton
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = new EventManager();
    //        }
    //        return instance;
    //    }
    //}

    private Dictionary<int, List<Delegate>> eventListeners;

    public EventManager() //constructor to initalize eventListeners dictionary
    {
        eventListeners = new();
    }

    /* Method for add, remove and trigger events
        * Tried to use method overloading to make all of them use the same method but with
        * different returns and parameters
        */

    //uses the Action delegate which does not return a value
    #region ADD/REMOVE/TRIGGER (NO PARAMETERS)
    public void AddListeners(int id, params Action[] listeners)
    {
        // If the event does not exist in the dictionary, add it.
        if (!eventListeners.ContainsKey(id))
        {
            eventListeners[id] = new List<Delegate>();
        }

        // Add the listener to the event's list of delegates.
        foreach (var listener in listeners)
        {
            eventListeners[id].Add(listener);
        }
    }

    public void RemoveListener(int id, params Action[] listeners)
    {

        // If the event exists, remove the listener from its list of delegates.
        foreach (var listener in listeners)
        {
            if (eventListeners.ContainsKey(id))
            {
                eventListeners[id].Remove(listener);
            }
        }
    }
    public void TriggerEvent(int id)
    {
        // If the event exists, invoke all listeners associated with it.
        if (eventListeners.ContainsKey(id))
        {
            var listeners = eventListeners[id].ToArray();
            foreach (var listener in listeners)
            {
                if (listener is Action action)
                {
                    action.Invoke();
                }
            }
        }
        else
        {
            Debug.LogError("Event not in list");
        }

    }
    #endregion

    //similar to the above but allows passing of parameters
    #region ADD/REMOVE/TRIGGER (W/ PARAM)
    //takes in 1 parameter
    public void AddListener<TParam>(int id, params Action<TParam>[] listeners)
    {
        if (!eventListeners.ContainsKey(id))
        {
            eventListeners[id] = new List<Delegate>();
        }

        foreach (var listener in listeners)
        {
            eventListeners[id].Add(listener);
        }
    }

    public void RemoveListener<TParam>(int id, params Action<TParam>[] listeners)
    {
        foreach (var listener in listeners)
        {
            if (eventListeners.ContainsKey(id))
            {
                eventListeners[id].Remove(listener);
            }
        }
    }

    public void TriggerEvent<TParam>(int id, TParam param)
    {
        if (eventListeners.ContainsKey(id))
        {
            var listeners = eventListeners[id].ToArray();
            foreach (var listener in listeners)
            {
                if (listener is Action<TParam> action)
                {
                    action.Invoke(param);
                }
            }
        }
    }

    //takes in 2 parameters
    public void AddListener<TParam1, TParam2>(int id, params Action<TParam1, TParam2>[] listeners)
    {
        if (!eventListeners.ContainsKey(id))
        {
            eventListeners[id] = new List<Delegate>();
        }

        foreach (var listener in listeners)
        {
            eventListeners[id].Add(listener);
        }
    }

    public void RemoveListener<TParam1, TParam2>(int id, params Action<TParam1, TParam2>[] listeners)
    {
        foreach (var listener in listeners)
        {
            if (eventListeners.ContainsKey(id))
            {
                eventListeners[id].Remove(listener);
            }
        }
    }

    public void TriggerEvent<TParam1, TParam2>(int id, TParam1 param1, TParam2 param2)
    {
        if (eventListeners.ContainsKey(id))
        {
            foreach (var listener in eventListeners[id])
            {
                if (listener is Action<TParam1, TParam2> action)
                {
                    action.Invoke(param1, param2);
                }
            }
        }
    }
    #endregion

    //uses the Func delegate which returns a value
    #region ADD/REMOVE/TRIGGER (W/ RETURN)
    public void AddListener<TResult>(int id, params Func<TResult>[] listeners)
    {
        if (!eventListeners.ContainsKey(id))
        {
            eventListeners[id] = new List<Delegate>();
        }

        foreach (var listener in listeners)
        {
            eventListeners[id].Add(listener);
        }
    }

    public void RemoveListener<TResult>(int id, params Func<TResult>[] listeners)
    {
        foreach (var listener in listeners)
        {
            if (eventListeners.ContainsKey(id))
            {
                eventListeners[id].Remove(listener);
            }
        }
    }

    public TResult TriggerEvent<TResult>(int id)
    {
        if (eventListeners.ContainsKey(id))
        {
            var listeners = eventListeners[id].ToArray();

            foreach (var listener in listeners)
            {
                if (listener is Func<TResult> function)
                {
                    return function.Invoke();
                }
            }
        }

        Debug.Log("returning default...");
        return default(TResult);
    }
    #endregion

    //similar to the above but allows passing of parameters w/ return of a value
    #region ADD/REMOVE/TRIGGER (W/ RETURN + PARAM)
    public void AddListener<TParam, TResult>(int id, params Func<TParam, TResult>[] listeners)
    {
        if (!eventListeners.ContainsKey(id))
        {
            eventListeners[id] = new List<Delegate>();
        }

        foreach (var listener in listeners)
        {
            eventListeners[id].Add(listener);
        }
    }

    public void RemoveListener<TParam, TResult>(int id, params Func<TParam, TResult>[] listeners)
    {
        foreach (var listener in listeners)
        {
            if (eventListeners.ContainsKey(id))
            {
                eventListeners[id].Remove(listener);
            }
        }
    }

    public TResult TriggerEvent<TParam, TResult>(int id, TParam param)
    {
        if (eventListeners.ContainsKey(id))
        {
            var listeners = eventListeners[id].ToArray();

            foreach (var listener in listeners)
            {
                if (listener is Func<TParam, TResult> function)
                {
                    return function.Invoke(param);
                }
            }
        }

        Debug.Log("returning default...");
        return default(TResult);
    }
    #endregion
}
