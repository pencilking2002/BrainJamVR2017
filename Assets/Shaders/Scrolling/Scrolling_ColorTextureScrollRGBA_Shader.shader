// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Scrolling/Color Texture Scroll RGBA" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color2 ("Main Color", Color) = (1,1,1,1)
	_ScrollTex ("Scroll R, Scroll G, Scroll B", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color2;
			sampler2D _ScrollTex;
			float4 _ScrollTex_ST;

			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 coords = _MainTex_ST.xy*i.texcoord.xy + _MainTex_ST.zw*_Time.y;
				fixed4 mainTex = _Color*tex2D(_MainTex, coords);

				//Normal for Red channel
				coords = _ScrollTex_ST.xy*i.texcoord.xy + float2(_ScrollTex_ST.z,_ScrollTex_ST.w)*_Time.y;
				fixed scrollTexR = _Color2*tex2D(_ScrollTex, coords).r;

				//Invert X for Green channel
				coords = _ScrollTex_ST.xy*i.texcoord.xy + float2(-_ScrollTex_ST.z*.85,_ScrollTex_ST.w*1.1)*_Time.y;
				fixed scrollTexG = _Color2*tex2D(_ScrollTex, coords).g;

				//Invert Y for Blue channel
				coords = _ScrollTex_ST.xy*i.texcoord.xy + float2(_ScrollTex_ST.z*1.15,-_ScrollTex_ST.w*.7)*_Time.y;
				fixed scrollTexB = _Color2*tex2D(_ScrollTex, coords).b;

				//Invert X&Y for Alpha channel
				coords = _ScrollTex_ST.xy*i.texcoord.xy + float2(-_ScrollTex_ST.z*.9,-_ScrollTex_ST.w*.8)*_Time.y;
				fixed scrollTexA = _Color2*tex2D(_ScrollTex, coords).a;




				fixed4 col = (mainTex+scrollTexR+scrollTexG+scrollTexB+scrollTexA)/5;
				
				return col;
			}
		ENDCG
	}
}

}
