Shader "MyTownProject/CameraDitherFade"
    {
        Properties
        {
            _Color("Color", Color) = (0.3962264, 0.3962264, 0.3962264, 0)
            _DitherOffset("Dither Offset", Float) = 0.5
            _DitherRange("Dither Range", Float) = 0.5
            _ClipThreshold("Clip Threshold", Range(0, 1)) = 1
            _DitherSize("Dither Size", Range(0, 2)) = 0.5
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Opaque"
                "UniversalMaterialType" = "Lit"
                "Queue"="AlphaTest"
            }
            Pass
            {
                Name "Universal Forward"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
                #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT
                #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                #pragma multi_compile _ SHADOWS_SHADOWMASK
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define VARYINGS_NEED_VIEWDIRECTION_WS
                #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_FORWARD
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 tangentWS;
                    float3 viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    float2 lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 sh;
                    #endif
                    float4 fogFactorAndVertexLight;
                    float4 shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    float3 interp3 : TEXCOORD3;
                    #if defined(LIGHTMAP_ON)
                    float2 interp4 : TEXCOORD4;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 interp5 : TEXCOORD5;
                    #endif
                    float4 interp6 : TEXCOORD6;
                    float4 interp7 : TEXCOORD7;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.tangentWS;
                    output.interp3.xyz =  input.viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    output.interp4.xy =  input.lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.interp5.xyz =  input.sh;
                    #endif
                    output.interp6.xyzw =  input.fogFactorAndVertexLight;
                    output.interp7.xyzw =  input.shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.tangentWS = input.interp2.xyzw;
                    output.viewDirectionWS = input.interp3.xyz;
                    #if defined(LIGHTMAP_ON)
                    output.lightmapUV = input.interp4.xy;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.sh = input.interp5.xyz;
                    #endif
                    output.fogFactorAndVertexLight = input.interp6.xyzw;
                    output.shadowCoord = input.interp7.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 NormalTS;
                    float3 Emission;
                    float Metallic;
                    float Smoothness;
                    float Occlusion;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.NormalTS = IN.TangentSpaceNormal;
                    surface.Emission = float3(0, 0, 0);
                    surface.Metallic = 0;
                    surface.Smoothness = 0.5;
                    surface.Occlusion = 1;
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "GBuffer"
                Tags
                {
                    "LightMode" = "UniversalGBuffer"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _SHADOWS_SOFT
                #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
                #pragma multi_compile _ _GBUFFER_NORMALS_OCT
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define VARYINGS_NEED_VIEWDIRECTION_WS
                #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_GBUFFER
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 tangentWS;
                    float3 viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    float2 lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 sh;
                    #endif
                    float4 fogFactorAndVertexLight;
                    float4 shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    float3 interp3 : TEXCOORD3;
                    #if defined(LIGHTMAP_ON)
                    float2 interp4 : TEXCOORD4;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 interp5 : TEXCOORD5;
                    #endif
                    float4 interp6 : TEXCOORD6;
                    float4 interp7 : TEXCOORD7;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.tangentWS;
                    output.interp3.xyz =  input.viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    output.interp4.xy =  input.lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.interp5.xyz =  input.sh;
                    #endif
                    output.interp6.xyzw =  input.fogFactorAndVertexLight;
                    output.interp7.xyzw =  input.shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.tangentWS = input.interp2.xyzw;
                    output.viewDirectionWS = input.interp3.xyz;
                    #if defined(LIGHTMAP_ON)
                    output.lightmapUV = input.interp4.xy;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.sh = input.interp5.xyz;
                    #endif
                    output.fogFactorAndVertexLight = input.interp6.xyzw;
                    output.shadowCoord = input.interp7.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 NormalTS;
                    float3 Emission;
                    float Metallic;
                    float Smoothness;
                    float Occlusion;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.NormalTS = IN.TangentSpaceNormal;
                    surface.Emission = float3(0, 0, 0);
                    surface.Metallic = 0;
                    surface.Smoothness = 0.5;
                    surface.Occlusion = 1;
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SHADOWCASTER
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthNormals"
                Tags
                {
                    "LightMode" = "DepthNormals"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 tangentWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.tangentWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.tangentWS = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 NormalTS;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.NormalTS = IN.TangentSpaceNormal;
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "Meta"
                Tags
                {
                    "LightMode" = "Meta"
                }
    
                // Render State
                Cull Off
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define ATTRIBUTES_NEED_TEXCOORD2
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_META
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    float4 uv2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 Emission;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.Emission = float3(0, 0, 0);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                // Name: <None>
                Tags
                {
                    "LightMode" = "Universal2D"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_2D
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
    
                ENDHLSL
            }
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Opaque"
                "UniversalMaterialType" = "Lit"
                "Queue"="AlphaTest"
            }
            Pass
            {
                Name "Universal Forward"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
                #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT
                #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
                #pragma multi_compile _ SHADOWS_SHADOWMASK
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define VARYINGS_NEED_VIEWDIRECTION_WS
                #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_FORWARD
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 tangentWS;
                    float3 viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    float2 lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 sh;
                    #endif
                    float4 fogFactorAndVertexLight;
                    float4 shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    float3 interp3 : TEXCOORD3;
                    #if defined(LIGHTMAP_ON)
                    float2 interp4 : TEXCOORD4;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    float3 interp5 : TEXCOORD5;
                    #endif
                    float4 interp6 : TEXCOORD6;
                    float4 interp7 : TEXCOORD7;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.tangentWS;
                    output.interp3.xyz =  input.viewDirectionWS;
                    #if defined(LIGHTMAP_ON)
                    output.interp4.xy =  input.lightmapUV;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.interp5.xyz =  input.sh;
                    #endif
                    output.interp6.xyzw =  input.fogFactorAndVertexLight;
                    output.interp7.xyzw =  input.shadowCoord;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.tangentWS = input.interp2.xyzw;
                    output.viewDirectionWS = input.interp3.xyz;
                    #if defined(LIGHTMAP_ON)
                    output.lightmapUV = input.interp4.xy;
                    #endif
                    #if !defined(LIGHTMAP_ON)
                    output.sh = input.interp5.xyz;
                    #endif
                    output.fogFactorAndVertexLight = input.interp6.xyzw;
                    output.shadowCoord = input.interp7.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 NormalTS;
                    float3 Emission;
                    float Metallic;
                    float Smoothness;
                    float Occlusion;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.NormalTS = IN.TangentSpaceNormal;
                    surface.Emission = float3(0, 0, 0);
                    surface.Metallic = 0;
                    surface.Smoothness = 0.5;
                    surface.Occlusion = 1;
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SHADOWCASTER
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
                ColorMask 0
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "DepthNormals"
                Tags
                {
                    "LightMode" = "DepthNormals"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    float3 normalWS;
                    float4 tangentWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 TangentSpaceNormal;
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    float3 interp1 : TEXCOORD1;
                    float4 interp2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyz =  input.normalWS;
                    output.interp2.xyzw =  input.tangentWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.normalWS = input.interp1.xyz;
                    output.tangentWS = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 NormalTS;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.NormalTS = IN.TangentSpaceNormal;
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                    output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                Name "Meta"
                Tags
                {
                    "LightMode" = "Meta"
                }
    
                // Render State
                Cull Off
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define ATTRIBUTES_NEED_TEXCOORD2
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_META
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float4 uv1 : TEXCOORD1;
                    float4 uv2 : TEXCOORD2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float3 Emission;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.Emission = float3(0, 0, 0);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
    
                ENDHLSL
            }
            Pass
            {
                // Name: <None>
                Tags
                {
                    "LightMode" = "Universal2D"
                }
    
                // Render State
                Cull Back
                Blend One Zero
                ZTest LEqual
                ZWrite On
    
                // Debug
                // <None>
    
                // --------------------------------------------------
                // Pass
    
                HLSLPROGRAM
    
                // Pragmas
                #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
    
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
    
                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>
    
                // Defines
                #define _AlphaClip 1
                #define _NORMALMAP 1
                #define _NORMAL_DROPOFF_TS 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define VARYINGS_NEED_POSITION_WS
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_2D
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
    
                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    
                // --------------------------------------------------
                // Structs and Packing
    
                struct Attributes
                {
                    float3 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                    float3 WorldSpacePosition;
                    float4 ScreenPosition;
                };
                struct VertexDescriptionInputs
                {
                    float3 ObjectSpaceNormal;
                    float3 ObjectSpaceTangent;
                    float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float3 interp0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
    
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
    
                // --------------------------------------------------
                // Graph
    
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DitherOffset;
                float _DitherRange;
                float _ClipThreshold;
                float _DitherSize;
                CBUFFER_END
                
                // Object and Global properties
    
                // Graph Functions
                
                void isShadowCaster_float(out float isSC){
                    isSC = false;
                    #ifdef UNITY_PASS_SHADOWCASTER
                    isSC = true;
                    #endif
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Clamp_float(float In, float Min, float Max, out float Out)
                {
                    Out = clamp(In, Min, Max);
                }
                
                void Unity_Branch_float(float Predicate, float True, float False, out float Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A / B;
                }
                
                void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
                {
                    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    Out = In - DITHER_THRESHOLDS[index];
                }
    
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
    
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0 = _Color;
                    float _Split_2817db5781f44513b7c9edab584ecec3_R_1 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[0];
                    float _Split_2817db5781f44513b7c9edab584ecec3_G_2 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[1];
                    float _Split_2817db5781f44513b7c9edab584ecec3_B_3 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[2];
                    float _Split_2817db5781f44513b7c9edab584ecec3_A_4 = _Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0[3];
                    float _isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0;
                    isShadowCaster_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0);
                    float4 _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0 = IN.ScreenPosition;
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_R_1 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[0];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_G_2 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[1];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_B_3 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[2];
                    float _Split_672c853cf3014637b7cc610c6cbebf4d_A_4 = _ScreenPosition_fe5629987a9c42a3b5d15ab384865154_Out_0[3];
                    float _Property_907be12d889240f5af7fa4dc35991a41_Out_0 = _DitherOffset;
                    float _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2;
                    Unity_Subtract_float(_Split_672c853cf3014637b7cc610c6cbebf4d_A_4, _Property_907be12d889240f5af7fa4dc35991a41_Out_0, _Subtract_6c8ddc481807483eb8d822e04472024c_Out_2);
                    float _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0 = _DitherRange;
                    float _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2;
                    Unity_Multiply_float(_Subtract_6c8ddc481807483eb8d822e04472024c_Out_2, _Property_e96dba65ad634fdca1431fd5d1816f3d_Out_0, _Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2);
                    float _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3;
                    Unity_Clamp_float(_Multiply_feace10d9c5b4c7f9541192697e4d79c_Out_2, 0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3);
                    float _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3;
                    Unity_Branch_float(_isShadowCasterCustomFunction_833bc7ab61fe4733b997350e8fb9302d_isSC_0, 1, _Clamp_016b1a67a55b45b1ae44edaa2ceb36c7_Out_3, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3);
                    float _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    Unity_Multiply_float(_Split_2817db5781f44513b7c9edab584ecec3_A_4, _Branch_fa0831812ba9456f8ef318a4587054f1_Out_3, _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2);
                    float _Property_b8372416848b47e992dd7ae2bb3121d4_Out_0 = _ClipThreshold;
                    float4 _ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                    float _Property_8d366b598ecb441b8a3838febddb4de4_Out_0 = _DitherSize;
                    float4 _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2;
                    Unity_Divide_float4(_ScreenPosition_4d208475764d4bfba558099a372606c0_Out_0, (_Property_8d366b598ecb441b8a3838febddb4de4_Out_0.xxxx), _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2);
                    float _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    Unity_Dither_float(_Property_b8372416848b47e992dd7ae2bb3121d4_Out_0, _Divide_eb4c4f03ffb54c0088c398fd384949fb_Out_2, _Dither_f8ded980dc57480daece5954a57acd19_Out_2);
                    surface.BaseColor = (_Property_5d7910440513495abd6a8e8e2fa08ac0_Out_0.xyz);
                    surface.Alpha = _Multiply_65f83325bab0459d84c86c474a9b617e_Out_2;
                    surface.AlphaClipThreshold = _Dither_f8ded980dc57480daece5954a57acd19_Out_2;
                    return surface;
                }
    
                // --------------------------------------------------
                // Build Graph Inputs
    
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =           input.normalOS;
                    output.ObjectSpaceTangent =          input.tangentOS.xyz;
                    output.ObjectSpacePosition =         input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                
                
                
                
                    output.WorldSpacePosition =          input.positionWS;
                    output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
    
                // --------------------------------------------------
                // Main
    
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
    
                ENDHLSL
            }
        }
        CustomEditor "ShaderGraph.PBRMasterGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
    }