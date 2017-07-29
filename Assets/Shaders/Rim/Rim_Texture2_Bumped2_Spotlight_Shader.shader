// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Rim/Texture2 Bumped2 Spotlight" {
	Properties {
		_MainTex ("Emission", 2D) = "white" {}
		_MainTex2 ("Emission", 2D) = "white" {}
		_BumpTex ("Normal", 2D) = "white" {}
		_BumpTex2 ("Normal 2", 2D) = "white" {}
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =1.0
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,12.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.5
    	_LightAngle ("Light Angle", Range(0.0,2.0)) = 0.6
	}
	SubShader {
		Pass{
			Tags { "RenderType"="Geometry" "LightMode"="ForwardBase"}
			
		
			CGPROGRAM
			#pragma approxview noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			//uniform
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _MainTex2;
			uniform fixed4 _MainTex2_ST;
			uniform sampler2D _BumpTex;
			uniform fixed4 _BumpTex_ST;
			uniform sampler2D _BumpTex2;
			uniform fixed4 _BumpTex2_ST;
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			uniform half _LightAngle;
		
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				float3 tangentSpaceLightDir : TEXCOORD1;
			};
			
			float3 CalculateLighting (float4 vertex, float3 normal, int lightIndex)
			{
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
	
				for (int i = 0; i < lightIndex; i++) {
					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					float lengthSq = dot(toLight, toLight);
					toLight *= rsqrt(lengthSq);

					float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
					//Arbitrary multiplier to increase intensity without increasing range
//					atten*=2;
		
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
				
				//Calculate useful variables
				half3 vTangent = v.tangent;
				half3 vNormal = v.normal;
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float3 binormal = cross( normalize(vNormal), normalize(vTangent.xyz) );
                float3x3 rotation = float3x3( vTangent.xyz, binormal, vNormal );
               
				//Calculate diffuse color
//				half3 diffuseColor=saturate(_LightAngle+dot(normalDirection,lightDirection));
				
               	//Send variables to frag
                o.tangentSpaceLightDir = mul(rotation, viewDir);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				o.col=float4(CalculateLighting (v.vertex, vNormal, 1),1.0);
				
				return o;
			}
			
			float4 frag(vertexOutput i) : COLOR
			{	
				half3 viewDirection=i.tangentSpaceLightDir;
				
				half atten=1.0;
			
				//textures
//				fixed4 tex = tex2D(_MainTex,_MainTex_ST.xy*i.texcoord.xy + float2(_MainTex_ST.z*_Time.y,_MainTex_ST.w));
				fixed4 tex1 = tex2D(_MainTex,_MainTex_ST.xy*i.texcoord.xy + float2(_MainTex_ST.z,_MainTex_ST.w));
				fixed4 tex2 = tex2D(_MainTex2,_MainTex2_ST.xy*i.texcoord.xy + float2(_MainTex2_ST.z,_MainTex2_ST.w));
				
				fixed4 tex = (tex1+tex2)*.5;
//				fixed4 tex = lerp(tex1,tex2,.5);
				
				float2 coords = _BumpTex_ST.xy*i.texcoord.xy + _BumpTex_ST.zw;
				fixed4 texN1 = tex2D(_BumpTex,coords);
				fixed4 texN2 = tex2D(_BumpTex2,_BumpTex2_ST.xy*i.texcoord.xy + _BumpTex2_ST.zw*_Time.y);
				
				fixed4 texN = (texN1+texN2)*.5;
				
             	half3 normalDirection = (texN.rgb * 2.0) - 1.0;
				
				//rim
				half rim = 1 - saturate(dot(normalize(viewDirection),normalDirection));
				half3 rimLighting= _RimColor.rgb * pow(rim,_RimAngle);
				rimLighting*=_RimIntensity;
				
				
				//final
				half3 texFinal=tex.rgb;
				half3 rimFinal = rimLighting;
				half3 emissionFinal = _EmissionColor*_EmissionIntensity;
				half3 lightFinal=emissionFinal;
				
				fixed3 colorFinal=texFinal*lightFinal+i.col+rimFinal;
				
				return float4(colorFinal,1.0);
			}
				
			
			ENDCG
		} 
	}	
}