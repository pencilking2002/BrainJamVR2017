// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Circulatory/ErythrocyteVert" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Emission", 2D) = "white" {}
//		_BumpTex ("Normal", 2D) = "white" {}
//		_BumpDepth ("Normal Intensity", Float) =1.0
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =1.0
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,8.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,10.0)) = 1.5
	}
	SubShader {
	Pass{
			Tags { "LightMode" = "Vertex" }
			Tags { "RenderType"="Opaque" }
			Lighting On
		
			CGPROGRAM
//			#pragma approxview noforwardadd noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
            #include "UnityCG.cginc"

			
			//uniform
			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform fixed4 _BumpTex_ST;
			uniform fixed _BumpDepth;
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			//distance
			uniform fixed _StartValue;
			uniform fixed _EndValue;
		
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				float3 rimColor : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};
			
			float3 CalculateLighting (float4 vertex, float3 normal, int lightIndex)
			{
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

				float3 lightColor = float3(0,0,0);//UNITY_LIGHTMODEL_AMBIENT.xyz;
	
				for (int i = 0; i < lightIndex; i++) {
					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					float lengthSq = dot(toLight, toLight);
					toLight *= rsqrt(lengthSq);

					float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		
					//Spotlight stuff
					float rho = max (0, dot(toLight, unity_SpotDirection[i].xyz));
					float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
					atten *= saturate(spotAtt);

					float diff = max (0, dot (viewN, toLight));
					lightColor += unity_LightColor[i].rgb * (diff * atten);
				}
				return lightColor;
			}
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				o.col=float4(CalculateLighting (v.vertex, v.normal, 1),1.0);
				UNITY_TRANSFER_FOG(o,o.pos);
				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
				
				//Calculate rim color
    			half3 rimAngleColor = 1.0 - saturate(dot (normalize(viewDirection), normalDirection));
				rimAngleColor=pow(rimAngleColor,_RimAngle);
				float3 rimColor=_RimColor*_RimIntensity*rimAngleColor*_RimColor.xyzw;
				
				o.rimColor=rimColor;
				
				return o;
			}
			
			float4 frag(vertexOutput i) : COLOR
			{	
			
				//textures
				fixed4 tex = tex2D(_MainTex,_MainTex_ST.xy*i.texcoord.xy + _MainTex_ST.zw);
				
				//final
				half3 texFinal=tex.rgb;
				half3 rimFinal = i.rimColor;
				half3 emissionFinal = _EmissionColor*_EmissionIntensity;
				half3 lightFinal=(texFinal*(i.col*_Color+emissionFinal));
				
				fixed3 colorFinal=rimFinal+lightFinal;
				
				UNITY_APPLY_FOG(i.fogCoord, colorFinal);
				
				return float4(colorFinal,1.0);
			}
			
		
		ENDCG
	} }	
	FallBack "Diffuse"
}
