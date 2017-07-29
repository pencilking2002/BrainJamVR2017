// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/UI/Crosshair" {
Properties {	
	_Brightness("Brightness",Range(0,2)) = 1
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Main Tex 1", 2D) = "white" {}
	_Color2 ("Main Color", Color) = (1,1,1,1)
	_MainTex2 ("Main Tex 2", 2D) = "white" {}
	_Color3 ("Main Color", Color) = (1,1,1,1)
	_MainTex3 ("Main Tex 3", 2D) = "white" {}
	_MaskTex ("Mask", 2D) = "white" {}
	
	_TilingMultiplierX("Tiling Multiplier", Float) = 1
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

			half _Brightness;

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color2;
			sampler2D _MainTex2;
			float4 _MainTex2_ST;

			fixed4 _Color3;
			sampler2D _MainTex3;
			float4 _MainTex3_ST;

			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			
			half _TilingMultiplierX;
			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				half2 mainTexMultiplier = half2(_TilingMultiplierX,1);
				
				half2 coords = _MainTex_ST.xy*i.texcoord.xy*mainTexMultiplier + _MainTex_ST.zw*_Time.y;
				fixed4 mainTex = tex2D(_MainTex, coords);
				
				coords = _MainTex2_ST.xy*i.texcoord.xy*mainTexMultiplier + _MainTex2_ST.zw*_Time.y;
				fixed4 mainTex2 = tex2D(_MainTex2, coords);
				
				coords = _MainTex3_ST.xy*i.texcoord.xy*mainTexMultiplier + _MainTex3_ST.zw*_Time.y;
				fixed4 mainTex3 = tex2D(_MainTex3, coords);
				
				coords = _MaskTex_ST.xy*i.texcoord.xy + _MaskTex_ST.zw;
				fixed4 maskTex = tex2D(_MaskTex, coords);
				
				fixed4 col = (mainTex+mainTex2+mainTex3)/3;
//				fixed4 col = lerp(lerp(mainTex,mainTex2,.5),mainTex3,.5);
				
				col*=_Brightness;
				col*= lerp(_Color,_Color2,i.texcoord.x);
				
				col.a *= maskTex.b;
				
				return col;
			}
		ENDCG
	}
}

}
