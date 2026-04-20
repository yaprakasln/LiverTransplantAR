Shader "Custom/Liver Professional (Realistic)"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo (RGB)", 2D) = "white" {}
        _BaseColor("Color Tint", Color) = (1,1,1,1)
        _GrowthOffset("Growth Offset (Regeneration)", Range(0, 0.5)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _IcterusIntensity("Icterus (Yellowing)", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "Queue"="Geometry" }
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
                float3 viewDirWS : TEXCOORD3;
                float2 uv : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _GrowthOffset;
                float _Glossiness;
                float _Metallic;
                float _IcterusIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // --- Vertex Inflation (Growth Simulation) ---
                float3 pos = input.positionOS.xyz + (input.normalOS * _GrowthOffset);
                
                output.positionCS = TransformObjectToHClip(pos);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(pos));
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Base colors
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half4 rejectionColor = half4(0.8, 0.7, 0.1, 1.0); // Sickly yellow/green
                
                // Blend with icterus (rejection)
                half4 finalColor = lerp(baseColor, rejectionColor, _IcterusIntensity);

                // --- Professional Lighting Polish ---
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(input.viewDirWS);
                
                // Fresnel / Rim Light (Glow at edges for organ depth)
                float rim = 1.0 - saturate(dot(viewDir, normal));
                rim = pow(rim, 3.0);
                half4 rimColor = half4(1, 1, 1, 1) * rim * 0.4;
                
                // Fake Specular (Glossy wet look)
                Light light = GetMainLight();
                float3 halfVec = normalize(light.direction + viewDir);
                float spec = pow(saturate(dot(normal, halfVec)), 32.0);
                half4 specColor = half4(1, 1, 1, 1) * spec * _Glossiness;

                return finalColor + rimColor + specColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
