Shader "NPR Paint Filter/MosaicTriangles" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_TileNum ("TileNum", Vector) = (40, 20, 0, 0)
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float2 _TileNum;

			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float2 uv2 = floor(uv * _TileNum) / _TileNum;
				uv -= uv2;
				uv *= _TileNum;
				return tex2D(_MainTex, uv2 + float2(step(1.0 - uv.y, uv.x) / (2.0 * _TileNum.x),
													step(uv.x, uv.y) / (2.0 * _TileNum.y)));
			}
			ENDCG
		}
	}
	FallBack Off
}