Shader "NPR Paint Filter/Pencil Dot" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float _Thickness;

			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float2 p = 1.0 / _ScreenParams.xy;
				float4 sum = 0.0;
				for (int a = -1.0 ; a <= 1.0 ; a ++) {
					for (int b = -1.0 ; b <= 1.0 ; b ++) {
						if (b == 0.0 && a == 0.0)
							sum += tex2D(_MainTex, uv + p * float2(b, a)) * 8.0;
						else
							sum += tex2D(_MainTex, uv + p * float2(b, a)) * -1.0;
					}
				}
				float avg = (sum.r + sum.g + sum.b) / (10.0 - _Thickness);
				float v = (1.0 - (1.0 / (1.0 + exp(-9.0 * (avg - 0.18))))) * 2.0;
				return float4(v.xxx, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}