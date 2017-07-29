// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Rim/Test" {
Properties {
    _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    _EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
//	_EmissionTex ("Base (RGB)", 2D) = "white" {}
	_BRDFTex ("Base (RGB)", 2D) = "white" {}
//	_RampTex ("BRDF Ramp", 2D) = "gray" {}
    _RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    _RimAngle ("Rim Angle", Range(0.5,8.0)) = 2.0
    _RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.0
}
	SubShader {
		Pass {
	Tags { "RenderType"="Transparent" "Queue"="Transparent" }
			Tags { "LightMode" = "ForwardBase" }
			
         Blend SrcAlpha OneMinusSrcAlpha
         ZWrite Off // don't write to depth buffer 
			CGPROGRAM
//			#pragma noambient noforwardadd 
			#pragma alpha	
			#pragma vertex vert
			#pragma fragment frag
			
			//user
			sampler2D _EmissionTex;
			float4 _EmissionText_ST;
			sampler2D _BRDFTex;
			float4 _BRDFTex_ST;
			sampler2D _RampTex;
			float4 _RampTex_ST;
samplerCUBE _Cube;


fixed _EmissionIntensity;
fixed4 _EmissionColor;
fixed4 _RimColor;
half _RimAngle;
half _RimIntensity;
    
//			uniform float4 _Color;
			
			//unity
			uniform float4 _LightColor0;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				o.texcoord=v.tex;
				
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
				float3 lightDirection;
				float atten =1.0;
				
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				_LightColor0.a=0.0;
				
				float3 diffuseColor=atten*max(0.0,pow(dot(normalDirection,lightDirection),1))+UNITY_LIGHTMODEL_AMBIENT*atten;
				
    			half4 rimAngle = 1.0 - saturate(dot (normalize(viewDirection), v.normal));
				rimAngle=pow(rimAngle,_RimAngle);
				
				float3 rimColor=_RimIntensity*diffuseColor*rimAngle*_RimColor.xyzw;
				float3 specularColor=pow(dot(normalDirection,lightDirection),12);
				
				
				fixed NdotL = dot(v.normal, viewDirection);
				fixed NdotE = dot(v.normal, lightDirection);
			
				fixed diff = (NdotL * 0.3) +0.5;
				fixed2 brdfUV = float2(NdotE * .8,diff);
//				fixed3 BRDF =brdfTex.rgb;
			
				float4 emissionFinal=_EmissionColor*_EmissionIntensity;
				float4 rimFinal=_RimColor*float4(rimColor,rimColor.b);
				float4 specularFinal=float4(specularColor,specularColor.r);
				
				o.col =emissionFinal+saturate(specularFinal+rimFinal);
				o.pos = UnityObjectToClipPos(v.vertex);
//				o.col.a+=2;
//				o.col=rimAngle;
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
    
				return o.col;//*brdfTex;
			}
			ENDCG
		}
	}
}