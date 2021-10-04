Shader "NPR Paint Filter/Tiles" {
	Properties {
		[HideInInspector] _MainTex ("Main", 2D) = "white" {}
		_NumTiles ("Num Tiles", Range(1, 100)) = 10
		_Threshhold ("Threshhold", Range(0, 2)) = 0.15
		_EdgeColor ("Edge Color", Color) = (0.7, 0.7, 0.7, 1)
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _NumTiles, _Threshhold;
			float3 _EdgeColor;

			float4 frag (v2f_img input) : SV_Target
			{
				float sz = 1.0 / _NumTiles;
				float2 base = input.uv - fmod(input.uv, sz.xx);
				float2 center = base + (sz / 2.0).xx;
				float2 st = (input.uv - base) / sz;
				float4 c1 = 0.0;
				float4 c2 = 0.0;
				float4 io = float4((1.0 - _EdgeColor), 1.0);
				if (st.x > st.y) { c1 = io; }
				float threshholdB =  1.0 - _Threshhold;
				if (st.x > threshholdB)  { c2 = c1; }
				if (st.y > threshholdB)  { c2 = c1; }
				float4 bottom = c2;
				c1 = 0.0;
				c2 = 0.0;
				if (st.x > st.y)  { c1 = io; }
				if (st.x < _Threshhold)  { c2 = c1; }
				if (st.y < _Threshhold)  { c2 = c1; }
				return tex2D(_MainTex, center) + c2 - bottom;
			}
			ENDCG
		}
	}
	FallBack Off
}