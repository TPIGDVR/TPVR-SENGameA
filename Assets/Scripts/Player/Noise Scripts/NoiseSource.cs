using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [SerializeField] Transform noiseIndicator;
    [SerializeField] CapsuleCollider c;
    public float NoiseRange = 10;
    public float NoiseValue = 5;
    public float NoiseRangeScaled;
    AudioSource audioSource;
    AudioHighPassFilter highPassFilter;
    AudioLowPassFilter lowPassFilter;
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        noiseIndicator.localScale = new Vector3(NoiseRange, NoiseRange, NoiseRange);
        NoiseRangeScaled = (NoiseRange + c.radius) * LevelConstants.Scale;
        audioSource.maxDistance = NoiseRangeScaled;
        audioSource.minDistance = 0;
        //audio.PlayDelayed(Random.Range(0, 3));
        audioSource.Play();
        highPassFilter = GetComponent<AudioHighPassFilter>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();

        EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_NOISE_INDICATOR, ActivateNoiseRangeIndicator);
        EventSystem.tutorial.AddListener(TutorialEvents.INIT_TUTORIAL, DeactivateNoiseRangeIndicator);
    }

    public bool CheckIfBlockedOrOutOfRange()
    {
        Transform camTrans = Camera.main.transform;
        Vector3 rayDir = camTrans.position - transform.position;
        Ray toPlayer = new(transform.position, rayDir);

        bool hasHit = Physics.Raycast(toPlayer, out RaycastHit hitInfo, NoiseRangeScaled);
        float dist = Vector3.Distance(camTrans.position, transform.position);
        
        //for debugging
        //if(dist <= NoiseRangeScaled)
        //{
        //    Debug.DrawLine(camTrans.position, transform.position,Color.red);
        //    print($"Has hit, {hasHit} Gameobject {hitInfo.transform.name}");
        //}


        if (hasHit)
        {
            if (!hitInfo.transform.CompareTag("Player"))
            {
                highPassFilter.cutoffFrequency = 1000;
                lowPassFilter.cutoffFrequency = 300;
            }
            else
            {
                highPassFilter.cutoffFrequency = 0;
                lowPassFilter.cutoffFrequency = 22000;
            }

            return !hitInfo.transform.CompareTag("Player") || dist > NoiseRangeScaled;
        }

        //out of range
        return true;
    }
    
    void ActivateNoiseRangeIndicator()
    {

        Debug.Log("ACTIVATE INDICATOR");
        meshRenderer.enabled = true;
    }

    void DeactivateNoiseRangeIndicator()
    {
        Debug.Log("DEACTIVATE INDICATOR");
        meshRenderer.enabled = false;
    }
}
