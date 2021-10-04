Shader "NPR Paint Filter/Ascii" {
	Properties {
		[HideInInspector]_MainTex  ("Texture", 2D) = "white" {}
		[NoScaleOffset]_TilesTex   ("Tiles", 2DArray) = "" {}
		_TileArraySize ("Tiles Array Size", int) = 0
		_TilesX        ("Tiles X", Int) = 0
		_TilesY        ("Tiles Y", Int) = 0
		_Blend         ("Blend", Range(0, 1)) = 1
		_Brightness    ("Character Brightness", Range(0, 10)) = 1.17
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			int _TilesX, _TilesY, _TileArraySize;
			float _Brightness, _Blend;
			UNITY_DECLARE_TEX2DARRAY(_TilesTex);

			float3 brighten (float x)
			{
				x = saturate(x);
				return 1.0 - (pow((1.0 - x), (_Brightness + 1.0)));
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				half4 tc = tex2D(_MainTex, uv);
				half lum = Luminance(tc);
				
				float3 uvz;
				uvz.x = uv.x * _TilesX;
				uvz.y = uv.y * _TilesY;
				uvz.z = brighten(lum) * _TileArraySize;
				
				half4 ret = tc;
				ret.rgb = UNITY_SAMPLE_TEX2DARRAY(_TilesTex, uvz) * tc.rgb;
				ret = lerp(tc, ret, _Blend);
				return ret;
			}
			ENDCG
		}
	}
	FallBack Off
}