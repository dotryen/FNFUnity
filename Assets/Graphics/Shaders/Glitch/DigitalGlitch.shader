Shader "Hidden/Glitch/Digital" {
	HLSLINCLUDE
		#include "../PostProcessLibrary/Core.hlsl"

		TEXTURE2D_X(_NoiseTex);
		TEXTURE2D_X(_TrashTex);
		float _Intensity;
		float2 _URegion;
		float2 _VRegion;

		float4 DigitalGlitchFrag(PostProcessVaryings i) : SV_Target {
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			float2 texcoord = UnityStereoTransformScreenSpaceTex(i.texcoord);
			if (texcoord.x < _URegion.x || texcoord.x > _URegion.y || texcoord.y < _VRegion.x || texcoord.y > _VRegion.y) return LOAD_TEXTURE2D_X(_CameraColorTexture, texcoord * _ScreenSize.xy);

			float4 glitch = LOAD_TEXTURE2D_X(_NoiseTex, texcoord * float2(64, 32));

			float thresh = 1.001 - _Intensity * 1.001;
			float w_d = step(thresh, pow(glitch.z, 2.5)); // displacement glitch
			float w_f = step(thresh, pow(glitch.w, 2.5)); // frame glitch
			float w_c = step(thresh, pow(glitch.z, 3.5)); // color glitch

			// Displacement.
			float2 uv = frac(texcoord + glitch.xy * w_d);

			float4 source = LOAD_TEXTURE2D_X(_CameraColorTexture, uv * _ScreenSize.xy);

			// Mix with trash frame.
			float3 color = lerp(source, LOAD_TEXTURE2D_X(_TrashTex, uv * _ScreenSize.xy), w_f).rgb;

			// Shuffle color components.
			float3 neg = saturate(color.grb + (1 - dot(color, 1)) * 0.5);
			color = lerp(color, neg, w_c);

			return float4(color, source.a);
		}
	ENDHLSL

	Subshader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			HLSLPROGRAM
				#pragma vertex FullScreenTrianglePostProcessVertexProgram
				#pragma fragment DigitalGlitchFrag
			ENDHLSL
		}
	}
}