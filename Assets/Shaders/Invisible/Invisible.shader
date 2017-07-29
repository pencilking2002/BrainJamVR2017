// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Invisible/Invisible" {
	Properties {
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