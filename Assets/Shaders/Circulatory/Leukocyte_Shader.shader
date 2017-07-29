    Shader "Custom/Circulatory/Leukocyte" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
//        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =1.0
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _RimIntensity("Rim Intensity", Range(0,3.0))=1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
    CGPROGRAM
    #pragma surface surf Lambert approxview noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass

	
    fixed4 _Color;
//    sampler2D _MainTex;
    sampler2D _BumpMap;
    //Emission
    fixed _EmissionIntensity;
    fixed4 _EmissionColor;
    //Rim
    fixed4 _RimColor;
    half _RimPower;
    fixed _RimIntensity;
     
    struct Input {
        half2 uv_MainTex;
        half2 uv_BumpMap;
        half3 viewDir;
    };
     
    void surf (Input IN, inout SurfaceOutput o) {
//        fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
        fixed4 c =  _Color;
        fixed4 e =  _EmissionColor;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        o.Normal = 1.0*UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
        half3 rimColor=pow(rim,_RimPower)*_RimColor.rgb;;
        rimColor=saturate(rimColor)*_RimIntensity;
        o.Emission =  _EmissionIntensity*e.rgb +rimColor;
    }
    ENDCG
    }
    FallBack "Self-Illumin/Specular"
    }
     
