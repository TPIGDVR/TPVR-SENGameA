using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.game.TriggerEvent(GameEvents.ENTER_NEW_SCENE);
    }
}
