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
    Vignette vig;
    LiftGammaGain lgg;
    //event
    EventManager<PlayerEvents> em_p = EventSystem.player;
    private void Start()
    {
        globalVolume = GetComponent<Volume>();
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out vig);
        globalVolume.profile.TryGet(out lgg);
        em_p.AddListener<float>(PlayerEvents.SUNGLASSES_ON,ReduceBloomIntensity);
        em_p.AddListener(PlayerEvents.SUNGLASSES_OFF, ResetBloomIntensity);
    }

    void ReduceBloomIntensity(float reducePercentage)
    {
        print("reducing bloom");
        float r = 1 - reducePercentage;

        bloom.intensity.value = defaultBloomIntensity * r;
        lgg.gain.overrideState = true;
        IncreaseVignette();
    }

    #region RESET BLOOM SETTINGS
    void ResetBloomIntensity()
    {
        bloom.intensity.value = defaultBloomIntensity;
        lgg.gain.overrideState = false;
        ResetVignette();
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
        ResetVignette();
    }
    #endregion

    void IncreaseVignette()
    {
        vig.intensity.value = 0.3f;
    }

    void ResetVignette()
    {
        vig.intensity.value = 0.05f;
    }
}
