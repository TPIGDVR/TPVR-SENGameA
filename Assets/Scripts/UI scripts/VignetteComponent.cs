using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class VignetteComponent : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;
  

    private Vignette _vignette;
  

    void Start()
    {
        
        postProcessingVolume.profile.TryGet(out _vignette);
        _vignette.intensity.value = 0;

    }

    void Update()
    {
  
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 15, 0.05f * Time.deltaTime); //old mathflerp incase

    }
}