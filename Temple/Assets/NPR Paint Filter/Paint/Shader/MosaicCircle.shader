Shader "NPR Paint Filter/MosaicCircle" {
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
			float4 _BackgroundColor, _Params;

			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float scale = 1.0 / _Params.x;
				
				float ratio = _ScreenParams.y / _ScreenParams.x;
				uv.x = uv.x / ratio;

				float interval = _Params.y;
				float2 coord = half2(interval *  floor(uv.x / (scale * interval)), interval * floor(uv.y / (scale * interval)));

				float2 circleCenter = coord * scale + scale * 0.5;
				float dist = length(uv - circleCenter) * _Params.x;
				circleCenter.x *= ratio;
				float4 c = tex2D(_MainTex, circleCenter);

				if (dist > _Params.z)
					c = _BackgroundColor;
				return c;
			}
			ENDCG
		}
	}
	FallBack Off
}