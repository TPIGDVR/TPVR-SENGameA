using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InitializeGame : MonoBehaviour
{
    [SerializeField] Volume vol;

    private void Awake()
    {
        GameData.volume = vol;
    }
}
