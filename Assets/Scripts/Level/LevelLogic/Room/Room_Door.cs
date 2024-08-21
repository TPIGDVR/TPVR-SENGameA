using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Door : Door
{
    [SerializeField]
    Room leadingRoom;
    [SerializeField]
    Room_Door_Tag doorTag;

    public bool CheckIfSameDoor(Room_Door_Tag tag)
    {
        return doorTag == tag;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("this");
        if (other.CompareTag("DoorChecker"))
        {
            //Debug.DrawLine(GameData.player.PlayerTransform.position, transform.position,Color.red,4f);
            //Debug.DrawRay(transform.position, (GameData.player.PlayerTransform.position - transform.position),Color.yellow,5f);
            //float val = Vector3.Dot(GameData.player.PlayerTransform.position - transform.position, transform.right);
            bool isForward = Vector3.Dot(GameData.player.PlayerTransform.position - transform.position, transform.right) < 0;
            //print($"doorchecker left {val}, {isForward}");
            if (isForward)
            {
                em_l.TriggerEvent(LevelEvents.ENTER_NEW_ROOM, leadingRoom);
            }
        }
    }
    [ContextMenu("Change new room")]
    public void TriggerEvent()
    {
        em_l.TriggerEvent(LevelEvents.ENTER_NEW_ROOM, leadingRoom);

    }
}

public enum Room_Door_Tag
{
    A,
    B,
    C
}
