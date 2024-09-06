using System.Collections;
using System.Collections.Generic;
using Patterns;
using TMPro;
using UnityEngine;

public class SimpleDebugingScript : Singleton<SimpleDebugingScript>
{
    [SerializeField] TextMeshProUGUI text;

    public void DebugLine(string line)
    {
        text.text = line;
    }

}
