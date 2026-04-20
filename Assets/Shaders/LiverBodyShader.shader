Shader "Custom/LiverOrganicShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.5, 0.2, 0.2, 1)
        _IcterusColor("Icterus Color (Yellow)", Color) = (0.8, 0.7, 0.1, 1)
        _IcterusIntensity("Icterus Intensity", Range(0,1)) = 0
        _DarkeningIntensity("Health Darkening", Range(0,1)) = 0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _VascularIntensity("Vascularization", Range(0,1)) = 0.5
        _SteatosisAmount("Steatosis (Fatty)", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD3;
            };

            float4 _BaseColor;
            float4 _IcterusColor;
            float _IcterusIntensity;
            float _DarkeningIntensity;
            float _Glossiness;
            float _Metallic;
            float _VascularIntensity;
            float _SteatosisAmount;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Organic Color Blending
                float3 baseColor = _BaseColor.rgb;
                
                // Yellowing (Icterus)
                baseColor = lerp(baseColor, _IcterusColor.rgb, _IcterusIntensity);
                
                // Darkening (Tissue Health)
                baseColor = lerp(baseColor, baseColor * 0.2, _DarkeningIntensity);
                
                // Steatosis (Yellowish white spots)
                float3 fattyColor = float3(0.9, 0.85, 0.6);
                baseColor = lerp(baseColor, fattyColor, _SteatosisAmount * 0.5);

                // Simple Lighting
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                Light light = GetMainLight();
                float3 lightDir = normalize(light.direction);

                float diffuse = saturate(dot(normal, lightDir));
                
                // Subsurface Scattering Fake (Translucency)
                float sss = saturate(dot(viewDir, -lightDir)) * 0.3;
                
                // Specular
                float3 halfwayDir = normalize(lightDir + viewDir);
                float specular = pow(saturate(dot(normal, halfwayDir)), _Glossiness * 128.0) * _Glossiness;

                float3 finalColor = baseColor * (diffuse + sss + 0.2) + (specular * light.color);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
