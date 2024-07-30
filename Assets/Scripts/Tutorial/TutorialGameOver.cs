using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGameOver : MonoBehaviour
{
    public Transform deathPoint;
    EventManager<TutorialEvents> em_t = EventSystem.tutorial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            em_t.TriggerEvent(TutorialEvents.RESTART); 
        }
    }
}
