// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "RenderFX/Skybox Cubed Rotated" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	_Tex ("Cubemap", Cube) = "white" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }

	Pass {
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members viewDir)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		samplerCUBE _Tex;
		fixed4 _Tint;
		uniform float4x4 _Rotation;
		
		struct appdata_t {
			float4 vertex : POSITION;
			float3 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
			float3 viewDir;
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = v.texcoord;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
//			fixed4 tex = texCUBE (_Tex, i.texcoord);
			float4 emptyFloat = float4(1,1,1,1);
			float4 ViewDirection=float4( i.viewDir.x, i.viewDir.y,i.viewDir.z,0 ); 
			float4 viewInvert=float4(float4( ViewDirection.x, ViewDirection.x, ViewDirection.x, ViewDirection.x).x, float4( ViewDirection.z, ViewDirection.z, ViewDirection.z, ViewDirection.z).y, float4( ViewDirection.y, ViewDirection.y, ViewDirection.y, ViewDirection.y).z, 0);
			fixed4 tex=texCUBE(_Tex,mul(_Rotation,viewInvert)); 
			fixed4 col;
			col.rgb = tex.rgb + _Tint.rgb - unity_ColorSpaceGrey;
			col.a = tex.a * _Tint.a;
			return col;
		}
		ENDCG 
	}
} 	


Fallback Off

}
