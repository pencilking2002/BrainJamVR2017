// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Museum/Environment Blood Vessel" {
	Properties {
		_MainTex ("Sun Overlay (RGB) Mask Fire (A)",2D) = "white" {}
		_FlowTex ("Flow Texture",2D) = "white" {}
		_DetailTex ( "Detail Texture",2D) = "white" {}
		_FlowSpeed ( "Flow Speed",Float)=1.0
		_FireTex1 ("Fire Texture 1",2D) = "white" {}
		_FireTex2 ("Fire Texture 2",2D) = "white" {}
		_Factor ( "Overall Brightness",Float)=1.0
	}
	SubShader {
		Pass {
			Tags { "Queue" = "Geometry" }
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag
			
			//user
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform sampler2D _FlowTex;
			uniform float _FlowSpeed;
			
			uniform sampler2D _FireTex1;
			uniform float4 _FireTex1_ST;
			
			uniform sampler2D _FireTex2;
			uniform float4 _FireTex2_ST;
			
			
			uniform sampler2D _DetailTex;
			uniform float4 _DetailTex_ST;
			
			uniform float _Factor;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				o.texcoord=v.tex;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				half4 mainTex = tex2D(_MainTex,_MainTex_ST.xy*o.texcoord.xy + _MainTex_ST.zw);
				
				half4 flowTex = tex2D(_FlowTex,_MainTex_ST.xy*o.texcoord.xy + _MainTex_ST.zw);
				flowTex.xy = flowTex.xy * 2.0 - 1.0;
				flowTex.y *= -1.0;
				
				half4 detailTex = tex2D (_DetailTex, _DetailTex_ST.xy*o.texcoord.xy + _DetailTex_ST.zw);
				
				mainTex = (mainTex*detailTex);
				
				float scaledTime = _Time.y * _FlowSpeed + flowTex.z;
   
				float flowA = frac( scaledTime );
				float flowBlendA = 1.0 - abs( flowA * 2.0 - 1.0 );
				flowA -= 0.5;
   
				float flowB = frac( scaledTime + 0.5 );
				float flowBlendB = 1.0 - abs( flowB * 2.0 - 1.0 );
				flowB -= 0.5;
				
				half4 fireTex1 = tex2D (_FireTex1, o.texcoord.xy * _FireTex1_ST.xy + _Time.y * _FireTex1_ST.zw + ( flowTex.xy * flowA * 1 ) );
				half4 fireTex2 = tex2D (_FireTex2, o.texcoord.xy * _FireTex2_ST.xy + _Time.y * _FireTex2_ST.zw + ( flowTex.xy * flowB * 1 ) );

				half4 finalFire = lerp( fireTex1, fireTex2, flowBlendB );
				finalFire = lerp( mainTex.x, finalFire, mainTex.w );
				
				half4 Final = lerp(mainTex, finalFire,.8)*mainTex * _Factor;
				
				
				
				return Final;
			}
			ENDCG
		}
	}
}