Shader "NPR Paint Filter/LineDrawing" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
		_Steps  ("Steps", Range(0.1, 4)) = 1
		_Offset ("Offset", Range(0, 16)) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half _Steps, _Offset;

			half4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float4 c = tex2D(_MainTex, uv);
				float g = max(c.r, max(c.g, c.b)) * _Steps;
				float f = fmod((uv.x + uv.y + 500.0) * (345.678 + _Offset), 1.0);
				if (fmod(g, 1.0) > f)
					c.r = ceil(g);
				else
					c.r = floor(g);
				c.r /= _Steps;
				return half4(c.rrr, 1.0);
			}
			ENDCG
		}
	}
	Fallback Off
}
