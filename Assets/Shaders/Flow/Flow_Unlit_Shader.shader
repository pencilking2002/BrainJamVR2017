// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Flow/Unlit" {
	Properties {
    	_Color ("Color", Color) = (1,1,1,1)
    	_BumpIntensity ("Bump Intensity",Range(0.0,1.0)) = 0.35
		_MainTex ("Main Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
    	_ParticleColor1 ("Particle Color 1", Color) = (1,1,1,1)
		_ParticleTex1 ("Particle Texture 1", 2D) = "white" {}
		_FlowTex1 ("Flow Texture 1", 2D) = "white" {}
    	_ParticleColor2 ("Particle Color 2", Color) = (1,1,1,1)
		_ParticleTex2 ("Particle Texture 2", 2D) = "white" {}
		_FlowTex2 ("Flow Texture 2", 2D) = "white" {}
//    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
//    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
//    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
//    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
//    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.5
//    	_SelectedColor ("Selected Color", Color) = (0,1.0,1.0,1.0)
	}
	SubShader {
		Tags { 
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}
			 
		LOD 200
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag
			
			
			//uniform
			uniform fixed4 _Color;
			uniform half _BumpIntensity;
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform sampler2D _MaskTex;
			
			uniform fixed4 _ParticleColor1;
			uniform sampler2D _ParticleTex1;
			uniform sampler2D _FlowTex1;
			
			uniform fixed4 _ParticleColor2;
			uniform sampler2D _ParticleTex2;
			uniform sampler2D _FlowTex2;
			
			
			
//			uniform fixed4 _EmissionColor;
//			uniform fixed _EmissionIntensity;
//			uniform fixed4 _RimColor;
//			uniform half _RimAngle;
//			uniform half _RimIntensity;
//			uniform fixed4 _SelectedColor;
			
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
				
				//Set position
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				//Emissive tex
//				half4 mainTex = tex2D(_MainTex,_MainTex_ST.xy*o.texcoord.xy + _MainTex_ST.zw);
    			
    			float t = _Time.x;
    			float2 texCoords = o.texcoord.xy;
//    			texCoords.x-=t*2.;

				//MainImage
//    			float bump = tex2D(_ParticleTex2,texCoords*0.5).r; 
    			float4 img = tex2D(_MainTex,float2(texCoords.x,texCoords.y));
    			
    			//Plasma
				texCoords = o.texcoord.xy;
//				texCoords.x-=0.05;
//				texCoords.y+= 0.05;
				
				//Plasma1
				float2 flow = tex2D(_FlowTex1,texCoords).gr;
				flow.y+=t;
				flow.y*=2.;
				float4 plasma = tex2D(_ParticleTex1,flow*1.5);
				plasma.rgb *= _ParticleColor1;
				plasma *= 3.;
				
				//Plasma2
				flow = tex2D(_FlowTex2,texCoords).rg;
				flow.g+=t;
				float4 plasma2 = tex2D(_ParticleTex2,flow);
				plasma2 *= 4.;
				plasma2.rgb *= _ParticleColor2;
				
				//Final
				float4 finalColor = plasma+plasma2;
//				finalColor+= (sin(t*15.)+1.) * mask.a;
//				finalColor.a= min(mask.a,finalColor.b);
    			float4 mask = tex2D(_MaskTex,texCoords);
				finalColor.a=mask.r;
				finalColor*=_Color;
				
				
				
				
				
				return finalColor;
			}
			ENDCG
		}
	}
}