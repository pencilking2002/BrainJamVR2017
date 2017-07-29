// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	Shader "Custom/Sun/Ring" {
	Properties {
    	_Color ("Color", Color) = (0.8,0.95,1.0,1.0)
		_MainTex ("Sun Overlay (RGB) Mask Fire (A)",2D) = "white" {}
		_FireTex1 ("Fire Texture 1",2D) = "white" {}
		_FireTex2 ("Fire Texture 2",2D) = "white" {}
		_FireTex3 ("Fire Texture 3",2D) = "white" {}
		_Factor ( "Overall Brightness",Float)=1.0
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
			
			//user
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform sampler2D _FireTex1;
			uniform float4 _FireTex1_ST;
			
			uniform sampler2D _FireTex2;
			uniform float4 _FireTex2_ST;
			
			uniform sampler2D _FireTex3;
			uniform float4 _FireTex3_ST;
			
			
			uniform float _Factor;
			
			uniform fixed4 _Color;
			
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
				float4 _MainTiling1 = float4 (30,1,-0.05,0);
				float4 _MainTiling2 = float4 (50,1,0.08,0);
			
				half4 mainTex = tex2D(_MainTex,_MainTiling1.xy*o.texcoord.xy + float2(_MainTiling1.z*_Time.y,_MainTiling1.w));
				half4 mainTex2 = tex2D(_MainTex,_MainTiling2.xy*o.texcoord.xy + float2(_MainTiling2.z*_Time.y,_MainTiling2.w));
				float mainTexAlpha=mainTex.a;
				
				mainTex = ( mainTex2 + mainTex ) * 0.5;
				float4 edge = mainTex * mainTex;
				
				half4 fireTex1 = tex2D (_FireTex1, _FireTex1_ST.xy*o.texcoord.xy + _Time.y * _FireTex1_ST.zw ); 
				half4 fireTex2 = tex2D (_FireTex2, _FireTex2_ST.xy*o.texcoord.xy + _Time.y * _FireTex2_ST.zw ); 
				half4 fireTex3 = tex2D (_FireTex3, _FireTex3_ST.xy*o.texcoord.xy + _Time.y * _FireTex3_ST.zw );
					
				mainTex *= fireTex1 + fireTex2 + fireTex3 * 0.3;
				mainTex+=edge;
				mainTex.xyz*=_Factor;
				
				mainTex.a=saturate(mainTex.a);
				
				return mainTex*_Color;
			}
			ENDCG
		}
	}
}