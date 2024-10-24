using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour,IScriptLoadQueuer
{
    [SerializeField] Transform noiseIndicator;
    [SerializeField] CapsuleCollider c;
    public float NoiseRange = 10;
    public float NoiseValue = 5;
    public float NoiseRangeScaled;
    public float meshOffset = 0.5f;
    AudioSource audioSource;
    AudioHighPassFilter highPassFilter;
    AudioLowPassFilter lowPassFilter;
    MeshRenderer meshRenderer;

    public void Initialize()
    {
        GetComponent();

        noiseIndicator.localScale = new Vector3(NoiseRange, NoiseRange, NoiseRange) +
            new Vector3(meshOffset, meshOffset, meshOffset);
        NoiseRangeScaled = (NoiseRange + c.radius) * LevelConstants.Scale;
        audioSource.maxDistance = NoiseRangeScaled;
        audioSource.minDistance = 0;
        audioSource.Play();

        EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_NOISE_INDICATOR, ActivateNoiseRangeIndicator);
        EventSystem.level.AddListener(LevelEvents.INIT_TUTORIAL, DeactivateNoiseRangeIndicator);
    }

    void GetComponent()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        highPassFilter = GetComponent<AudioHighPassFilter>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.AUTOMATONS - 1);
    }

    public bool CheckIfBlockedOrOutOfRange()
    {
        
        Transform camTrans = Camera.main.transform;
        Vector3 rayDir = camTrans.position - transform.position;
        Ray toPlayer = new(transform.position, rayDir);
        bool hasHit = Physics.Raycast(toPlayer, out RaycastHit hitInfo, NoiseRangeScaled);
        float dist = Vector3.Distance(camTrans.position, transform.position);
       
        if (hasHit)
        {
            bool cantHearNoise = (!hitInfo.transform.CompareTag("Player") &&
                !hitInfo.transform.CompareTag("MainCamera") &&
                !hitInfo.transform.CompareTag("Hand") &&
                !hitInfo.transform.CompareTag("Player Head")
                ) 
                || GameData.player.IsWearingHeadphones;
            if (cantHearNoise)
            {
                //when the player cant hear it

                highPassFilter.cutoffFrequency = 1000;
                lowPassFilter.cutoffFrequency = 300;
            }
            else
            {
                //when the player can hear it
                highPassFilter.cutoffFrequency = 0;
                lowPassFilter.cutoffFrequency = 22000;
            }

            return cantHearNoise || dist > NoiseRangeScaled;
        }

        //out of range
        return true;
    }
    
    void ActivateNoiseRangeIndicator()
    {
        meshRenderer.enabled = true;
    }

    void DeactivateNoiseRangeIndicator()
    {
        meshRenderer.enabled = false;
    }
}
