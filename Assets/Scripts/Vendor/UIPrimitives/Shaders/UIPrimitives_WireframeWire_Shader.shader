Shader "UIPrimitives/Wireframe_Wire" {
    Properties {
    	_Color ("Main Color", Color) = (0,0,0,1)
    }
    
    SubShader {
        Pass {
        	Blend SrcAlpha OneMinusSrcAlpha 
            ZWrite On
        	Color [_Color]
            Lighting Off
            Cull Off
			Tags {Queue=Transparent}
        }
    }
    
    FallBack "VertexLit"
} 