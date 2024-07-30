using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerObjectiveHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text ObjectiveText;

    EventManager<PlayerEvents> em_p = EventSystem.player;

    public void InitializeObjectiveHandler()
    {
        em_p.AddListener<string>(PlayerEvents.OBJECTIVE_UPDATED, UpdateObjectiveText);
    }

    void UpdateObjectiveText(string text)
    {
        ObjectiveText.text = text;
    }

}
