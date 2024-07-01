using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrawLineRendererPass : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        LineRenderer lineRenderer;
        RenderTargetIdentifier cameraTarget;
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("DrawLineRendererPass");

            // Set the render target and clear if necessary

            using (new ProfilingScope(cmd, new("HEARTLINE")))
            {
                lineRenderer = GameObject.FindGameObjectWithTag("Heart").GetComponent<LineRenderer>();
                //lineRenderer.sharedMaterial.renderQueue = 5000;
                lineRenderer.forceRenderingOff = false; // Ensure it renders if needed
                cmd.SetRenderTarget(cameraTarget);
                cmd.DrawRenderer(lineRenderer, lineRenderer.sharedMaterial);
                
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

    }

    CustomRenderPass m_ScriptablePass;
    public int renderInt;
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass
        {
            // Configure the pass settings if necessary
            renderPassEvent = RenderPassEvent.AfterRendering + renderInt
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
