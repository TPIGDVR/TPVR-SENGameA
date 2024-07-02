Shader "Hidden/LuminanceTexShader"
{

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off
        

        Pass
        {
            Name "Grab luminance tex"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            SamplerState point_clamp_sampler;

            float luminance(float3 col) {
                return dot(col, float3(0.299f, 0.587f, 0.114f));
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, i.texcoord);
                return luminance(col);
            }
            ENDHLSL
        }
    }
}
