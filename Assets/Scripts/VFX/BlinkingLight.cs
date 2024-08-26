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

    Coroutine blink;
    int index;
    private void Start()
    {
        index = Random.Range(1, _mats.Length);
        if (index == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        _renderer = GetComponent<MeshRenderer>();
        _renderer.material = _mats[index];
        //blink = StartCoroutine(BlinkLights());
    }
    void OnBecameVisible()
    {
        blink = StartCoroutine(BlinkLights());

        //print("became visible");
    }

    void OnBecameInvisible()
    {
        StopCoroutine(blink);
        print("became invisible");
    }

    IEnumerator BlinkLights()
    {
        while (true)
        {
            float rV = Random.value;
            randInterval = Random.Range(minRandInterval_WAIT, maxRandInterval_WAIT);
            
            if (rV > randChance)
            {
                _renderer.material = _mats[0];
                //_light.enabled = false;
            }

            float rW = Random.Range(minRandInterval_DELAY,maxRandInterval_DELAY);

            yield return new WaitForSeconds(rW);

            _renderer.material = _mats[index];
            //_light.enabled = true;

            yield return new WaitForSeconds(randInterval);
        }
    }
}
