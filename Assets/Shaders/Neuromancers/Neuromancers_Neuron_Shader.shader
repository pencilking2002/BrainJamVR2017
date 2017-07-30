// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Neuromancers/Neuron" {
	Properties {
    	_Color ("Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.75
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = .5

		_OffsetTex ("Vertex Offset Texture", 2d) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque"
			"LightMode"="ForwardBase"}
		Pass {
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag
			
			//uniform
			uniform fixed4 _Color;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;

			//tex
			uniform sampler2D _OffsetTex;
			uniform float4 _OffsetTex_ST;
			
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
				//Set position
				o.texcoord=v.tex;

				half4 offsetTex = tex2Dlod(_OffsetTex,float4(_OffsetTex_ST.xy*v.tex.xy,0.0,0.0));
				float4 localPos = v.vertex * (.8 + .2*offsetTex * abs(_SinTime.z));
				o.pos = UnityObjectToClipPos(localPos);


				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,localPos).xyz);
				half3 lightDirection= normalize(_WorldSpaceLightPos0.xyz);
				
				//Calculate diffuse color
				half3 diffuseColor=max(0.0,0.6+dot(normalDirection,lightDirection));
		
				//Calculate emission color
				half3 emissionColor=_Color*_EmissionIntensity;
				
				//Calculate rim color
    			half3 rimAngleColor = 1.0 - saturate(dot (normalize(viewDirection), normalDirection));
				rimAngleColor=pow(rimAngleColor,_RimAngle);
				half3 rimColor=_RimColor*_RimIntensity*rimAngleColor*_RimColor.xyzw;
				
				//Calculate final color
				o.col =float4(emissionColor+saturate(diffuseColor*rimColor),1.0);


				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				return o.col;
			}
			ENDCG
		}
	}
}