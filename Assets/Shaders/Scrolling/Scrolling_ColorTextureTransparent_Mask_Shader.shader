// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Scrolling/Color Texture Transparent Mask" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Color", 2D) = "white" {}
//	_Color2 ("Main Color", Color) = (1,1,1,1)
	_MaskTex ("Mask", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
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

			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			
			
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
				
				coords = _MaskTex_ST.xy*i.texcoord.xy + _MaskTex_ST.zw*_Time.y;
				fixed4 maskTex = tex2D(_MaskTex, coords);
				
				fixed4 col = mainTex;
				col.a *= maskTex.b;
				
				return col;
			}
		ENDCG
	}
}

}
