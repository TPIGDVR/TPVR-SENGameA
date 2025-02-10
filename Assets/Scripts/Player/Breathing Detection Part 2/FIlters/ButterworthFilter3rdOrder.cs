using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A class that replicates MATLAB's [b,a] = butter(3, Wn, 'low') for a 3rd-order low-pass filter.
/// Wn should be normalized (0 to 1, where 1 means the Nyquist frequency).
/// </summary>
public class ButterworthFilter3rdOrder
{
    // Filter order (n = 3, so we have 4 coefficients in b and a)
    private const int Order = 3;

    // Coefficient arrays (for a 3rd-order filter, indices 0..3)
    private readonly float[] a = new float[Order + 1];  // a[0] is always 1 (difference equation convention)
    private readonly float[] b = new float[Order + 1];

    // State arrays to store past input and output samples.
    private readonly float[] x = new float[Order + 1];
    private readonly float[] y = new float[Order + 1];

    /// <summary>
    /// Creates a new 3rd-order Butterworth low-pass filter.
    /// </summary>
    /// <param name="normalizedCutoff">
    /// The normalized cutoff frequency Wn (cutoff/(fs/2)) – a value between 0 and 1.
    /// </param>
    /// <param name="samplingFrequency">
    /// The sampling frequency in Hz.
    /// </param>
    public ButterworthFilter3rdOrder(float normalizedCutoff)
    {
        // For a digital design, MATLAB uses the normalized cutoff relative to Nyquist.
        // Here we assume normalizedCutoff = cutoff / (fs/2).
        // Compute the coefficients to replicate [b,a] = butter(3, normalizedCutoff, 'low')
        ComputeCoefficients(normalizedCutoff);
        Reset();
    }

    /// <summary>
    /// Computes the filter coefficients.
    /// Replicates MATLAB's butter(3, Wn, 'low') operation.
    /// </summary>
    /// <param name="Wn">Normalized cutoff frequency (0 to 1, where 1 corresponds to fs/2).</param>
    private void ComputeCoefficients(float Wn)
    {
        // Pre-warp the normalized cutoff.
        // In the bilinear transform, the pre-warp factor is:
        //   c = 1 / tan(pi*Wn/2)
        float c = 1.0f / Mathf.Tan(Mathf.PI * Wn / 2.0f);

        // Compute the denominator normalization factor.
        float a0 = 1.0f + 3.0f * c + 3.0f * c * c + c * c * c;

        // Set the feedforward (b) coefficients.
        b[0] = 1.0f / a0;
        b[1] = 3.0f / a0;
        b[2] = 3.0f / a0;
        b[3] = 1.0f / a0;

        // Set the feedback (a) coefficients.
        // a[0] is 1 by convention.
        a[0] = 1.0f;
        a[1] = (3.0f - 3.0f * c * c * c) / a0;
        a[2] = (3.0f - 2.0f * c + 2.0f * c * c - 2.0f * c * c * c) / a0;
        a[3] = (1.0f - 2.0f * c + 2.0f * c * c - c * c * c) / a0;
    }

    /// <summary>
    /// Resets the filter's internal state (clearing past inputs and outputs).
    /// Useful for real-time applications when restarting the filter.
    /// </summary>
    public void Reset()
    {
        for (int i = 0; i <= Order; i++)
        {
            x[i] = 0.0f;
            y[i] = 0.0f;
        }
    }

    /// <summary>
    /// Processes a single sample through the filter.
    /// </summary>
    /// <param name="sample">The input sample.</param>
    /// <returns>The filtered output sample.</returns>
    public float ProcessSample(float sample)
    {
        // Shift the state history.
        x[3] = x[2];
        x[2] = x[1];
        x[1] = x[0];
        x[0] = sample;

        y[3] = y[2];
        y[2] = y[1];
        y[1] = y[0];

        // Difference equation:
        // y[0] = b[0]*x[0] + b[1]*x[1] + b[2]*x[2] + b[3]*x[3]
        //        - a[1]*y[1] - a[2]*y[2] - a[3]*y[3]
        y[0] = b[0] * x[0] +
               b[1] * x[1] +
               b[2] * x[2] +
               b[3] * x[3] -
               a[1] * y[1] -
               a[2] * y[2] -
               a[3] * y[3];

        return y[0];
    }

    /// <summary>
    /// Processes an entire buffer of samples through the filter.
    /// </summary>
    /// <param name="data">An array of input samples.</param>
    /// <returns>A new array containing the filtered samples.</returns>
    public float[] ProcessBuffer(float[] data)
    {
        float[] output = new float[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            output[i] = math.sqrt(ProcessSample(data[i] * data[i]));
        }
        return output;
    }

    /// <summary>
    /// (Optional) Exposes the computed b coefficients.
    /// </summary>
    public float[] BCoefficients => b;

    /// <summary>
    /// (Optional) Exposes the computed a coefficients.
    /// </summary>
    public float[] ACoefficients => a;
}
