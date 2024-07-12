using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLights : MonoBehaviour
{
    MeshRenderer _renderer;
    [SerializeField] Material[] _mats;
    [SerializeField] float minRandInterval_WAIT;
    [SerializeField] float maxRandInterval_WAIT;
    [SerializeField] float minRandInterval_DELAY;
    [SerializeField] float maxRandInterval_DELAY;
    float randInterval;
    [SerializeField]float randChance;

    private void Start()
    {
        int index = Random.Range(0, _mats.Length);
        if(index == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        _renderer = GetComponent<MeshRenderer>();
        _renderer.material = _mats[index];
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
                _renderer.enabled = false;
                //_light.enabled = false;
            }

            float rW = Random.Range(minRandInterval_DELAY,maxRandInterval_DELAY);

            yield return new WaitForSeconds(rW);

            _renderer.enabled = true;
            //_light.enabled = true;

            yield return new WaitForSeconds(randInterval);
        }
    }
}
