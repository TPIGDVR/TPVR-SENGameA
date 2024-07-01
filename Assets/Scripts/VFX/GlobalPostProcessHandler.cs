using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalPostProcessHandler : MonoBehaviour
{
    float defaultBloomIntensity = 4f;
    float defaultBloomThreshold = 0.35f;
    float defaultBloomScatter = 0.35f;
    Volume globalVolume;
    [SerializeField]Bloom bloom;

    //event
    EventManager<PostProcessEvents> ppem = EventSystem.postProcess;
    private void Start()
    {
        globalVolume = GetComponent<Volume>();
        globalVolume.profile.TryGet(out bloom);

        ppem.AddListener<float>(PostProcessEvents.SUNGLASSES_ON,ReduceBloomIntensity);
        ppem.AddListener(PostProcessEvents.SUNGLASSES_OFF, ResetBloomIntensity);
    }

    void ReduceBloomIntensity(float reducePercentage)
    {
        float r = 1 - reducePercentage;

        bloom.intensity.value = defaultBloomIntensity * r;
    }

    #region RESET BLOOM SETTINGS
    void ResetBloomIntensity()
    {
        bloom.intensity.value = defaultBloomIntensity;
    }

    void ResetBloomThreshold()
    {
        bloom.threshold.value = defaultBloomThreshold;
    }

    void ResetBloomScatter()
    {
        bloom.scatter.value = defaultBloomScatter;
    }

    void ResetBloomParams()
    {
        ResetBloomIntensity();
        ResetBloomThreshold();
        ResetBloomScatter();
    }
    #endregion
}
