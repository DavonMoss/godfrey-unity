Shader "NPR Paint Filter/Knitwear" {
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
			sampler2D _MainTex, _KnitwearTex;
			float _KnitwearShear, _KnitwearDivision, _KnitwearAspect;

			void KnitwearCoordinate(inout float2 uv, out float2 cell, half shear)
			{
				float offset = distance(frac(uv.x), 0.5) * shear;
				uv.y += offset;

				cell = floor(uv * float2(2.0, 1.0));
				cell += float2(0.5, 0.5);
				cell *= float2(0.5, 1.0);
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float2 uvNew = uv;

				float2 scale = _KnitwearDivision / float2(_KnitwearAspect, 1.0);
				uv *= scale;

				KnitwearCoordinate(uv, uvNew, _KnitwearShear);
				uvNew /= scale;

				float4 c = tex2D(_MainTex, uvNew);
				float4 kc = tex2D(_KnitwearTex, uv) * 1.2;
				return c * kc;
			}
			ENDCG
		}
	}
	FallBack Off
}