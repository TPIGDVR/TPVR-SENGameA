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
    float defVignetteValue;
    Vector4 defGamma;
    Vector4 defGain;
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

        defaultBloomIntensity = bloom.intensity.value;
        defaultBloomThreshold = bloom.threshold.value;
        defaultBloomScatter = bloom.scatter.value;

        defVignetteValue = vig.intensity.value;
        defGamma = lgg.gamma.value;
        defGain = lgg.gain.value;
        lgg.active = false;
        em_p.AddListener<float>(PlayerEvents.SUNGLASSES_ON,ReduceBloomIntensity);
        em_p.AddListener(PlayerEvents.SUNGLASSES_OFF, ResetBloomIntensity);
    }

    void ReduceBloomIntensity(float reducePercentage)
    {
        print("reducing bloom");
        float r = 1 - reducePercentage;

        bloom.intensity.value = defaultBloomIntensity * r;
        lgg.active = true;
        IncreaseVignette();
        GameData.player.isWearingSunglasses = true;
    }

    #region RESET BLOOM SETTINGS
    void ResetBloomIntensity()
    {
        bloom.intensity.value = defaultBloomIntensity;
        ResetVignette();
        lgg.active = false;
        GameData.player.isWearingSunglasses = false;
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
        vig.intensity.value = 0.4f;
    }

    void ResetVignette()
    {
        vig.intensity.value = defVignetteValue;
        //lgg.gamma.value = defGamma;
        //lgg.gain.value = defGain;
    }
}
