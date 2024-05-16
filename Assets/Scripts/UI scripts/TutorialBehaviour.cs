using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour
{
    //very simple script 
    private int index = 0;
    [SerializeField] private GameObject[] screens;
    [SerializeField] private GameObject backBtn;
    [SerializeField] private GameObject nextBtn;
    [SerializeField] private GameObject closeBtn;

    private void Start()
    {
        nextBtn.SetActive(true);
        closeBtn.SetActive(false);
        backBtn.SetActive(false);
        for(int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }
        screens[index].SetActive(true);
    }

    public void NextScreen()
    {
        //close the one before
        screens[index].SetActive(false);
        index++;
        screens[index].SetActive(true);
        ToggleButton();
    }

    public void PrevScreen()
    {
        screens[index].SetActive(false);
        index--;
        screens[index].SetActive(true);
        ToggleButton();
    }

    public void ToggleButton()
    {
        if(index == 0)
        {
            nextBtn.SetActive(true);
            backBtn.SetActive(false);
        }
        else if(index == screens.Length - 1)
        {
            nextBtn.SetActive(false);
            backBtn.SetActive(true);
            closeBtn.SetActive(true);
        }
        else 
        { 
            nextBtn.SetActive(true) ;
            backBtn.SetActive(true) ;
        }
    }
}
