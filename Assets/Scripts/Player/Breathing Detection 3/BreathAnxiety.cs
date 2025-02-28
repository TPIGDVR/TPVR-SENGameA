using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BreathAnxiety : MonoBehaviour
{
    //This script is used to link the breathing detection to the anxiety in the game.
    [SerializeField]
    BreathRecorder breath;

    public float cooldownTime = 4f;
    public float maxBreathTime = 1.75f;
    public float breathTime = 0;
    public float minInhaleTime = 0.5f;
    

    float inhaleTime = 0;
    float exhaleTime = 0;
    float cooldownTimer = 0;
    bool hasFinishedInhale = false;
    bool hasFinishedExhale = false;
    bool isInhaling = false;
    bool isExhaling = false;
    bool onCooldown = false;
    EventManager<PlayerEvents> em = EventSystem.player;

    [Header("UI")]
    [SerializeField] GameObject breathingPanel;
    [SerializeField] Image breathingImage;
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] Gradient breathingColorScheme;

    void Start()
    {
        //reset the ui panel
        breathingPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        UpdateFillColor(breathTime / maxBreathTime);

        if (onCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if(cooldownTimer >= cooldownTime)
            {
                onCooldown = false;
                cooldownTimer = 0;
            }
        }

        if (hasFinishedInhale && hasFinishedExhale)
        {
            hasFinishedInhale = false;
            hasFinishedExhale = false;
            isInhaling = false;
            isExhaling = false;
            onCooldown = true;

            float percentage = Mathf.InverseLerp(0, 2, inhaleTime / maxBreathTime + exhaleTime / maxBreathTime);
            breathTime = 0;
            em.TriggerEvent(PlayerEvents.ANXIETY_BREATHE, percentage);
        }

        if (breath.state == BreathState.Talking) return;

        if (breath.state == BreathState.Idle)
        {
            if (isInhaling)
            {
                hasFinishedInhale = true;
                inhaleTime = breathTime;
            }

            if (isExhaling)
            {
                hasFinishedExhale = true;
                exhaleTime = inhaleTime - breathTime;
                breathTime = 0;
            }
            return;
        }

        if (breath.state == BreathState.Inhale && !hasFinishedInhale)
        {
            breathTime = Mathf.Min(breathTime + Time.deltaTime, maxBreathTime);
            isInhaling = true;
        }

        if (breath.state == BreathState.Exhale && hasFinishedInhale)
        {
            breathTime = Mathf.Max(breathTime - Time.deltaTime, 0);
            isExhaling = true;
        }
    }


    public void ActivateBreathingPanel()
    {
        UpdateFillColor(0);
        breathingPanel.SetActive(true);
    }

    void UpdateFillColor(float normaliseValue)
    {
        breathingImage.color = breathingColorScheme.Evaluate(normaliseValue);
        breathingImage.fillAmount = normaliseValue;
    }

    void UpdateText()
        {
            
            switch (breath.state)
            {
                case BreathState.Idle:
                    stateText.text = "Waiting for inhale";
                break;
                case BreathState.Talking:
                    stateText.text = "Waiting for inhale";
                    break;
                case BreathState.Inhale:
                    stateText.text = "Inhaling";
                    break;
                case BreathState.Exhale:
                    stateText.text = "Exhaling";
                    break;
            }
        }
}
