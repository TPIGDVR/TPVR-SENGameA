using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableObjectManager;
public class Room : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Objective[] roomObj;
    [SerializeField]
    protected Objective[] roomObj_rt;

    [SerializeField]
    AutomatonBehaviour[] automatons;
    [SerializeField]
    protected Kiosk[] kiosks;
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
        InitRoom();
        isCompleted = false;
    }

    #region retrieving room
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
    #endregion

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.AUTOMATONS + 1);
    }
    #endregion

    //call this when switching objective
    protected void DisplayRoomObjective()
    {
        //call the display event?
        string objectivename = "";
        foreach (var item in roomObj_rt)
        {
            //objStr.Add($"\n {item.Name} : {item.Completed} / {item.NumToComplete}");
            objectivename += $"{item.Name} : {item.Completed} / {item.NumToComplete}\n";
        }
        EventSystem.player.TriggerEvent(PlayerEvents.OBJECTIVE_UPDATED, objectivename);
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
                EvaluateObjective();
                //show the objective
                DisplayRoomObjective();
                if (obj.IsComplete)
                {
                    //go through each door to check\
                    print("Room is Completed");
                    foreach(var door in doors)
                    {
                        if(door.CheckIfSameDoor(obj.doorToOpen))
                        {
                            print($"made {door.name}");
                            door.MakeDoorOpenable();
                        }
                    }
                }
            }
        }
    }

    protected virtual void EvaluateObjective()
    {
        //NOOP
    }

    protected virtual void InitRoom()
    {
        //NOOP
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
