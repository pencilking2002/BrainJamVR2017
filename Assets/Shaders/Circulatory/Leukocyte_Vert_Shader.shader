// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Circulatory/LeukocyteVert" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Emission", 2D) = "white" {}
		_BumpTex ("Normal", 2D) = "white" {}
		_BumpDepth ("Normal Intensity", Float) =1.0
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =1.0
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,10.0)) = 1.5
	}
	SubShader {
	Pass{
			Tags { "LightMode" = "Vertex" }
			Tags { "RenderType"="Opaque" }
               Tags {"LightMode" = "Always" /* Upgrade NOTE: changed from PixelOrNone to Always */}
            /* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
			LOD 200
			Lighting On
		
			CGPROGRAM
			#pragma approxview noforwardadd noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
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
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				float3 tangentSpaceLightDir : TEXCOORD1;
				UNITY_FOG_COORDS(2)
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
//					atten*=16;
//					atten=pow(atten,.3);
		
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
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
                float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
               
               	//Send variables to frag
                o.tangentSpaceLightDir = mul(rotation, viewDir);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				o.col=float4(CalculateLighting (v.vertex, v.normal, 1),1.0);
				UNITY_TRANSFER_FOG(o,o.pos);
				
				return o;
			}
			
			float4 frag(vertexOutput i) : COLOR
			{	
				half3 viewDirection=i.tangentSpaceLightDir;
//				half3 rimViewDirection= normalize(_WorldSpaceCameraPos.xyz - i.pos.xyz);	
//				half3 lightDirection=normalize(_WorldSpaceLightPos0.xyz);
				half atten=1.0;
			
				//textures
				fixed4 tex = tex2D(_MainTex,_MainTex_ST.xy*i.texcoord.xy + _MainTex_ST.zw);
				fixed4 texN = tex2D(_BumpTex,_BumpTex_ST.xy*i.texcoord.xy + _BumpTex_ST.zw);
				
				
             	half3 normalDirection = (texN.rgb * 2.0) - 1.0;
             	normalDirection*=_BumpDepth;
				
				//lighting
//				half3 diffuseColor = atten  * saturate(dot(normalDirection,lightDirection));
				
				//rim
				half rim = 1 - saturate(dot(normalize(viewDirection),normalDirection));
				half3 rimLighting= _RimColor.rgb * pow(rim,_RimAngle);
				rimLighting*=_RimIntensity;
				
				
				//final
				half3 texFinal=tex.rgb*_Color;
				half3 rimFinal = rimLighting;
				half3 emissionFinal = _EmissionColor*_EmissionIntensity;
				half3 lightFinal=texFinal*(i.col)+emissionFinal;
				
				fixed3 colorFinal=rimFinal+lightFinal;
				
				UNITY_APPLY_FOG(i.fogCoord, colorFinal);
				
				return float4(colorFinal,1.0);
			}
				
			
			ENDCG
		} 
	}	
}
