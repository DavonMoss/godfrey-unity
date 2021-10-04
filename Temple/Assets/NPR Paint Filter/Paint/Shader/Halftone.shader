Shader "NPR Paint Filter/Halftone" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
		_Intensity ("Intensity", Range(0, 1)) = 0.3
		_Size ("Size", Range(1, 16)) = 3
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Intensity, _Size;

			float uvPattern (float2 uv)
			{
				float d = 2136.2812 / _Size;
				float2 rsh = uv * 0.70710638280;   // 0.70710638280 means 45 degree
				return 0.5 + 0.25 * cos((rsh.x + rsh.y) * d) + 0.25 * cos((rsh.x - rsh.y) * d);
			}
			half4 frag (v2f_img input) : SV_Target
			{
				float p = uvPattern(input.uv);
				half4 c = tex2D(_MainTex, input.uv);
				float avg  = 0.2125 * c.r + 0.7154 * c.g + 0.0721 * c.b;
				float gray = (p * _Intensity + avg - _Intensity) / (1.0 - _Intensity);
				return float4(gray.xxx, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}