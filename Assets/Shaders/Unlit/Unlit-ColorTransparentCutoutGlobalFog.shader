// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent Cutout Color Global Fog" {
Properties {
	_Color("Color",color) = (1,1,1,1)
	_MainTex ("Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Distance("Distance",  Range(0,10000)) = 200
	_ReflectCube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
}
SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100

	Lighting Off

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
				float cameraDistance : TEXCOORD2;
				half2 texcoord : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
				float3 I : TEXCOORD1;
			};

//			sampler2D _MainTex;
//			float4 _MainTex_ST;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			fixed4 _Color;
			
			uniform samplerCUBE _ReflectCube;
		   	uniform  float _Distance;
	    
			float lengthFloatThree(float3 v)
			{
  				return sqrt(dot(v,v));
			}
		    
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
//				UNITY_TRANSFER_FOG(o,o.vertex);
				
				float3 viewDirection = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz;//not normalized
				o.cameraDistance = lengthFloatThree(viewDirection);
				o.I = -normalize(viewDirection);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col =_Color*tex2D(_MainTex, i.texcoord);
				clip(tex2D(_MainTex, i.texcoord).a - _Cutoff);
				
				
				fixed4 reflcol = texCUBE( _ReflectCube, i.I );
//				UNITY_APPLY_FOG(i.fogCoord, col);
//				fixed fogStrength;
        // Compute fog amount.
//        return reflcol;
//		        float g = ComputeDistance(i.cameraDistance) - 0;
//		        half fogStrength = ComputeFogFactor(max(0.0, i.cameraDistance));
				float objectDistance = abs(i.cameraDistance);
				objectDistance = pow(objectDistance,2.0);
				half fogStrength = clamp(objectDistance/(_Distance*800.0),0,1);
//				return reflcol;
				return lerp(col,reflcol,fogStrength);
				
			}
		ENDCG
	}
}

}