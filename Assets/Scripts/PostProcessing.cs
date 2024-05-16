using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PostProcessing : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private LensFlareComponentSRP lensFlare;

    private Vignette _anxietyVignette;
    private LensDistortion _lensDistortion;
    private PaniniProjection _pp;
    public ProximityHapticFeedback proximityHapticFeedback;

    // Define the min and max values for intensity and distance.
    //public float minIntensity = 0f;
    //public float maxIntensity = 7f;
    public float minDistortionIntensity = 0f;
    public float maxDistortionIntensity = 0.7f;

    [SerializeField] private float maxXExpansion = 0.1f;
    [SerializeField] private float maxYExpansion = 0.8f;

    [SerializeField] Color anxietyVignetteCol1;
    [SerializeField] Color anxietyVignetteCol2;
    [SerializeField] float maxAnxietyVignetteIntensity;

    void Start()
    {
        postProcessingVolume.profile.TryGet(out _lensDistortion);
        postProcessingVolume.profile.TryGet(out _anxietyVignette);
        postProcessingVolume.profile.TryGet(out _pp);
        //lensFlare.intensity = minIntensity; // Start with min intensity.
        _lensDistortion.intensity.value = minDistortionIntensity; // Start with min distortion intensity.

        //Old Lens Distortion values, only using x and y multiplier for now.
        //_lensDistortion.intensity.value = 0;
        _lensDistortion.xMultiplier.value = 0;
        _lensDistortion.yMultiplier.value = 0;

        //anxiety vignette init
        _anxietyVignette.intensity.value = 0f;
        _anxietyVignette.color.value = anxietyVignetteCol1;

        //test
        _pp.distance.value = 0;
    }


    public void UpdatePostProcess(float progress)
    {
        if (progress > 0.5f) //when in the zone for 5 seconds, begin lens distortion (?)
        {
            _lensDistortion.active = true;
            _pp.active = true;
        }
        else
        {
            _lensDistortion.active = false;
            _pp.active = false;
        }

        float distortionIntensity = Mathf.Lerp(minDistortionIntensity,
            maxDistortionIntensity,
            progress);

        _lensDistortion.intensity.value = distortionIntensity;

        _lensDistortion.xMultiplier.value = Mathf.PingPong(Time.time, maxXExpansion);
        _lensDistortion.yMultiplier.value = Mathf.PingPong(Time.time, maxYExpansion);

        _pp.distance.value = Mathf.PingPong(Time.time * 0.2f, 0.2f);

        UpdateVignette(progress);
    }

    private void UpdateVignette(float progress)
    {
        if (progress <= 0.25f)
        {
            _anxietyVignette.intensity.value = Mathf.Clamp(progress / 0.4f, 0, maxAnxietyVignetteIntensity);
        }
        else if (progress <= 0.5f)
        {
            _anxietyVignette.color.value = Color.Lerp(anxietyVignetteCol1, anxietyVignetteCol2, (progress - 0.25f) / 0.25f);
        }
    }

    IEnumerator DeadAni()
    {
        float t = 0;
        float tInterval = 0.01f;
        while(t < 7)
        {
            yield return new WaitForSeconds(tInterval);
            t += tInterval;
            var c = Color.Lerp(anxietyVignetteCol2,Color.black, t/7);
            _anxietyVignette.color.value = c;
            _anxietyVignette.intensity.value = Mathf.Lerp(_anxietyVignette.intensity.value,2,t/7) ;
        }
        SceneManager.LoadScene("Restart Scene");

    }

    public void Dead()
    {
        StartCoroutine(DeadAni());
    }
}