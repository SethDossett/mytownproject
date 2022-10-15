#ifndef QUIBLI_VERTEX_INCLUDED
#define QUIBLI_VERTEX_INCLUDED

half _WindEnabled;
half _WindIntensity;
half _WindFrequency;
half _WindNoise;
half _WindSpeed;
half _WindDirection;

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
// ReSharper disable once CppUnusedIncludeDirective
#include "../Grass/FoliageLib.hlsl"

// float Dither(float In, float4 ScreenPosition)
// {
//     float2 uv = ScreenPosition.xy; //* _ScreenParams.xy;
//     float DITHER_THRESHOLDS[16] =
//     {
//         1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
//         13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
//         4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
//         16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
//     };
//     uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
//     return In - DITHER_THRESHOLDS[index];
// }
// float DitheredAlbedo(VertexPositionInputs input)
// {   
//     // Screen-door transparency: Discard pixel if below threshold.
// 		float4x4 thresholdMatrix =
// 		{  1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
// 		  13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
// 		   4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
// 		  16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
// 		};
// 		float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
// 		float2 pos = IN.screenPos.xy / IN.screenPos.w;
// 		pos *= _ScreenParams.xy; // pixel position
// 		clip(c.a - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);


//     //float4 screenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS));
//     //float4 screenPos = float4(screenPosition.xy / screenPosition.w, 0, 0);
//     //float4 divideOut = screenPos / _DitherSize.xxxx;

    
//     //return Dither(_ClipThreshold, divideOut);
// }

Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    // Wind (Ambient).
    #if defined(DR_WIND_ENABLED)
    {
        const float2 noise_uv = input.texcoord * _WindFrequency;
        const float noise01 = GradientNoise(noise_uv, 1.0);
        const float noise = (noise01 * 2.0 - 1.0) * _WindNoise;

        const float s = SineWave(input.positionOS.xyz, noise, _WindSpeed, _WindFrequency) * _WindIntensity;

        input.positionOS.xy += s * 0.1 * input.positionOS.y;
    }
    #endif

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    //float4 screenPos = ComputeScreenPos(input.positionOS);


    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

    // already normalized from normal transform to WS.
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = viewDirWS;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    output.tangentWS = tangentWS;
    #endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
    output.viewDirTS = viewDirTS;
    #endif

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    output.positionWS = vertexInput.positionWS;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    output.positionCS = vertexInput.positionCS;

    #if defined(DR_VERTEX_COLORS_ON)
    output.VertexColor = input.color;
    #endif

    return output;
}

#endif
