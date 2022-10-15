#ifndef QUIBLI_STYLIZED_INPUT_INCLUDED
#define QUIBLI_STYLIZED_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)

// --- Compatibility with Simple Lit.
float4 _BaseMap_ST;
half4 _BaseColor;
// -----------------------------

half _Cutoff;
half _Surface;
// half4 _EmissionColor;

// --- DR_SPECULAR_ON
half4 _FlatSpecularColor;
float _FlatSpecularSize;
float _FlatSpecularEdgeSmoothness;
// --- DR_SPECULAR_ON

// --- DR_RIM_ON
half4 _FlatRimColor;
float _FlatRimSize;
float _FlatRimEdgeSmoothness;
float _FlatRimLightAlign;
// --- DR_RIM_ON

// --- DR_GRADIENT_ON
half4 _ColorGradient;
half _GradientCenterX;
half _GradientCenterY;
half _GradientSize;
half _GradientAngle;
// --- DR_GRADIENT_ON

half _TextureImpact;

half _SelfShadingSize;
half _LightContribution;

half _OverrideLightmapDir;
half3 _LightmapDirection;

half4 _LightAttenuation;
half4 _ShadowColor;

float4 _DetailMap_ST;
half _DetailMapImpact;
half4 _DetailMapColor;

half4 _OutlineColor;
half _OutlineWidth;
half _OutlineScale;
half _OutlineDepthOffset;
half _CameraDistanceImpact;

// --- DR_DITHER_ON
half _DitherOffset;
half _DitherRange;
half _ClipThreshold;
half _AlphaImpact;
half _DitherSize;
// --- DR_DITHER_ON

CBUFFER_END
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
inline void InitializeSimpleLitSurfaceData(float4 screenPosition, float2 uv, out SurfaceData outSurfaceData)
{
    outSurfaceData = (SurfaceData)0;

    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));

	// Screen-door transparency: Discard pixel if below threshold.
    #if DR_DITHER_ON
    float sp1 = clamp((screenPosition.w - _DitherOffset) * _DitherRange, 0, 1);
    outSurfaceData.alpha = (albedoAlpha.a * _BaseColor.a) * sp1;
    float4 dividedScreenPos = screenPosition/_DitherSize;
    float OutPos;
    Unity_Dither_float(_ClipThreshold, dividedScreenPos, OutPos);
	//clip(outSurfaceData.alpha - OutPos);
    AlphaDiscard(outSurfaceData.alpha, OutPos);
    #else
    outSurfaceData.alpha = albedoAlpha.a * _BaseColor.a;
    AlphaDiscard(outSurfaceData.alpha, _Cutoff);
    #endif

    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
    #ifdef _ALPHAPREMULTIPLY_ON
    outSurfaceData.albedo *= outSurfaceData.alpha;
    #endif

    outSurfaceData.metallic = 0.0; // unused
    outSurfaceData.specular = 0.0; // unused
    outSurfaceData.smoothness = 0.0; // unused
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
    outSurfaceData.occlusion = 1.0; // unused
    outSurfaceData.emission = 0.0; //SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

half4 SampleSpecularSmoothness(half2 uv, half alpha, half4 specColor, TEXTURE2D_PARAM(specMap, sampler_specMap))
{
    half4 specularSmoothness = half4(0.0h, 0.0h, 0.0h, 1.0h);
    return specularSmoothness;
}

#endif  // QUIBLI_STYLIZED_INPUT_INCLUDED
