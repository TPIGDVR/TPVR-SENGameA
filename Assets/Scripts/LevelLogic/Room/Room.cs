using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableObjectManager;

public class Room : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Objective[] roomObj;
    [SerializeField]
    Objective[] roomObj_rt;


    [SerializeField]
    AutomatonBehaviour[] automatons;
    [SerializeField]
    Kiosk[] kiosks;
    [SerializeField]
    Room_Door[] doors;

    bool isCompleted;

    #region ROOM INITIALIZATION
    public void Initialize()
    {
        RetrieveAutomatonsInRoom();
        RetrieveKiosksInRoom();
        RetrieveDoorsInRoom();
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

    void RetrieveDoorsInRoom()
    {
        doors = GetComponentsInChildren<Room_Door>();
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

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL + 1);
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

    public void ProgressObjective(ObjectiveName objName)
    {
        foreach (var obj in roomObj_rt)
        {
            //check if completed
            if (obj.Name == objName && !obj.IsComplete) 
            {
                obj.Completed += 1;

                //check if the objective is completed
                if (obj.IsComplete)
                {
                    //go through each door to check
                    foreach(var door in doors)
                    {
                        if(door.CheckIfSameDoor(obj.doorToOpen))
                        {
                            door.MakeDoorOpenable();
                        }
                    }
                }
            }
        }
    }

    bool isObjectiveComplete()
    {
        bool completed = false;
        foreach (var obj in roomObj_rt)
        {
            completed &= obj.IsComplete;
        }

        return completed;
    }

}
