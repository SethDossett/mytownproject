#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"

struct Inputs{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
    float4 color : COLOR;
};

struct Interpolators{
    float4 positionWS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
    float4 color : COLOR;

};


//TEXTURE2D(_BaseMap);  SAMPLER(sampler_BaseMap);
TEXTURE2D(_NormalMap);  SAMPLER(sampler_NormalMap);

//CBUFFER_START(UnityPerMaterial)
//float4 _BaseMap_ST;
//float4 _Color;
//CBUFFER_END

Interpolators vert(Inputs inputs){
    Interpolators o = (Interpolators)0;
    o.color = inputs.color * _BaseColor;

    VertexPositionInputs positionInputs = GetVertexPositionInputs(inputs.positionOS.xyz);
    o.positionWS = positionInputs.positionCS;
    //o.positionWS = TransformObjectToHClip(inputs.positionOS.xyz);
    o.uv = TRANSFORM_TEX(inputs.uv, _BaseMap);
    //o.positionWS = UnityObjectToClipPos(inputs.positionOS);

    return o;
}

float4 frag(Interpolators IN) : SV_TARGET{

    InputData inputData;
    float2 uv = IN.uv;
    float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    //float3 normal = SampleNormal(uv, SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap));

    SurfaceData surfaceData;

    //float4 color = UniversalFragmentBlinnPhong(inputData)
    return albedo * IN.color;
}