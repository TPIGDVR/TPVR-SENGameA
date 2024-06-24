using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PulseScript : MonoBehaviour
{
    [SerializeField] private RawImage image;
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Color waveColor = Color.red; // Set the float4 value you want to use
    public Color backGroundColor = Color.black;
    private int kernelHandle;
    private Texture2D outputTexture;

    void Start()
    {
        int width = (int)image.uvRect.width;
        int height = (int)image.uvRect.height;
        print($"Height {height} and width {width}");
        // Initialize the RenderTexture
        renderTexture = new RenderTexture(width, height, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Find the kernel
        kernelHandle = computeShader.FindKernel("CSMain");

        // Set the texture on the compute shader
        computeShader.SetTexture(kernelHandle, "Result", renderTexture);
        computeShader.SetVector("colorOfTheWave", waveColor);
        computeShader.SetVector("BackgroundColor", backGroundColor);
        computeShader.SetInt("width" , width); 
        computeShader.SetInt("height" , height);

        // Initialize the output Texture2D
        outputTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        image.texture = outputTexture;

    }

    async void Update()
    {
        // Dispatch the compute shader
        computeShader.Dispatch(kernelHandle, 1, 1, 1);

        // Wait for the compute shader to finish
        await WaitForComputeShader();

        // Continue with the rest of your code (e.g., apply the render texture)
        CopyRenderTextureToTexture2D(renderTexture, outputTexture);
        image.texture = outputTexture;

        Debug.Log("Compute shader job completed.");
    }

    async Task WaitForComputeShader()
    {
        // Check the status of the GPU
        AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(renderTexture);

        // Wait for the request to complete
        while (!request.done)
        {
            await Task.Yield(); // Yield control back to the main thread
        }

        if (request.hasError)
        {
            Debug.LogError("Error while waiting for the compute shader to complete.");
        }
    }

    void CopyRenderTextureToTexture2D(RenderTexture renderTexture, Texture2D texture2D)
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = currentActiveRT;
    }
}
