Shader "NPR Paint Filter/Pencil" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_LineColor ("Line", Color) = (1, 1, 1, 1)
		_Strength ("Strength", Float) = 1
		_Offset ("Offset", Float) = 2
	}
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			fixed4 _LineColor;
			float _Strength, _Offset;

			fixed4 frag (v2f_img i) : SV_Target
			{
				half xr = _MainTex_TexelSize.x * _Offset;
				half yr = _MainTex_TexelSize.y * _Offset;
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 c0 = tex2D(_MainTex, i.uv + float2(xr, 0.0)).rgb;
				fixed3 c1 = tex2D(_MainTex, i.uv + float2(0.0, yr)).rgb;
				half f = 0.0;
				f += abs(c.r - c0.r);
				f += abs(c.g - c0.g);
				f += abs(c.b - c0.b);
				f += abs(c.r - c1.r);
				f += abs(c.g - c1.g);
				f += abs(c.b - c1.b);
				f -= _Strength;
				f = saturate(f);
				c.rgb = (1.0 - f) + _LineColor.rgb * f;
				return c;
			}
			ENDCG
		}
	}
	Fallback Off
}