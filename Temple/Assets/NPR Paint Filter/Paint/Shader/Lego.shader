Shader "NPR Paint Filter/Lego" {
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
			float4 _MainTex_TexelSize;
			float _Size, _GridShadow, _CircleSize;

			float4 frag (v2f_img input) : SV_Target
			{
				float c = _Size;
				float2 fragCoord = input.uv * _MainTex_TexelSize.zw;
				
				float2 middle = floor(fragCoord * c + 0.5) / c;
				
				float3 color = tex2D(_MainTex, middle / _MainTex_TexelSize.zw).rgb;
				
				float dis = abs(distance(fragCoord, middle) * c * _CircleSize - 0.6);
				color *= smoothstep(0.1, 0.05, dis) * dot(0.707, normalize(fragCoord - middle)) * 0.5 + 1.0;

				// side shadow
				float2 delta = abs(fragCoord - middle) * c * _GridShadow;
				float sdis = max(delta.x, delta.y);
				color *= 0.8 + smoothstep(0.95, 0.8, sdis) * 0.2;
				return float4(color, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}