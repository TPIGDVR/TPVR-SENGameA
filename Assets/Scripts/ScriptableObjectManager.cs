using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptableObjectManager //make singleton
{
    static Dictionary<ScriptableObject, ScriptableObject> scriptableObjectCollection = new();
    
    /// <summary>
    /// Creates a instance of the original scriptable object for editing in runtime without affecting the original.
    /// </summary>
    /// <param name="original"></param>
    public static void AddIntoSOCollection(ScriptableObject original)
    {
        Debug.Log($"adding {original.name}...");
        scriptableObjectCollection.Add(original, ScriptableObject.Instantiate(original));
    }
    
    public static void AddIntoSOCollection(ScriptableObject[] originals)
    {
        foreach (var item in originals)
        {
            AddIntoSOCollection(item);
        }
    }

    public static void RemoveFromSOCollection(ScriptableObject original)
    {
        if (scriptableObjectCollection[original] == null) throw new System.Exception($"Scriptable Object Manager : Item does not exist in dictionary!");
        scriptableObjectCollection.Remove(original);
    }

    public static void RemoveFromSOCollection(ScriptableObject[] originals)
    {
        foreach (var item in originals)
        {
            RemoveFromSOCollection(item);
        }
    }

    /// <summary>
    /// Passes in the original SO to retrieve the runtime scriptable object instance.
    /// Helps prevent the editing of the original data in the original scriptable object.
    /// </summary>
    /// <param name="original">The original scriptable object</param>
    /// <returns>Returns the instantiated runtime scriptable object</returns>
    public static ScriptableObject RetrieveRuntimeScriptableObject(ScriptableObject original)
    {
        if (scriptableObjectCollection[original] == null) throw new System.Exception($"Scriptable Object Manager : Item does not exist in dictionary!");
        return scriptableObjectCollection[original];
    } 

    public static ScriptableObject[] RetrieveRuntimeScriptableObject(ScriptableObject[] originals)
    {
        ScriptableObject[] objs = new ScriptableObject[originals.Length];
        int index = 0;

        foreach (var item in originals)
        {
            objs[index] = RetrieveRuntimeScriptableObject(item);
            index++;
        }

        return objs;
    }
}
