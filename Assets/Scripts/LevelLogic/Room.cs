using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableObjectManager;
public class Room : MonoBehaviour
{
    [SerializeField]
    readonly Objective[] roomObj;
    Objective[] roomObj_rt;

    AutomatonBehaviour[] automatons;
    Kiosk[] kiosks;
    bool isCompleted;

    #region ROOM INITIALIZATION
    void InitializeRoom()
    {
        RetrieveAutomatonsInRoom();
        RetrieveKiosksInRoom();
        InstantiateScriptableObject();
        isCompleted = false;
    }

    void RetrieveAutomatonsInRoom()
    {
        automatons = GetComponentsInChildren<AutomatonBehaviour>();
    }

    void RetrieveKiosksInRoom()
    {
        kiosks = GetComponentsInChildren<Kiosk>();
    }

    void InstantiateScriptableObject()
    {
        AddIntoSOCollection(roomObj); //instantiates the objects
        var temp = RetrieveRuntimeScriptableObject(roomObj); //retrieve the instantiated object
        roomObj_rt = new Objective[roomObj.Length];
        int index = 0;

        foreach (var item in temp)
        {
            roomObj_rt[index] = (Objective)item;
            index++;
        }
    }
    #endregion

    //call this when switching objective
    void DisplayRoomObjective()
    {
        //call the display event?
        List<string> objStr = new();
        foreach (var item in roomObj_rt)
        {
            objStr.Add($"\n {item.Name} : {item.Completed} / {item.NumToComplete}");
        }
    }

    void ObjectiveComplete(ObjectiveName objName)
    {
        foreach (var obj in roomObj_rt)
        {
            if (obj.Name == objName)
                obj.Completed += 1;
        }
        
    }
}
