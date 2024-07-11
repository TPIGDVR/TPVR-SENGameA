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



    private bool scanCompleted;
    private bool scanStarted = false;

    public bool ScanCompleted {  get { return scanCompleted; } }


    private void Update()
    {
        if (scanStarted == true)
        {

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


