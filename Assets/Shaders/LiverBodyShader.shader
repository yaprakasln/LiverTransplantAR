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
        _GrowthOffset("Growth Displacement", float) = 0
        _NecrosisAmount("Necrosis (Rot)", Range(0,1)) = 0
        _VascularOcclusion("Vascular Occlusion", Range(0,1)) = 0
        _FibrosisAmount("Fibrosis Amount", Range(0,1)) = 0
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
            float _GrowthOffset;
            float _NecrosisAmount;
            float _VascularOcclusion;
            float _FibrosisAmount;

            float pseudoNoise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 displacedPos = input.positionOS.xyz;
                output.positionCS = TransformObjectToHClip(displacedPos);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 baseColor = _BaseColor.rgb;
                
                // 1. Organic Transitions
                baseColor = lerp(baseColor, _IcterusColor.rgb, _IcterusIntensity);
                baseColor = lerp(baseColor, baseColor * 0.2, _DarkeningIntensity);
                
                // 2. Growing Purple Rot (Radial + Noise)
                float2 center = float2(0.5, 0.5);
                float dist = distance(input.uv, center);
                
                float noise = pseudoNoise(input.uv * 10.0) * 0.15;
                // The rot expands based on _NecrosisAmount
                // When amount is 0, nothing. When 1.0, covers full 0.0-1.0 UV range.
                float rotEdge = _NecrosisAmount * 1.2; 
                float rotMask = smoothstep(rotEdge, rotEdge - 0.2, dist + noise);
                
                float3 purpleRot = float3(0.15, 0.02, 0.15); // Dark Purple/Livid
                baseColor = lerp(baseColor, purpleRot, rotMask);

                // 3. Vascular Occlusion (Blood clot patterns along vessel paths)
                float veinNoise1 = pseudoNoise(input.uv * 8.0 + float2(3.7, 1.2));
                float veinNoise2 = pseudoNoise(input.uv * 15.0 + float2(7.3, 4.8));
                // Create branching vessel-like pattern using sine waves at different angles
                float veinPattern = sin(input.uv.x * 12.0 + input.uv.y * 6.0 + veinNoise1 * 2.0) * 0.5 + 0.5;
                veinPattern *= sin(input.uv.y * 10.0 - input.uv.x * 4.0 + veinNoise2 * 1.5) * 0.5 + 0.5;
                float veinMask = smoothstep(0.3, 0.7, veinPattern) * _VascularOcclusion;
                // Dark red-purple clot color: thrombosed vessels
                float3 clotColor = float3(0.25, 0.02, 0.08); // Dark crimson-maroon
                baseColor = lerp(baseColor, clotColor, veinMask * 0.85);

                // 4. Fibrosis (Gray-white scar tissue)
                float fibrNoise = pseudoNoise(input.uv * 6.0 + float2(5.1, 2.9)) * 0.3;
                float fibrPattern = smoothstep(0.4, 0.8, pseudoNoise(input.uv * 4.0)) * _FibrosisAmount;
                float3 fibrColor = float3(0.55, 0.52, 0.48); // Gray-white scar tissue
                baseColor = lerp(baseColor, fibrColor, fibrPattern);

                // 5. Lighting & Final Mix
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                Light light = GetMainLight();
                float3 lightDir = normalize(light.direction);
                float diffuse = saturate(dot(normal, lightDir));
                
                // Fibrosis reduces glossiness (dry, scarred surface)
                float fibrGlossReduction = 1.0 - _FibrosisAmount * 0.7;
                float currentGloss = _Glossiness * (1.0 - rotMask * 0.9) * fibrGlossReduction;
                float3 halfwayDir = normalize(lightDir + viewDir);
                float specular = pow(saturate(dot(normal, halfwayDir)), currentGloss * 128.0 + 1.0) * currentGloss;

                float3 finalColor = baseColor * (diffuse + 0.3) + (specular * light.color);
                
                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
