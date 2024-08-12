using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
public abstract class Door : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    GameObject interactionIcon;
    [SerializeField]
    GameObject doorIcon;
    [SerializeField]
    Transform door_L, door_R;
    [SerializeField]
    float aniSpeedMultiplier, aniTime;
    bool canBeOpened;

    Vector3 door_L_OP, door_R_OP;
    protected EventManager<LevelEvents> em_l = EventSystem.level;

    [SerializeField]
    float scanSpeed;

    [SerializeField]
    ScannerUI scanner;

    [SerializeField]
    Image pingCircle;
    [SerializeField]
    float pingSize = 30f;

    #region Initialization
    public virtual void Initialize()
    {
        door_L_OP = door_L.localPosition;
        door_R_OP = door_R.localPosition;
        scanner.SetActive(false);
        interactionIcon.SetActive(false);
        doorIcon.SetActive(true);
        pingCircle.transform.localScale = Vector3.zero;
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL + 2);
    }
    #endregion

    public void OpenDoor()
    {
        scanner.SetActive(false);
        interactionIcon.SetActive(false);
        doorIcon.SetActive(false);
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
        float time = aniTime / aniSpeedMultiplier;
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
        interactionIcon.SetActive(true);
        doorIcon.SetActive(false);
        scanner.enabled = true;
        StartCoroutine(PingDoor());
    }

    IEnumerator PingDoor()
    {
        int timesToPing = 3;
        for (int i = 0; i < timesToPing; i++)
        {
            float timer = 0;
            float timeInterval = 0.02f;
            float time = 1;
            while (timer < time)
            {
                pingCircle.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * pingSize, timer / time);
                //pingCircle.transform.localRotation = Quaternion.Euler(0, 0, 360 * timer / time);
                timer += timeInterval;
                yield return new WaitForSeconds(timeInterval);
            }
            pingCircle.transform.localScale = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeOpened) return;


    }
}
