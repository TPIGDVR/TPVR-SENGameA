using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class AnxietyUIHandler : MonoBehaviour
{
    [SerializeField]
    Image _anxietyBar,_anxietyLogo;
    [SerializeField]
    Sprite[] _anxietySpriteLogos = new Sprite[4];

    EventManager em = EventManager.Instance;

    private void Start()
    {
        em.AddListener<float>(Event.ANXIETY_UPDATE, UpdateAnxietyUI);
    }

    void UpdateAnxietyUI(float anxiety)
    {
        SwitchAnxietyIcon(anxiety);
        UpdateProgressBar(anxiety);
    }

    void SwitchAnxietyIcon(float progress)
    {
        Sprite chosenSprite;

        if (progress < 0.25f)
        {//less than 25 percentage
            chosenSprite = _anxietySpriteLogos[0];
        }
        else if (progress < .5f)
        {
            chosenSprite = _anxietySpriteLogos[1];
        }
        else if (progress < .75f)
        {
            chosenSprite = _anxietySpriteLogos[2];
        }
        else
        {
            chosenSprite = _anxietySpriteLogos[3];
        }

        _anxietyLogo.sprite = chosenSprite;
    }

    void UpdateProgressBar(float progress)
    {
        _anxietyBar.fillAmount = progress;
    }
}
