Shader "Custom/Lava/Portal" {
    SubShader {
        ZWrite off 
        ColorMask 0
        Cull off

        Tags
        { 
            "RenderType"="Transparent"
            "Queue"="Geometry+2"
        }

        Stencil {
            Ref 1
            Comp always
            Pass replace
        }

        Pass {
            
        }
    }
}