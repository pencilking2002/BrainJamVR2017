// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/Rim/Reflective" {
	Properties {
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.5
    	_ReflectColor ("Reflection Color", Color) = (0.5,0.5,0.5,1.0)
		_ReflectCube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	SubShader {
		Pass {
		Tags { "RenderType"="Geometry"}
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag
			
			/////Uniform
			//Emission
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			//Rim
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			//Reflect
			uniform fixed4 _ReflectColor;
			uniform samplerCUBE _ReflectCube;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float3 I : TEXCOORD1;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				//Set position
				o.pos = UnityObjectToClipPos(v.vertex);
				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
//				o.I = -viewDirection;
				
				//Calculate reflective color
				float3 worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
				o.I = reflect( -viewDirection, worldN );
				
//				o.I = -viewDirection;
//				fixed4 reflAngle = texCUBE (_ReflectCube, IN.worldRefl);
//				fixed4 reflColor = _ReflectColor*reflAngle;
	
				//Calculate emission color
				half3 emissionColor=_EmissionColor*_EmissionIntensity;
				
				//Calculate rim color
    			half3 rimAngleColor = 1.0 - saturate(dot (normalize(viewDirection), normalDirection));
				rimAngleColor=pow(rimAngleColor,_RimAngle);
				half3 rimColor=_RimColor*_RimIntensity*rimAngleColor*_RimColor.xyzw;
				
				//Calculate final color
				o.col =float4(emissionColor+saturate(rimColor),1.0);
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				fixed4 reflcol = texCUBE( _ReflectCube, o.I );
				return o.col + (reflcol * _ReflectColor);
			}
			ENDCG
		}
	}
}