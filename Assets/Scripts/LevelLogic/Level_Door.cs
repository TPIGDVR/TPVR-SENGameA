using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Door : MonoBehaviour
{
    [SerializeField]
    Transform door_L, door_R;
    [SerializeField]
    float aniSpeedMutiplier, aniTime;

    Vector3 door_L_OP, door_R_OP;
    EventManager<LevelEvents> em_l = EventSystem.level;


    // Start is called before the first frame update
    void Start()
    {
        door_L_OP = door_L.localPosition;
        door_R_OP = door_R.localPosition;
        em_l.AddListener(LevelEvents.LEVEL_CLEARED, LevelCleared);
        //StartCoroutine(OpenDoor());
    }

    void LevelCleared()
    {
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        float timer = 0;
        float timeInterval = 0.02f;
        float time = aniTime / aniSpeedMutiplier;
        while(timer < time)
        {
            door_L.localPosition = Vector3.Lerp(door_L_OP,new(-1.75f,0,-16f),timer/time);
            door_R.localPosition = Vector3.Lerp(door_R_OP, new(1.25f, 0, 16f), timer / time);
            //door_L.localPosition += Vector3.forward;
            timer += timeInterval;
            yield return new WaitForSeconds(timeInterval);
        }
    }
}
