Shader "Custom/Lava/UnderPortal"
{
    SubShader {
        ZWrite off 
        ColorMask 0
        Cull off

        Tags
        { 
            "RenderType"="Transparent"
            "Queue"="Geometry+1"
        }

        Stencil {
            Ref 2
            Comp always
            Pass replace
        }

        Pass {
            
        }
    }
}