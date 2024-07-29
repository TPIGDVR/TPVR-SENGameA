using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] int levelToReturn;
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.game.TriggerEvent(GameEvents.ENTER_NEW_SCENE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelToReturn != null)
            {
                SceneManager.LoadScene(levelToReturn);
            }
            else
            {
                Debug.Log("No Level to Return to!");
            }
        }
    }
}
