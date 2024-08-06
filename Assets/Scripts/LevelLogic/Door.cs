using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public abstract class Door : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    GameObject mapIcon;
    [SerializeField]
    Transform door_L, door_R;
    [SerializeField]
    float aniSpeedMutiplier, aniTime;
    bool canBeOpened;

    Vector3 door_L_OP, door_R_OP;
    protected EventManager<LevelEvents> em_l = EventSystem.level;

    [SerializeField]
    float scanSpeed;

    [SerializeField]
    ScannerUI scanner;

    #region Initialization
    public virtual void Initialize()
    {
        door_L_OP = door_L.localPosition;
        door_R_OP = door_R.localPosition;
        scanner.SetActive(false);
        mapIcon.SetActive(false);
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL + 2);
    }
    #endregion

    public void OpenDoor()
    {
        scanner.SetActive(false);
        mapIcon.SetActive(false);
        StartCoroutine(OpenDoor_Cor());
    }

    protected void LevelCleared()
    {
        StartCoroutine(OpenDoor_Cor());
    }

    IEnumerator OpenDoor_Cor()
    {
        float timer = 0;
        float timeInterval = 0.02f;
        float time = aniTime / aniSpeedMutiplier;
        while (timer < time)
        {
            door_L.localPosition = Vector3.Lerp(door_L_OP, new(-1.75f, 0, -16f), timer / time);
            door_R.localPosition = Vector3.Lerp(door_R_OP, new(1.25f, 0, 16f), timer / time);
            timer += timeInterval;
            //play sound

            yield return new WaitForSeconds(timeInterval);
        }
    }

    public void MakeDoorOpenable()
    {
        canBeOpened = true;
        scanner.SetActive(true);
        mapIcon.SetActive(true);
        scanner.enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeOpened) return;


    }
}
