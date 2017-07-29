// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Cockpit/Shell" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Visible ("Visible",Range(0,1)) = 0.5
	_MainTex ("Base (RGB)", 2D) = "white" {}
//	_MaskTex ("Mask (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma alpha:blend
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
//			sampler2D _MaskTex;
//			float4 _MaskTex_ST;
			float4 _Color;
			fixed _Visible;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float4 col = _Color*tex2D(_MainTex,i.texcoord);
				
				float visibleAlpha=80*(_Visible-i.texcoord.y);
				visibleAlpha=clamp(visibleAlpha,0,1);
				
//				col.rgb*=pow((1.0-i.texcoord.y),2);
				col.a*=visibleAlpha;
				
				
				return col;
			}
		ENDCG
	}
}

}
