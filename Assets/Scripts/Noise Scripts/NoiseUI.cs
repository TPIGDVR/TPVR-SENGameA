using Patterns;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoiseUI : Singleton<NoiseUI>
{
    private Transform playerCameraPosition;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider slider;
    private NoiseSource targetNoiseSource;
    private void Start()
    {
        playerCameraPosition = Camera.main.transform;
        targetNoiseSource = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //make sure that the UI face to the camera.
        var rotation = Quaternion.LookRotation(playerCameraPosition.forward);
        Vector3 eularRotation = rotation.eulerAngles;
        eularRotation.z = 0;
        transform.rotation = Quaternion.Euler(eularRotation);
        UpdateUI();
    }

    public void HideText()
    {
        targetNoiseSource = null;
        gameObject.SetActive(false);
    }

    public void SpawnText(NoiseSource source , PlacementStrategy strategy) 
    { 
        targetNoiseSource = source;
        NameText.text = targetNoiseSource.name;
        transform.position = strategy.Setposition(source.Position);
        gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        if (targetNoiseSource == null) return;
        slider.value = CalculateNoise();
    }

    private float CalculateNoise()
    {//normalize value
        var dist = Vector3.Distance(targetNoiseSource.Position, playerCameraPosition.position);
        if (dist > targetNoiseSource.NoiseValue) return 0f;

        float normalise = dist / targetNoiseSource.NoiseValue;
        return 1 - normalise;
    }
}
