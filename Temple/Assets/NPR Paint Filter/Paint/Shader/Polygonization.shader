Shader "NPR Paint Filter/Polygonization" {
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
			float _Strength, _Size, _Blur;

			float2 hash2 (float2 p)
			{
				return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
			}
			float2 voronoi (float2 x)
			{
				float2 n = floor(x);
				float2 f = frac(x);

				float2 mr = 0;
				float md = _Strength;
				for (int j = -1; j <= 1; j++)
				{
					for (int i = -1; i <= 1; i++)
					{
						float2 g = float2(float(i), float(j));
						float2 o = hash2(n + g);
						float2 r = g + o - f;
						float d = dot(r, r);
						if (d < md)
						{
							md = d;
							mr = r;
						}
					}
				}
				return mr;
			}
			float3 voronoiColor (float steps, float2 uv)
			{
				float2 c = voronoi(steps * uv);
				float2 uv1 = uv;
				uv1.x += c.x / steps;
				uv1.y += c.y / steps * _ScreenParams.x / _ScreenParams.y;
				return tex2D(_MainTex, uv1).xyz;
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float2 uv = input.uv;
				float3 c = 0.0;
				for (int i = 0; i < 4; i++)
				{
					float steps = _Size * pow(_Blur, i);
					c += voronoiColor(steps, uv);
				}
				return float4(c * 0.25, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}