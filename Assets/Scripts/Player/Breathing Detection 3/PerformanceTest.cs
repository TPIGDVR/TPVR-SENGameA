using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerformanceTest
{
    public static IEnumerator GetFPS(float seconds)
    {
        int frames = 0;
        float time = 0;
        while (time < seconds)
        {
            frames++;
            time += Time.deltaTime;
            yield return null;
        }
        float fps = frames / seconds;
        Debug.Log("Average FPS: " + fps);
    }
}
