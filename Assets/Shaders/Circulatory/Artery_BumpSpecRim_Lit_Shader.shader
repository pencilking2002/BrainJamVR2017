Shader "Custom/Circulatory/Artery Lit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _Emission ("Emission", Range(0.1,1.0)) =1.0
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power",  Range(0.5,8.0)) = 3.0
//        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 400
    	CGPROGRAM
   		#pragma surface surf Lambert 
   		#pragma debug
   		//noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
     
//    inline fixed4 LightingMobileLambert (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
//	{
//		fixed diff = max (0, dot (s.Normal, lightDir));
//	
//		fixed4 c;
//		c.rgb = (s.Albedo * _LightColor0.rgb * diff) * atten;
//		
//		return c;
//	}
	
    	sampler2D _MainTex;
    	sampler2D _BumpMap;
    	fixed4 _Color;
    	half _Shininess;
    	float4 _RimColor;
    	float _RimPower;
    	fixed _Emission;
    	fixed4 _EmissionColor;
     
    	struct Input {
        	float2 uv_MainTex;
        	float2 uv_BumpMap;
        	float3 viewDir;
    	};
     
    	void surf (Input IN, inout SurfaceOutput o) {
        	float4 tex = tex2D(_MainTex, IN.uv_MainTex);
        	float4 c = tex * _Color;
        	o.Albedo = c.rgb;
        	o.Normal = 1.0*UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        	float rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
        	o.Emission =  (_Emission*_EmissionColor*tex+ pow (rim, _RimPower) * _RimColor.rgb);
    	}
    	ENDCG
    }
    FallBack "Self-Illumin/Specular"
}
     
