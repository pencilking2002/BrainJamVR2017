Shader "Custom/Rim" {
Properties {
    _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    _EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
	_EmissionTex ("Base (RGB)", 2D) = "white" {}
    _RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    _RimPower ("Rim Angle", Range(0.5,8.0)) = 2.0
    _RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.0
}
SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma surface surf Lambert noambient noforwardadd

sampler2D _EmissionTex;
samplerCUBE _Cube;


fixed _EmissionIntensity;
fixed4 _EmissionColor;
fixed4 _RimColor;
half _RimPower;
half _RimIntensity;
    
struct Input {
	float2 uv_EmissionTex;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
    half rimAngle = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
    fixed4 rimColor = pow(rimAngle,_RimPower)*_RimIntensity*_RimColor;
    
    fixed4 emissionColor= tex2D(_EmissionTex,IN.uv_EmissionTex)*_EmissionIntensity*_EmissionColor;
   	
    o.Emission = emissionColor+rimColor;
}
ENDCG
}
	
FallBack "Reflective/VertexLit"
} 
