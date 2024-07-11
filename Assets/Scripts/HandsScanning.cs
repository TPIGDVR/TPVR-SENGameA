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



    private bool scanCompleted;
    public bool scanStarted = false;

    public bool ScanCompleted {  get { return scanCompleted; } }

    private void Start()
    {
        scanUI.fillAmount = 0f;
        
    }


    private void Update()
    {
        if (scanUI.fillAmount == 1)
        {
            scanCompleted = true;
        }
        else if (scanUI.fillAmount == 0)
        {
            canvas.SetActive(false);
        }
        

        if (scanStarted == true)
        {
            scanUI.fillAmount += Time.fixedDeltaTime / 100;
        }
        else if (scanStarted != true)
        {
            scanUI.fillAmount -= Time.fixedDeltaTime / 100;
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


