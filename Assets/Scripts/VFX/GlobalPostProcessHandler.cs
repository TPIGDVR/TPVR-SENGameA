using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalPostProcessHandler : MonoBehaviour
{
    float defaultBloomIntensity = 20f;
    float defaultBloomThreshold = 0.35f;
    float defaultBloomScatter = 0.35f;
    Volume globalVolume = GameData.volume;
    [SerializeField]Bloom bloom;
    private void Start()
    {
        bloom = globalVolume.GetComponent<Bloom>();
    }

}
