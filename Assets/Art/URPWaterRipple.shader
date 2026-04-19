Shader "Custom/URPWaterRipple"
{
    Properties
    {
        _BaseColor ("Water Color", Color) = (0.1, 0.4, 0.6, 1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.8

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Float) = 1

        _WaveSpeed ("Wave Speed", Float) = 0.5
        _WaveScale ("Wave Scale", Float) = 0.1

        // Ripple params
        _ImpactPos ("Impact Position", Vector) = (0,0,0,0)
        _ImpactTime ("Impact Time", Float) = 0
        _RippleSpeed ("Ripple Speed", Float) = 3
        _RippleFrequency ("Ripple Frequency", Float) = 15
        _RippleStrength ("Ripple Strength", Float) = 0.1
        _RippleRadius ("Ripple Radius", Float) = 5
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            float4 _BaseColor;
            float _Smoothness;
            float _NormalStrength;

            float _WaveSpeed;
            float _WaveScale;

            float3 _ImpactPos;
            float _ImpactTime;
            float _RippleSpeed;
            float _RippleFrequency;
            float _RippleStrength;
            float _RippleRadius;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                // Base wave motion
                float wave = sin(worldPos.x * 0.5 + _Time.y * _WaveSpeed) * 0.05;
                worldPos.y += wave;

                // Ripple calculation
                float dist = distance(worldPos.xz, _ImpactPos.xz);
                float timeSince = _Time.y - _ImpactTime;

                float ripple = sin(dist * _RippleFrequency - timeSince * _RippleSpeed);
                float falloff = saturate(1.0 - dist / _RippleRadius);

                worldPos.y += ripple * falloff * _RippleStrength;

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldPos = worldPos;
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Scrolling UV for subtle movement
                float2 uv = IN.uv + _Time.y * _WaveSpeed * 0.05;

                float3 normalTex = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv).xyz * 2 - 1;
                normalTex.xy *= _NormalStrength;

                float3 normalWS = normalize(float3(normalTex.xy, 1));

                // Simple lighting (fake but effective)
                float3 lightDir = normalize(float3(0.3, 1, 0.5));
                float NdotL = saturate(dot(normalWS, lightDir));

                float3 color = _BaseColor.rgb * (0.5 + NdotL * 0.5);

                return float4(color, 1);
            }

            ENDHLSL
        }
    }
}