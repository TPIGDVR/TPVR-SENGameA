using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class HandsScanning : MonoBehaviour
{

    [SerializeField]
    private Image scanUI;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private float timeMultiplier;
    [SerializeField]
    private AudioSource completeSound;



    private bool scanCompleted;
    private bool playSongOnce;
    public bool scanStarted = false;

    public bool ScanCompleted {  get { return scanCompleted; } }

    private void Start()
    {
        scanUI.fillAmount = 0f;
        
    }


    private void Update()
    {
        if (scanCompleted)
        {

            if (!playSongOnce)
            {
                completeSound.Play();
                playSongOnce = true;
            }

            Debug.Log("Scan complete!!!");


        }
        else
        {
            if (scanUI.fillAmount == 1)
            {
                scanCompleted = true;
            }


            if (scanStarted == true)
            {
                scanUI.fillAmount += Time.fixedDeltaTime / 100;
            }
            else if (scanStarted != true)
            {
                scanUI.fillAmount -= Time.fixedDeltaTime / 100;
            }

            if (scanUI.fillAmount == 0)
            {
                canvas.SetActive(false);
            }
        }
    }

    public void ScanStart()
    {
        canvas.SetActive(true);
        scanStarted = true;
        
    }


    public void ScanStop()
    {
        scanStarted = false;
    }



}


