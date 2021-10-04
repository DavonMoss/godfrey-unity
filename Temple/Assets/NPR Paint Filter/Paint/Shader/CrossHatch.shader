Shader "NPR Paint Filter/CrossHatch" {
	Properties {
		[HideInInspector]_MainTex ("Main", 2D) = "white" {}
		_LineWidth ("LineWidth", Float) = 5.0
		_Resolution ("_Resolution", Vector) = (800, 800, 0, 0)
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Resolution;
			float _LineWidth;

			float mod (float x,float f) { return x - floor(x * (1.0 / f)) * f; }
			half sampleColorLumn (float2 uv, float dx, float dy, float2 step)
			{
				half4 c = tex2D(_MainTex, (uv + float2(dx, dy)) * step);
				return 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
			}
			float4 frag (v2f_img input) : SV_Target
			{
				float4 tex = tex2D(_MainTex, input.uv);
				float lumn = (0.2126 * tex.x) + (0.7152 * tex.y) + (0.0722 * tex.z);

				float minChannel = min(min(tex.r, tex.g), tex.b);
				float maxChannel = max(max(tex.r, tex.g), tex.b);
				float delta = maxChannel - minChannel;
				if (delta > 0.1)
					tex = tex * (1.0 / maxChannel);
				else
					tex.rgb = 1.0;

				float2 iuv = input.uv * _Resolution.xy;
				float dtA = iuv.x + iuv.y;
				float dtS = iuv.x - iuv.y;

				float3 res = 1.0;
				if (lumn < 0.8)
				{
					if (mod(dtA, 10.0) <= _LineWidth)
						res = float3(tex.rgb * 0.8);
				}
				if (lumn < 0.6)
				{
					if (mod(dtS, 10.0) <= _LineWidth)
						res = float3(tex.rgb * 0.6);
				}
				if (lumn < 0.3)
				{
					if (mod(dtA - 5.0, 10.0) <= _LineWidth)
						res = float3(tex.rgb * 0.3);
				}
				if (lumn < 0.15) 
				{
					if (mod(dtS - 5.0, 10.0) <= _LineWidth)
						res = 0.0;
				}

				float2 invRes = 1.0 / _Resolution.xy;
				float2 p = iuv;
				float gx = 0.0;
				float gy = 0.0;

				lumn = sampleColorLumn(p, -1.0, -1.0, invRes);
				gx += -lumn;
				gy += -lumn;

				lumn = sampleColorLumn(p, -1.0,  0.0, invRes);
				gx += -2.0 * lumn;
				gx += -lumn;

				lumn = sampleColorLumn(p,  1.0, -1.0, invRes);
				gx +=  lumn;
				gy += -lumn;

				gx +=  2.0 * sampleColorLumn(p,  1.0,  0.0, invRes);
				gx +=  sampleColorLumn(p,  1.0,  1.0, invRes);
				gy +=  sampleColorLumn(p,  1.0,  1.0, invRes);

				gy += -2.0 * sampleColorLumn(p,  0.0, -1.0, invRes);
				gy +=  sampleColorLumn(p, -1.0,  1.0, invRes);
				gy +=  2.0 * sampleColorLumn(p,  0.0,  1.0, invRes);

				float g = gx * gx + gy * gy;
				res *= (1.0 - g);
				return float4(res, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}