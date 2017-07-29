// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PeterBurr" {
	Properties {

	}
	SubShader {
		Pass {
		Tags { "RenderType"="Geometry"
			"LightMode"="ForwardBase"}
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag

			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			float3 Hue(float H)
			{
			    float R = abs(H * 6 - 3) - 1;
			    float G = 2 - abs(H * 6 - 2);
			    float B = 2 - abs(H * 6 - 4);
			    return saturate(float3(R,G,B));
			}

			float3 HSVtoRGB(float3 HSV)
			{
			    return ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
			}

			float3 RGBtoHSV(float3 RGB)
			{
			    float3 HSV = 0;

			    HSV.z = max(RGB.r, max(RGB.g, RGB.b));
			    float M = min(RGB.r, min(RGB.g, RGB.b));
			    float C = HSV.z - M;

			    if (C != 0)
			    {
			        HSV.y = C / HSV.z;
			        float3 Delta = (HSV.z - RGB) / C;
			        Delta.rgb -= Delta.brg;
			        Delta.rg += float2(2,4);
			        if (RGB.r >= HSV.z)
			            HSV.x = Delta.b;
			        else if (RGB.g >= HSV.z)
			            HSV.x = Delta.r;
			        else
			            HSV.x = Delta.g;
			        HSV.x = frac(HSV.x / 6);
			    }

			    return HSV;
			}
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.col = float4(1,1,1,0);

				return o;
			}

			float3 Dither(float3 inputColor) {

				//Do dithering
				return inputColor;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				//Color Pass
				float3 originalColor = o.col;
				float3 originalColorHSV = RGBtoHSV(originalColor);
				originalColorHSV.b = 1.0;//Brightness set to 1
				float3 fullBrightnessColor = RGBtoHSV(originalColorHSV);
				float isColor = clamp(originalColorHSV.g*1000.0,0,1);//this will be 1 for anything with a saturation greater than 0

				float colorPassFinal = lerp(originalColor,fullBrightnessColor,isColor);


				//B&W Pass
				float3 white = float3(1.0,1.0,1.0);
				float3 black = float3(0.0,0.0,0.0);
				float3 blackAndWhitePassFinal = lerp(black,white,isColor);

				//Dither Pass
				float3 ditherPassFinal = Dither(blackAndWhitePassFinal);

				float3 finalColor = colorPassFinal*ditherPassFinal;

				return o.col;
			}
			ENDCG
		}
	}
}