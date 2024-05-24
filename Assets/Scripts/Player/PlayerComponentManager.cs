using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.Rendering.DebugUI;

public class PlayerComponentManager : MonoBehaviour
{
    //this script will basically control what component the player can use at certain events
    [Header("movement")]
    [SerializeField] ContinuousMoveProviderBase movement;
    [SerializeField] ContinuousTurnProviderBase cameraMovement;

    [Header("left Controller")]
    [SerializeField] XRRayInteractor leftRayInteractor;
    [SerializeField] XRDirectInteractor leftDirectInteraction;

    [Header("right Controller")]
    [SerializeField] XRRayInteractor rightRayInteractor;
    [SerializeField] XRDirectInteractor rightDirectInteraction;

    //addition things 
    bool hasGameStarted = false;

    private void OnEnable()
    {
        OnBeginGame();
        EventManager.Instance.AddListener(Event.START_GAME, OnStartGame);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener(Event.START_GAME, OnStartGame);
    }

    void OnBeginGame()
    {
        movement.enabled = false;
        cameraMovement.enabled = false;
        leftRayInteractor.enabled = true;
        rightRayInteractor.enabled = true;
    }

    void OnStartGame()
    {
        movement.enabled = true;
        cameraMovement.enabled = true;
        leftRayInteractor.enabled = false;
        rightRayInteractor.enabled = false;
        hasGameStarted = true;
    }

    public void ActivateLeftRayInteractor()
    {
        //ignore this command
        if (!hasGameStarted) return;
        leftRayInteractor.enabled = true;
        leftDirectInteraction.enabled = false;
    }

    public void DeactivateLeftRayInteractor()
    {
        //ignore this command
        if (!hasGameStarted) return;
        leftRayInteractor.enabled = false;
        leftDirectInteraction.enabled = true;
    }

    public void ActivateRightRayInteractor()
    {
        rightRayInteractor.enabled = true;
        rightDirectInteraction.enabled = false;
    }

    public void DeactivateRightRayInteractor()
    {
        rightRayInteractor.enabled = false;
        rightDirectInteraction.enabled = true;
    }
}
