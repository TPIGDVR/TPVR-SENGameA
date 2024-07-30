using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField]
    Transform LevelTransform;
    Level level;
    EventManager<LevelEvents> em_l = EventSystem.level;

    private void Awake()
    {
        //have to make sure the level's scale is uniform for all axis
        LevelConstants.UpdateScale(LevelTransform.localScale.x);
        level = GetComponent<Level>();
    }

    private void Start()
    {
        ScriptLoadSequencer.LoadScripts();
    }
}
