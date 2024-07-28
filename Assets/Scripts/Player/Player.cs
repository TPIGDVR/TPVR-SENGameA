using Assets.Scripts.Player.Anxiety_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isWearingSunglasses;
    EventManager<PlayerEvents> em_p = EventSystem.player;
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;
    EventManager<DialogEvents> em_d = EventSystem.dialog;
    //AnxietyHandler anxietyHandler;
    PlayerAnxietyHandler anxietyHandler;
    NoiseProximityHandler noiseProximityHandler;

    //references to ui
    [SerializeField]
    GameObject Heart,Objective;

    //room / objective variables
    Room currentRoom;


    private void Awake()
    {
        em_t.AddListener(TutorialEvents.INIT_TUTORIAL, DeactivateAllMechanic);
        em_d.AddListener(DialogEvents.ACTIVATE_HEARTRATE, ActivateHeartRateMechanic);
        em_d.AddListener(DialogEvents.ACTIVATE_OBJECTIVE, ActivateObjectiveMechanic);
        anxietyHandler = GetComponent<PlayerAnxietyHandler>();
        noiseProximityHandler = GetComponent<NoiseProximityHandler>();

    }

    #region UNUSED FOR NOW
    void WearSunglasses()
    {
        isWearingSunglasses = true;
        em_p.TriggerEvent<float>(PlayerEvents.SUNGLASSES_ON, 1f);
    }

    void TakeOffSunglasses()
    {
        isWearingSunglasses = false;
    }
    #endregion
    //Mechanics :
    //Noise Range Indicator
    //Heartrate
    //Level objective

    void ActivateHeartRateMechanic()
    {
        anxietyHandler.CanRun = true;
        Heart.SetActive(true);
    }

    void ActivateObjectiveMechanic()
    {
        Objective.SetActive(true);
    }

    void DeactivateAllMechanic()
    {
        //heart rate: deactivate the ui + anxiety build up
        //disable noise range indicator
        //disable objective indicator
        anxietyHandler.CanRun = false;
        Heart.SetActive(false);
        Objective.SetActive(false);
    }

    void CompleteObjective()
    {

    }
}
