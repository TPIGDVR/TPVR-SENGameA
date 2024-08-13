using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    //public FadeScreen fadeScreen;
    public static SceneTransitionManager singleton;

    private void Awake()
    {
        if (singleton && singleton != this)
            Destroy(singleton);

        singleton = this;
    }

    public void GoToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        //StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    //IEnumerator GoToSceneRoutine(int sceneIndex)
    //{
    //    fadeScreen.FadeOut();
    //    yield return new WaitForSeconds(fadeScreen.fadeDuration);

    //    //Launch the new scene
    //    SceneManager.LoadScene(sceneIndex);
    //}

    //public void GoToSceneAsync(int sceneIndex)
    //{
    //    StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    //}

    //IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    //{
    //    fadeScreen.FadeOut();
    //    //Launch the new scene
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    //    operation.allowSceneActivation = false;

    //    float timerOffset = 0;
    //    while(timerOffset <= fadeScreen.fadeDuration && !operation.isDone)
    //    {
    //        timerOffset += Time.deltaTime;
    //        yield return null;
    //    }

    //    operation.allowSceneActivation = true;
    //}
}
