// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cockpit/Baked Brightness" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Baked Lighting", 2D) = "white" {}
	_Brightness ("Brightness", Range(0,1.0))=1.0
//	_GITex ("GI Strength", 2D) = "white" {}
}

SubShader {
	Tags {"RenderType"="Geometry"}
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GITex;
			float4 _GITex_ST;
			float4 _Color;
			fixed _Brightness;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.texcoord);
//				half4 colGI = tex2D(_GITex, i.texcoord); 
				float value = col.r;
				fixed4 colorFinal = float4(value,value,value,value);
				colorFinal =lerp(col,_Color,((col.a)));
				
				
//				col = float4(value,value,value,value);
				
				return colorFinal*_Brightness;
			}
		ENDCG
	}
}

}
