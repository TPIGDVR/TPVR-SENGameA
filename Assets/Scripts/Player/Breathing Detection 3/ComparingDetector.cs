using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Breathing_Detection_3
{
    public class ComparingDetector :MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioSource audioSource; // Drag your AudioSource here.
        public SampleSize fftSize = SampleSize._4096;      // FFT size; must be a power of 2.
        public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
        
        [Header("MFCC Settings")]
        public int numMelFilters = 26;  // Number of Mel filters.
        public int numMFCCs = 13;    // Number of MFCC coefficients to compute.

        // Buffer to hold spectrum data (only half the FFT is used)
        private float[] spectrumData;
        
        [Header("MFCC for inhale and exhaling")]
        public int windowSize = 10;
        
        private Queue<float[]> inhaleMFCCs = new ();
        private Queue<float[]> exhaleMFCCs = new();

        [SerializeField]private float ignoringDistnace = 120f;

        private bool canDetectInhaling = false;
        private bool canDetectExhaling = false;
        [SerializeField] private TextMeshProUGUI output;

        [Header("Smoothing function")]
        [SerializeField] private int smoothing = 10;
        [SerializeField] private bool useSmoothing = true; // Toggle smoothing
        
        [Header("cutoff")]
        [SerializeField] private bool useCutoff = true;
        [SerializeField] private Vector2Int cutoff;
        
        [Header("visualiser")]
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] private float size = 5f;
        [SerializeField] private float visualizerSize = 5f;
        void Start()
        {
            // If no AudioSource was assigned, try to get one from the same GameObject.
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            // Prepare the spectrum data buffer (FFT returns fftSize/2+1 bins)
            // spectrumData = new float[fftSize / 2 + 1];
        }

        void Update()
        {
            var mfccCoeffs = GenerateCoeff();

            if (canDetectInhaling)
            {
                inhaleMFCCs.Enqueue(mfccCoeffs);
                print("Inhale MFCC sampled: " + Output(mfccCoeffs));
                while (inhaleMFCCs.Count > windowSize)
                {
                    inhaleMFCCs.Dequeue();
                }
            }
            else if (canDetectExhaling)
            {
                exhaleMFCCs.Enqueue(mfccCoeffs);
                print("Exhale MFCC sampled: " + Output(mfccCoeffs));
                while (exhaleMFCCs.Count > windowSize)
                {
                    exhaleMFCCs.Dequeue();
                }
            }
            else
            {
                print("MFCC sampled: " + Output(mfccCoeffs));
            }

            if (inhaleMFCCs.Count > 0 && exhaleMFCCs.Count > 0)
            {
                string result = ClassifyMFCC(mfccCoeffs, inhaleMFCCs, exhaleMFCCs);
                output.text = result;
            }

            VisualiseData();
        }

        /// <summary>
        /// Smooths the spectrum data using a simple moving average.
        /// </summary>
        /// <param name="data">Reference to the spectrum data array</param>
        /// <param name="windowSize">Number of adjacent values to average</param>
        void SmoothSpectrumData(ref float[] data, int windowSize)
        {
            int halfWindow = windowSize / 2;

            for (int i = 0; i < data.Length; i++)
            {
                float sum = 0;
                int count = 0;

                for (int j = -halfWindow; j <= halfWindow; j++)
                {
                    int index = i + j;
                    if (index >= 0 && index < data.Length)
                    {
                        sum += data[index];
                        count++;
                    }
                }

                data[i] = sum / count;
            }
            
        }
        
        // Computes the Euclidean distance between two MFCC vectors.
        float EuclideanDistance(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new System.ArgumentException("Vectors must be the same length");

            float sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                float diff = a[i] - b[i];
                sum += diff * diff;
            }

            return Mathf.Sqrt(sum);
        }

// Computes the mean MFCC vector from a collection.
        float[] ComputeMeanMFCC(IEnumerable<float[]> mfccSamples)
        {
            int count = 0;
            float[] mean = null;

            foreach (var sample in mfccSamples)
            {
                if (mean == null)
                {
                    mean = new float[sample.Length];
                }

                for (int i = 0; i < sample.Length; i++)
                {
                    mean[i] += sample[i];
                }

                count++;
            }

            if (count > 0)
            {
                for (int i = 0; i < mean.Length; i++)
                {
                    mean[i] /= count;
                }
            }

            return mean;
        }

// Classifies the new MFCC vector using the nearest-mean approach.
        string ClassifyMFCC(float[] newMFCC, Queue<float[]> inhaleSamples, Queue<float[]> exhaleSamples)
        {
            // Convert queues to lists for easier iteration (or iterate directly if you prefer).
            var inhaleList = new List<float[]>(inhaleSamples);
            var exhaleList = new List<float[]>(exhaleSamples);

            // Compute the average MFCC for each class.
            float[] meanInhale = ComputeMeanMFCC(inhaleList);
            float[] meanExhale = ComputeMeanMFCC(exhaleList);

            // Compute distances to each mean.
            float distanceToInhale = EuclideanDistance(newMFCC, meanInhale);
            float distanceToExhale = EuclideanDistance(newMFCC, meanExhale);

            // For debugging/logging.
            Debug.Log("Distance to Inhale: " + distanceToInhale);
            Debug.Log("Distance to Exhale: " + distanceToExhale);

            if (distanceToInhale > ignoringDistnace && distanceToExhale > ignoringDistnace)
            {
                return "ignore";
            }
            
            return (distanceToInhale < distanceToExhale) ? "Inhale" : "Exhale";
        }

        public void ToggleInhaling(bool val)
        {
            print($"inhaling val change {val}");
            canDetectInhaling = val;
        }
        
        public void ToggleExhaling(bool val)
        {
            print($"Exhaling val change {val}");
            canDetectExhaling = val;
        }

        private string Output(float[] mfccCoeffs)
        {
            // For demonstration, print the MFCC coefficients to the Console.
            string output = "MFCCs: ";
            for (int i = 0; i < mfccCoeffs.Length; i++)
            {
                output += mfccCoeffs[i].ToString("F2") + " ";
            }

            return output;
        }

        private void ApplyFrequencyCutoff(ref float[] data, float lowCutoff, float highCutoff)
        {
            //nquist frequency
            float sampleRate = AudioSettings.outputSampleRate / 2;

            // Convert frequencies to FFT bin indices
            int lowIndex = Mathf.FloorToInt(((float)lowCutoff / sampleRate) * (float) fftSize);
            int highIndex = Mathf.FloorToInt(((float)highCutoff / sampleRate) * (float) fftSize);
            
            lowIndex = Mathf.Clamp(lowIndex, 0, spectrumData.Length - 1);
            highIndex = Mathf.Clamp(highIndex, 0, spectrumData.Length - 1);

            int newLength = highIndex - lowIndex + 1;
            float[] newArray = new float[newLength];

            // Copy the relevant subarray
            Array.Copy(spectrumData, lowIndex, newArray, 0, newLength);

            // Override the reference
            spectrumData = newArray;
        }
        
        private float[] GenerateCoeff()
        {
            spectrumData = new float[(int)(fftSize)];

            // Get the spectrum data from the AudioSource (using channel 0 and a window function)
            audioSource.GetSpectrumData(spectrumData, 0, fftWindow);
            
            //cut off the frequency
            if (useCutoff)
            {
                ApplyFrequencyCutoff(ref spectrumData, cutoff.x, cutoff.y);
            }
            
            // Apply smoothing if enabled
            if (useSmoothing)
            {
                SmoothSpectrumData(ref spectrumData, smoothing); // Adjust window size as needed
            }
            
            // Convert the amplitude spectrum to a power spectrum (squaring each value)
            float[] powerSpectrum = new float[spectrumData.Length];
            for (int i = 0; i < spectrumData.Length; i++)
            {
                powerSpectrum[i] = spectrumData[i] * spectrumData[i];
            }

            // Compute the MFCC coefficients using our MFCCExtractor
            float[] mfccCoeffs = MFCCExtractor.ComputeMFCC(
                powerSpectrum, 
                useCutoff ? cutoff.y : AudioSettings.outputSampleRate / 2, 
                (int)fftSize, 
                numMelFilters, 
                numMFCCs
            );
            return mfccCoeffs;
        }

        void VisualiseData()
        {
            lineRenderer.positionCount = spectrumData.Length;
            for (int i = 0; i < spectrumData.Length; i++ )
            {
                lineRenderer.SetPosition(i, new Vector3(i * size, spectrumData[i] * visualizerSize, 0f));
            }
        }
    }
    
    /// <summary>
/// A helper class to compute Mel-Frequency Cepstral Coefficients (MFCCs) from a power spectrum.
/// </summary>
public class MFCCExtractor
{
    /// <summary>
    /// Compute MFCC coefficients from a power spectrum.
    /// </summary>
    /// <param name="powerSpectrum">Power spectrum (squared magnitude of FFT data)</param>
    /// <param name="sampleRate">Audio sample rate (e.g., 44100)</param>
    /// <param name="fftSize">FFT size used to produce the spectrum</param>
    /// <param name="numFilters">Number of Mel filters to use (e.g., 26)</param>
    /// <param name="numCoeffs">Number of MFCC coefficients to output (typically 12 or 13)</param>
    /// <returns>Array of MFCC coefficients</returns>
    public static float[] ComputeMFCC(float[] powerSpectrum, int frequency, int fftSize, int numFilters, int numCoeffs)
    {
        // 1. Create the Mel filter bank.
        float[][] melFilterBank = CreateMelFilterBank(numFilters, fftSize, frequency);

        // 2. Apply the filter bank to the power spectrum.
        float[] filterEnergies = new float[numFilters];
        for (int i = 0; i < numFilters; i++)
        {
            float sum = 0f;
            for (int j = 0; j < powerSpectrum.Length; j++)
            {
                sum += powerSpectrum[j] * melFilterBank[i][j];
            }
            // Avoid log(0) by adding a small value.
            filterEnergies[i] = Mathf.Log(sum + 1e-10f);
        }

        // 3. Compute the DCT of the log energies to get the MFCCs.
        float[] mfcc = new float[numCoeffs];
        for (int k = 0; k < numCoeffs; k++)
        {
            float sum = 0f;
            for (int n = 0; n < numFilters; n++)
            {
                sum += filterEnergies[n] * Mathf.Cos(Mathf.PI * k * (n + 0.5f) / numFilters);
            }
            mfcc[k] = sum;
        }

        return mfcc;
    }

    /// <summary>
    /// Creates a Mel filter bank.
    /// </summary>
    /// <param name="numFilters">Number of filters in the bank</param>
    /// <param name="fftSize">FFT size</param>
    /// <param name="frequency">ending frequency </param>
    /// <returns>A 2D array where each row is a filter (one filter per FFT bin)</returns>
    private static float[][] CreateMelFilterBank(int numFilters, int fftSize, int frequency)
    {
        int numFFTbins = fftSize ; // Only positive frequencies.
        float[][] filterBank = new float[numFilters][];

        for (int i = 0; i < numFilters; i++)
        {
            filterBank[i] = new float[numFFTbins];
        }

        // Convert the frequency range (0 to Nyquist) into the Mel scale.
        float lowFreqMel = FrequencyToMel(0);
        float highFreqMel = FrequencyToMel(frequency);
        float[] melPoints = new float[numFilters + 2];
        for (int i = 0; i < melPoints.Length; i++)
        {
            melPoints[i] = lowFreqMel + (highFreqMel - lowFreqMel) * i / (numFilters + 1);
        }

        // Convert Mel points back to Hz.
        float[] freqPoints = new float[melPoints.Length];
        for (int i = 0; i < melPoints.Length; i++)
        {
            freqPoints[i] = MelToFrequency(melPoints[i]);
        }

        // Map Hz values to FFT bin numbers.
        int[] bin = new int[freqPoints.Length];
        for (int i = 0; i < freqPoints.Length; i++)
        {
            // fftSize+1 to include the Nyquist bin.
            bin[i] = Mathf.FloorToInt((fftSize + 1) * freqPoints[i] / frequency);
        }

        // Create the triangular filters.
        for (int i = 1; i <= numFilters; i++)
        {
            for (int j = bin[i - 1]; j < bin[i]; j++)
            {
                filterBank[i - 1][j] = (j - bin[i - 1]) / (float)(bin[i] - bin[i - 1]);
            }
            for (int j = bin[i]; j < bin[i + 1] && j < numFFTbins; j++)
            {
                filterBank[i - 1][j] = (bin[i + 1] - j) / (float)(bin[i + 1] - bin[i]);
            }
        }

        return filterBank;
    }

    /// <summary>
    /// Converts a frequency in Hz to the Mel scale.
    /// </summary>
    private static float FrequencyToMel(float frequency)
    {
        return 2595f * Mathf.Log10(1f + frequency / 700f);
    }

    /// <summary>
    /// Converts a Mel scale value back to frequency in Hz.
    /// </summary>
    private static float MelToFrequency(float melValue)
    {
        return 700f * (Mathf.Pow(10f, melValue / 2595f) - 1f);
    }
}
}