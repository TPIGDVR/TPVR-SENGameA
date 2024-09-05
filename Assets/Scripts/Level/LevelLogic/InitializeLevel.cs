using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField]
    PlayerVFX playerVFX;
 
    [SerializeField]
    Transform LevelTransform;
    EventManager<LevelEvents> em_l = EventSystem.level;

    private void Awake()
    {
        //have to make sure the level's scale is uniform for all axis
        LevelConstants.UpdateScale(LevelTransform.localScale.x);

        //Make the black screen.
        playerVFX.DisplayFadeScreen();
    }

    private void Start()
    {
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        ScriptLoadSequencer.LoadScripts();
        yield return new WaitForSeconds(2f);
        playerVFX.BeginUnfadeScreen();
    }
}
