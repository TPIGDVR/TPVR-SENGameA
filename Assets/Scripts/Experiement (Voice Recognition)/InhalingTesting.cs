using Caress.Examples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace Breathing3
{
    public class InhalingTesting : MonoBehaviour
    {
        [Range(0, 24000)]
        [SerializeField]
        float lowPassFilter= 900f;
        [SerializeField]
        float highPassFilter = 8000f;
        
        [SerializeField]
        int minNumberOfPointToHit;
        [SerializeField]
        int maxNumberOfPointToHit;
        
        [SerializeField]
        float minAmpThreshold = 0.0006f;
        [SerializeField]
        float MaxAmpThreshold;
        [SerializeField]
        AudioVisualizer visualizer;
        const float spectrumInc = 24000f / 1024f;

        [SerializeField] NewMicController volumeProvider;
        [SerializeField] float maxDPThreshold;

        [SerializeField] TextMeshProUGUI text;
        private void Update()
        {
            GetDataToCalculate();
        }
        public void GetDataToCalculate()
        {
            int lowCutOff = (int)(lowPassFilter / spectrumInc);
            int highCutOff = (int)(highPassFilter / spectrumInc);

            int counter = 0;
            for (int i = lowCutOff; i < highCutOff; i++)
            {
                var val = visualizer._data[i];
                if (val > minAmpThreshold && val < MaxAmpThreshold)
                {
                    counter++;
                }
            }

            print($"Result :{counter}");
            if(counter > minNumberOfPointToHit && 
                counter < maxNumberOfPointToHit &&
                volumeProvider.CalculatedVolume < maxDPThreshold
                )
            {
                print("Inhaling");
                text.text = "Inhaling";
            }
            else
            {
                text.text = "exhaling";
            }

        }

    }
}