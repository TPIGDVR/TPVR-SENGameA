using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLights : MonoBehaviour
{
    Light _light;
    [SerializeField]Color _color;
    float randInterval;
    float randChance;

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
            randInterval = Random.Range(0, 1f);
            
            if (rV > randChance)
            {
                _light.enabled = false;
            }

            float rW = Random.Range(0.2f,0.5f);

            yield return new WaitForSeconds(rW);

            _light.enabled = true;

            yield return new WaitForSeconds(randInterval);
        }
    }
}
