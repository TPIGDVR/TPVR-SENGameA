using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchVisualiser : MonoBehaviour
{
    public GameObject sampleCubePrefab;
    public int numberOfSamples = 64;
    public float maxHeight = 20.0f;
    public float spacing = 1.5f;
    private GameObject[] sampleCubes;
    private AudioSource audioSource;
    [SerializeField] float multiplier = 1.0f;
    [SerializeField] private FFTWindow window = FFTWindow.BlackmanHarris;
    [SerializeField] private int channel = 0; 
    float[] spectrumData; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        if (Microphone.devices.Length > 0)
        {
            string micName = Microphone.devices[0];
            audioSource.clip = Microphone.Start(micName, true, 1, AudioSettings.outputSampleRate);
            while (!(Microphone.GetPosition(micName) > 0)) { } // Wait until the recording has started
            audioSource.Play();
        }

        sampleCubes = new GameObject[numberOfSamples];
        spectrumData = new float[numberOfSamples];
        for (int i = 0; i < numberOfSamples; i++)
        {
            GameObject cube = Instantiate(sampleCubePrefab,transform);
            cube.transform.position = new Vector3(i * spacing, 0, 0);
            cube.transform.parent = this.transform;

            cube.SetActive(true);
            sampleCubes[i] = cube;
        }
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, channel, window);
        
        PrintArray(spectrumData);

        for (int i = 0; i < numberOfSamples; i++)
        {
            float scaleY = Mathf.Clamp(spectrumData[i] * maxHeight, 0.1f, maxHeight);
            Vector3 newScale = new Vector3(1, scaleY * multiplier, 1);
            sampleCubes[i].transform.localScale = newScale;
        }
    }

    void PrintArray(float[] array)
    {
        string arrayString = "";
        for (int i = 0; i < array.Length; i++)
        {
            arrayString += array[i].ToString("F4") + " "; // Format to 4 decimal places for readability
        }
        Debug.Log(arrayString);
    }
}
