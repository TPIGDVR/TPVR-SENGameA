using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test : MonoBehaviour
{
    Texture2D lumTex2D;
    EventManager<Event> em = EventSystem.em;
    void TestMethod()
    {
       // Material mat = new();
    }


    private void Update()
    {
        Process();
    }

    void Process() 
    {

        RenderTexture rt = EventSystem.em.TriggerEvent<RTHandle>(Event.REQUEST_LUMTEXTURE).rt;
        lumTex2D = new(rt.width, rt.height);
        RenderTexture.active = rt;
        lumTex2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        lumTex2D.Apply();
        RenderTexture.active = null;

        Color[] lumArray = lumTex2D.GetPixels();
        float totalBrightness = 0;
        for (int i = 0; i < lumArray.Length; i++)
        {
            float brightness = lumArray[i].grayscale;
            totalBrightness += brightness;
        }
        totalBrightness /= lumArray.Length;
        rt.Release();
        Debug.Log(totalBrightness);
        em.TriggerEvent<float>(Event.GLARE_UPDATE, totalBrightness);
    }
}
