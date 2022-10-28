shader "MyTownProject/MyShader"{

    properties{
        _BaseMap("Albedo", 2D) = "white" {}
        _NORMALMAP("NormalMap", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
    }
    SubShader{
        tags{
            "RenderPipeLine" = "UniversalPipeline"
        }
        Pass{
            Name "ForwardRenderer"
            tags{
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // CBUFFER_START()
            // float4 _MainTexture;
            // CBUFFER_END
            

            #include "MyShaderLibrary/MyForwardRenderer.hlsl"
            ENDHLSL
        }
        Pass{
            Name "ShadowCaster"
            tags{
                "LightMode" = "ShadowCaster"
            }

            ColorMask 0
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
                    
            ENDHLSL
        }

    }
}