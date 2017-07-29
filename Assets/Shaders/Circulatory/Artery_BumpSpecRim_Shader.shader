Shader "Custom/Circulatory/Artery Lit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Emission ("Brightness", Range(0.0,1.0)) =1.0
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
//        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimAngle ("Rim Angle",  Range(0.5,8.0)) = 3.0
        _RimIntensity("Rim Intensity", Range(0.0,1.0))=0.5
		//Distance
		_FadeDistance ("Fade Distance", Range(-.5,3000.0)) = 1.0
		_FadeRange ("Fade Range", Range(0.0,8.0)) = 1.0
//        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
//        Cull Off
        LOD 400
    	CGPROGRAM
   		#pragma surface surf MobileLambert 
   		//noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
     
    	inline fixed4 LightingMobileLambert (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
		{
			fixed diff = max (0, 0.5*(2.0+dot (s.Normal, lightDir)));
	
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff) * atten;
			
			return c;
		}
	
    	sampler2D _MainTex;
    	sampler2D _BumpMap;
    	fixed4 _Color;
//    	half _Shininess;
    	float4 _RimColor;
    	float _RimAngle;
    	fixed _RimIntensity;
    	
    	fixed _Emission;
		//Distance
		half _FadeDistance;
		half _FadeRange;
//    	fixed4 _EmissionColor;
     
    	struct Input {
        	float2 uv_MainTex;
        	float2 uv_BumpMap;
        	float3 viewDir;
        	float3 worldPos;
//          	fixed4 color : COLOR;
    	};
     
    	void surf (Input IN, inout SurfaceOutput o) {
        	float4 tex = tex2D(_MainTex, IN.uv_MainTex);
        	o.Normal = 1.0*UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        	float4 c = tex * _Color;
        	
        	float4 albedoColor=c*_Emission;
        	
        	float rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
        	float4 rimColor= _RimIntensity * pow(rim, _RimAngle) * _RimColor;
        	
        	//Fade distance
       	 	float dist =  distance(IN.worldPos,_WorldSpaceCameraPos)/_FadeDistance;
//        	float distanceColor = (_FadeRange-clamp(dist-_FadeDistance,0.0,_FadeRange))/_FadeRange;
        	float distanceColor = pow(dist,_FadeRange);
        	distanceColor= clamp(distanceColor,0,.12);
        	distanceColor+=.1;
        	
        	o.Emission =(albedoColor+rimColor);
//        	o.Emission=float4(distanceColor,distanceColor,distanceColor,distanceColor);
//        	o.Emission =  (_Emission*_EmissionColor*tex);
    	}
    	ENDCG
    }
}
     
