using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMicPlayer : MonoBehaviour
{
    [SerializeField]AudioSource _audiosource;

    private void Start()
    {
        foreach (string name in Microphone.devices)
        {
            print(name);
        }
        var clip = Microphone.Start(null, true, 10, (int)Caress.SampleRate._48000);
        _audiosource.clip = clip;
        _audiosource.loop = true;
        while(!(Microphone.GetPosition(null) > 0))
        _audiosource.Play();
    }
}
