Shader "NPR Paint Filter/Cartoon" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
		_Resolution ("Resolution", Vector) = (512, 512, 0, 0)
		_Threshold ("Threshold", Range(0, 1)) = 0.35
	}
	SubShader {
		ZTest Off Cull Off ZWrite Off Blend Off Fog { Mode Off }
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Resolution;
			float _Threshold;

			float sample (float x, float y)
			{
				return tex2D(_MainTex, float2(x, y) / _Resolution.xy).r;
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float2 uvst = input.uv;
				float x = uvst.x * _Resolution.x;
				float y = uvst.y * _Resolution.y;

				float v1 = -sample(x-1.0, y-1.0) - 2.0*sample(x-1.0, y) - sample(x-1.0, y+1.0) + sample(x+1.0, y-1.0) + 2.0*sample(x+1.0, y) + sample(x+1.0, y+1.0);
				float v2 = sample(x-1.0, y-1.0) + 2.0*sample(x, y-1.0) + sample(x+1.0, y-1.0) - sample(x-1.0, y+1.0) - 2.0*sample(x, y+1.0) - sample(x+1.0, y+1.0);

				return (length(float2(v1, v2)) > _Threshold) ? 0.0 : tex2D(_MainTex, float2(x, y) / _Resolution.xy);
			}
			ENDCG
		}
	}
	FallBack Off
}