using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLights : MonoBehaviour
{
    Light _light;
    [SerializeField]Color _color;
    [SerializeField] float minRandInterval_WAIT;
    [SerializeField] float maxRandInterval_WAIT;
    [SerializeField] float minRandInterval_DELAY;
    [SerializeField] float maxRandInterval_DELAY;
    float randInterval;
    [SerializeField]float randChance;

    private void Start()
    {
        _light = GetComponent<Light>();
        _light.color = _color;
        StartCoroutine(BlinkLights());
    }

    IEnumerator BlinkLights()
    {
        while (true)
        {
            float rV = Random.value;
            randInterval = Random.Range(minRandInterval_WAIT, maxRandInterval_WAIT);
            
            if (rV > randChance)
            {
                _light.enabled = false;
            }

            float rW = Random.Range(minRandInterval_DELAY,maxRandInterval_DELAY);

            yield return new WaitForSeconds(rW);

            _light.enabled = true;

            yield return new WaitForSeconds(randInterval);
        }
    }
}
