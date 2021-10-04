Shader "NPR Paint Filter/Dot Drawing" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float _DotSize, _Darkness;

			#define PI  3.1415926536
			#define PI2 (PI * 2.0)

			half2 random (half2 p)
			{
				p = frac(p * (half2(314.159, 314.265)));
				p += dot(p, p.yx + 17.17);
				return frac((p.xx + p.yx) * p.xy);
			}
			half3 colorDodge (half3 src, half3 dst)
			{
				return step(0.0, dst) * lerp(min(1.0, dst/ (1.0 - src)), 1.0, step(1.0, src));
			}
			float greyScale (half3 c) { return dot(c, half3(0.3, 0.59, 0.11)); }
			half4 frag (v2f_img input) : SV_Target
			{
				half2 uv = input.uv;
				half3 col = tex2D(_MainTex, uv).rgb;

				half2 r = random(uv);
				r.x *= PI2;
				half2 cr = half2(sin(r.x), cos(r.x)) * sqrt(r.y);

				half3 blurred = tex2D(_MainTex, uv + cr * (_DotSize / _ScreenParams.xy)).rgb;
				half3 inv = 1.0 - blurred;

				half3 lighten = colorDodge(col, inv);
				half3 res = greyScale(lighten);
				res = pow(res.x, _Darkness);
				return half4(res, 1.0); 
			}
			ENDCG
		}
	}
	FallBack Off
}