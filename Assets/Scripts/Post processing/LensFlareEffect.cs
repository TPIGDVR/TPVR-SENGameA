using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class LensFlareEffect: MonoBehaviour
{

    [SerializeField] private LensFlareComponentSRP lensFlare;

    // Start is called before the first frame update
    void Start()
    {
        lensFlare.intensity = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
        lensFlare.intensity = Mathf.PingPong(Time.time, 3.4f);

    }
}
