Shader "Hidden/Glitch/Analog" {
    HLSLINCLUDE
        #include "../PostProcessLibrary/Core.hlsl"

        float2 _ScanLineJitter; // (displacement, threshold)
        float2 _VerticalJump;   // (amount, time)
        float _HorizontalShake;
        float2 _ColorDrift;     // (amount, time)

        float2 _URegion;
        float2 _VRegion;

        float nrand(float x, float y) {
            return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
        }

        float4 AnalogGlitchFrag(PostProcessVaryings i) : SV_Target {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            float2 uv = UnityStereoTransformScreenSpaceTex(i.texcoord);
            if (uv.x < _URegion.x || uv.x > _URegion.y || uv.y < _VRegion.x || uv.y > _VRegion.y) return LOAD_TEXTURE2D_X(_CameraColorTexture, uv * _ScreenSize.xy);

            float u = uv.x;
            float v = uv.y;

            // Scan line jitter
            float jitter = nrand(v, _Time.x) * 2 - 1;
            jitter *= step(_ScanLineJitter.y, abs(jitter)) * _ScanLineJitter.x;

            // Vertical jump
            float jump = lerp(v, frac(v + _VerticalJump.y), _VerticalJump.x);

            // Horizontal shake
            float shake = (nrand(_Time.x, 2) - 0.5) * _HorizontalShake;

            // Color drift
            float drift = sin(jump + _ColorDrift.y) * _ColorDrift.x;

            float4 src1 = LOAD_TEXTURE2D_X(_CameraColorTexture, frac(float2(u + jitter + shake, jump)) * _ScreenSize.xy);
            float4 src2 = LOAD_TEXTURE2D_X(_CameraColorTexture, frac(float2(u + jitter + shake + drift, jump)) * _ScreenSize.xy);

            return float4(src1.r, src2.g, src1.b, 1);
        }
    ENDHLSL

    Subshader {
        Pass {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
                #pragma vertex FullScreenTrianglePostProcessVertexProgram
                #pragma fragment AnalogGlitchFrag
                #pragma target 3.0
            ENDHLSL
        }
    }
    Fallback Off
}
