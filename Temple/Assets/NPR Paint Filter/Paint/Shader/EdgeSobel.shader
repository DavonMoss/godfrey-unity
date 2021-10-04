Shader "NPR Paint Filter/EdgeSobel" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
		_Resolution ("Resolution", Vector) = (800, 800, 0, 0)
		_LineColor  ("Line Color", Color) = (1, 1, 1, 1)
		_BgColor    ("Background Color", Color) = (0, 0, 0, 1)
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			half4 _Resolution, _LineColor, _BgColor;

			float intensity (float4 c) { return sqrt((c.r * c.r) + (c.g * c.g) + (c.b * c.b)); }
			float3 sobel (float stepx, float stepy, float2 center)
			{
				float tleft  = intensity(tex2D(_MainTex, center + float2(-stepx, stepy)));
				float left   = intensity(tex2D(_MainTex, center + float2(-stepx, 0)));
				float bleft  = intensity(tex2D(_MainTex, center + float2(-stepx, -stepy)));
				float top    = intensity(tex2D(_MainTex, center + float2(0, stepy)));
				float bottom = intensity(tex2D(_MainTex, center + float2(0, -stepy)));
				float tright = intensity(tex2D(_MainTex, center + float2(stepx, stepy)));
				float right  = intensity(tex2D(_MainTex, center + float2(stepx, 0)));
				float bright = intensity(tex2D(_MainTex, center + float2(stepx, -stepy)));

				float x =  tleft + 2.0 * left + bleft  - tright - 2.0 * right  - bright;
				float y = -tleft - 2.0 * top  - tright + bleft  + 2.0 * bottom + bright;
				float edge = sqrt((x * x) + (y * y));
				return lerp(_BgColor.rgb, _LineColor.rgb, edge);
			}
			half4 frag (v2f_img input) : SV_Target
			{
				return float4(sobel(1.0 / _Resolution.x, 1.0 / _Resolution.y, input.uv), 1.0);
			}
			ENDCG
		}
	}
	Fallback Off
}
