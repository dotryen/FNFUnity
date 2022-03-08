Shader "Hidden/Yetman/PostProcess/ChromaSplit"
{
    HLSLINCLUDE
    #include "../PostProcessLibrary/Core.hlsl"

    // The split amount
    uint2 _Split;

    float4 ChromaSplitFragmentProgram(PostProcessVaryings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        uint2 positionSS = uv * _ScreenSize.xy;

        float4 color = LOAD_TEXTURE2D_X(_CameraColorTexture, positionSS);
        
        // Read read and blue from the neighbouring pixels at the given split offset
        color.r = LOAD_TEXTURE2D_X(_CameraColorTexture, positionSS - _Split).r;
        color.b = LOAD_TEXTURE2D_X(_CameraColorTexture, positionSS + _Split).b;
        
        return color;
    }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex FullScreenTrianglePostProcessVertexProgram
            #pragma fragment ChromaSplitFragmentProgram
            ENDHLSL
        }
    }
    Fallback Off
}
