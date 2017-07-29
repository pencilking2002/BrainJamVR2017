// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Cell/Cytoplasm/Membrane" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Main Tex", 2D) = "white" {}
	_MainTex2 ("Main Tex 2", 2D) = "white" {}
	_MaskTex ("Mask", 2D) = "white" {}
	_Bias ("Texture Bias", Range(0.0,1.0)) = 0.5
}

SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode"="ForwardBase"}
	
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
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MainTex2;
			float4 _MainTex2_ST;
			sampler2D _MaskTex;
			float4 _Color;
			fixed _Bias;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex1 = tex2D(_MainTex,_MainTex_ST.xy*i.texcoord.xy + float2(_MainTex_ST.z,_MainTex_ST.w)*_Time.y);
				fixed4 tex2 = tex2D(_MainTex2,_MainTex2_ST.xy*i.texcoord.xy + float2(_MainTex2_ST.z,_MainTex2_ST.w));
				
				fixed4 tex = lerp(tex1,tex2,_Bias);
				
				tex*=_Color;
				
				fixed4 mask = tex2D(_MaskTex,i.texcoord.xy );
				
				tex.a = mask.r;
				
				fixed4 fogColor = tex;
				UNITY_APPLY_FOG(i.fogCoord, fogColor);
				
				return lerp(tex,fogColor,.5);
			}
		ENDCG
	}
}

}
