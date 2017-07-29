// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Invisible/Invisible Rim Texture Lit Fog" {
	Properties {
		_MainTex ("Main Texture", 2d) = "white" {}
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,6.0)) = 1.5
	}
	SubShader {
		Pass {
		Tags { "RenderType"="Geometry" "LightMode"="ForwardBase"}
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag

			
			struct vertexInput {
				float4 vertex : POSITION;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				//Set position
				o.pos = UnityObjectToClipPos(float4(0,0,0,0));
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				return float4(0,0,0,0);
			}
			ENDCG
		}
	}
}