Shader "NPR Paint Filter/MosaicDiamond" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_PixelSize ("Pixel Size", Float) = 0.2
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float _PixelSize;

			float4 frag (v2f_img input) : SV_Target
			{
				half2 uv = input.uv;
				half2 pixelSize = 10.0 / _PixelSize;
				half2 coord = uv * pixelSize;
				int direction = int(dot(frac(coord), half2(1, 1)) >= 1.0) + 2 * int(dot(frac(coord), half2(1, -1)) >= 0.0);
				coord = floor(coord);

				if (direction == 0) coord += half2(0, 0.5);
				if (direction == 1) coord += half2(0.5, 1);
				if (direction == 2) coord += half2(0.5, 0);
				if (direction == 3) coord += half2(1, 0.5);

				coord /= pixelSize;
				return tex2D(_MainTex, coord);
			}
			ENDCG
		}
	}
	FallBack Off
}