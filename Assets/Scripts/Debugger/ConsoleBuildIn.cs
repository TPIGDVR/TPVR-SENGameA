using System.Collections;
using System.Collections.Generic;
using System.Text;
using Patterns;
using TMPro;
using UnityEngine;

public class ConsoleBuildIn : Singleton<ConsoleBuildIn>
{
    [SerializeField] GameObject consoleGameObject;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] OVRInput.Button triggerInputOne, triggerInputTwo;



    private void Update()
    {
        if (OVRInput.GetDown(triggerInputOne) && OVRInput.GetDown(triggerInputTwo))
        {
            consoleGameObject.SetActive(!consoleGameObject.activeSelf);
        }
        else if (OVRInput.GetDown(triggerInputOne) && consoleGameObject.activeSelf)
        {
            Clear();
        }
    }

    IEnumerator test()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            Log("Testing");
        }
    }

    public void Log(string line)
    {
        StringBuilder stringBuilder = new StringBuilder(text.text);
        stringBuilder.Append("- " + line + "\n");
        text.text = stringBuilder.ToString();
    }


    void Clear()
    {
        text.text = "";
    }
}
