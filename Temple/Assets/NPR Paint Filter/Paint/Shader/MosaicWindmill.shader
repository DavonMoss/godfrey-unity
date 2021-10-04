Shader "NPR Paint Filter/MosaicWindmill" {
	Properties {
		_MainTex     ("Main", 2D) = "white" {}
		_PixelSize   ("Pixel Scale", Range(0.01, 1.0)) = 0.8
		_PixelRatio  ("Pixel Ratio", Range(0.2, 5.0)) = 1
		_PixelScaleX ("Pixel Scale X", Range(0.2, 5.0)) = 1
		_PixelScaleY ("Pixel Scale Y", Range(0.2, 5.0)) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			half _PixelSize, _PixelRatio, _PixelScaleX, _PixelScaleY;

			float2 CalcUV (float2 uv)
			{
				float2 pixelScale = _PixelSize * float2(_PixelScaleX, _PixelScaleY / _PixelRatio);

				float2 coord = floor(uv * pixelScale) / pixelScale;

				uv -= coord;
				uv *= pixelScale;

				coord += float2(step(1.0 - uv.y, uv.x) / (pixelScale.x), step(uv.x, uv.y) / (pixelScale.y));
				return coord;
			}
			half4 frag (v2f_img input) : SV_TARGET
			{
				float2 uv = CalcUV(input.uv);
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
	Fallback Off
}
