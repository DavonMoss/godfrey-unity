Shader "NPR Paint Filter/Cmyk Halftone" {
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
			float4 _MainTex_TexelSize;
			float _Scale, _Angle, _Strength;

			float pattern (float angle, float2 uv)
			{
				float s = sin(angle), c = cos(angle);
				float2 tex = uv * _MainTex_TexelSize.zw - 0.5;
				float2 pt = float2(c * tex.x - s * tex.y, s * tex.x + c * tex.y) * _Scale;
				return (sin(pt.x) * sin(pt.y)) * _Strength;
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float4 c = tex2D(_MainTex, uv);
				float3 cmy = 1.0 - c.rgb;
				float k = min(cmy.x, min(cmy.y, cmy.z));
				cmy = (cmy - k) / (1.0 - k);
				cmy = saturate(cmy * 10.0 - 3.0 + float3(pattern(_Angle + 0.26, uv), pattern(_Angle + 1.31, uv), pattern(_Angle, uv)));
				k = saturate(k * 10.0 - 5.0 + pattern(_Angle + 0.79, uv));
				return float4(1.0 - cmy - k, c.a);
			}
			ENDCG
		}
	}
	FallBack Off
}