﻿Shader "Custom/WaterShader"
{
    Properties
    {
        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        [MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        _CellsBuffer("Cells buffer", 2D) = "black" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
    }

    SubShader
    {
        // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
        // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
        // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
        // "UniversalRenderPipeline"
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300

        // ------------------------------------------------------------------
        // Forward pass. Shades GI, emission, fog and all lights in a single pass.
        // Compared to Builtin pipeline forward renderer, LWRP forward renderer will
        // render a scene with multiple lights with less drawcalls and less overdraw.
        Pass
        {
            // "Lightmode" tag must be "UniversalForward" or not be defined in order for
            // to render objects.
            Name "StandardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
        // Required to compile gles 2.0 with standard SRP library
        // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
        #pragma prefer_hlslcc gles
        #pragma exclude_renderers d3d11_9x
        #pragma target 2.0

        // -------------------------------------
        // Material Keywords
        // unused shader_feature variants are stripped from build automatically
        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _ALPHATEST_ON
        #pragma shader_feature _ALPHAPREMULTIPLY_ON
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICSPECGLOSSMAP
        #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        #pragma shader_feature _OCCLUSIONMAP

        #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature _GLOSSYREFLECTIONS_OFF
        #pragma shader_feature _SPECULAR_SETUP
        #pragma shader_feature _RECEIVE_SHADOWS_OFF

        // -------------------------------------
        // Universal Render Pipeline keywords
        // When doing custom shaders you most often want to copy and past these #pragmas
        // These multi_compile variants are stripped from the build depending on:
        // 1) Settings in the LWRP Asset assigned in the GraphicsSettings at build time
        // e.g If you disable AdditionalLights in the asset then all _ADDITIONA_LIGHTS variants
        // will be stripped from build
        // 2) Invalid combinations are stripped. e.g variants with _MAIN_LIGHT_SHADOWS_CASCADE
        // but not _MAIN_LIGHT_SHADOWS are invalid and therefore stripped.
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile_fog

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing

        #pragma vertex LitPassVertex
        #pragma fragment LitPassFragment

        // Required by all Universal Render Pipeline shaders.
        // It will include Unity built-in shader variables (except the lighting variables)
        // (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
        // It will also include many utilitary functions. 
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        // Include this if you are doing a lit shader. This includes lighting shader variables,
        // lighting and shadow functions
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        // Material shader variables are not defined in SRP or LWRP shader library.
        // This means _BaseColor, _BaseMap, _BaseMap_ST, and all variables in the Properties section of a shader
        // must be defined by the shader itself. If you define all those properties in CBUFFER named
        // UnityPerMaterial, SRP can cache the material properties between frames and reduce significantly the cost
        // of each drawcall.
        // In this case, for sinmplicity LitInput.hlsl is included. This contains the CBUFFER for the material
        // properties defined above. As one can see this is not part of the ShaderLibrary, it specific to the
        // LWRP Lit shader.
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

        #define LEVEL_MASK  0x00ff
        #define SOURCE_MASK 0x0100
        #define WALL_MASK   0x0200

        #define TEXTURE_OFFSET_MASK 0xf0000000

        uniform StructuredBuffer<uint> _CellsBuffer : register(t1);
        uniform float _Shift;

        struct Attributes
        {
            float4 positionOS   : POSITION;
            uint   unitIndex    : COLOR;
            float2 uv           : TEXCOORD0;
            uint   vertexId     : SV_VertexID;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float2 uv                       : TEXCOORD0;
            float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
            half3  normalWS                 : TEXCOORD3;

            half2  groundLevel              : COLOR;

#if _NORMALMAP
            half3 tangentWS                 : TEXCOORD4;
            half3 bitangentWS               : TEXCOORD5;
#endif

#ifdef _MAIN_LIGHT_SHADOWS
            float4 shadowCoord              : TEXCOORD6; // compute shadow coord per-vertex for the main light
#endif
            float4 positionCS               : SV_POSITION;
        };

        Varyings LitPassVertex(Attributes input)
        {
            Varyings output;

            float3 position = input.positionOS.xyz;

            uint textureOffset = (input.unitIndex & TEXTURE_OFFSET_MASK) >> 28;
            uint textureOffsetX = textureOffset & 0x3;
            uint textureOffsetY = (textureOffset & 0xC) >> 2;
            float2 uv = (input.uv + float2(textureOffsetX, textureOffsetY)) * 0.25;

            uint unitIndex = input.unitIndex & (~TEXTURE_OFFSET_MASK);

            uint cell = _CellsBuffer[unitIndex];

            if ((cell & WALL_MASK) == 0) {
                if (textureOffsetX % 2) {
                    if (position.y > 0) {
                        position.x += _Shift * 0.1;
                        position.y += _Shift * sin(_Shift * 1.5707) * 0.09f;
                    }
                    else {
                        position.x -= _Shift * 0.1 * 0.5;
                    }
                }
                else {
                    if (position.y > 0) {
                        position.x -= _Shift * 0.1;
                        position.y -= _Shift * sin(_Shift * 1.5707) * 0.09f;
                    }
                    else {
                        position.x += _Shift * 0.1 * 0.5;
                    }
                    
                }

                int vertexOffset = input.vertexId % 6;
                float current = float(_CellsBuffer[unitIndex] & LEVEL_MASK) / 255.0f;
                if (current > 1000.0f || current < 0.4f) {
                    position.y = 0.0f;
                }
                else {
                    current = clamp(current, 0.0, 1.0);
                    if ((vertexOffset == 0 || vertexOffset == 1) && unitIndex > 0) {
                        float left = clamp(float(_CellsBuffer[unitIndex - 1] & 0xff) / 255.0f, 0.0, 1.0);
                        if (left > 1000.0f) {
                            left = current;
                        }
                        position.y += clamp((current + left) * 0.5f, 0.0, 1.0) - 1.0;
                    }
                    else if ((vertexOffset == 2 || vertexOffset == 3)) {
                        position.y += clamp(current, 0.0, 1.0) - 1.0;
                    }
                    else if ((vertexOffset == 4 || vertexOffset == 5) && unitIndex + 1 < 512 * 128) {
                        float right = clamp(float(_CellsBuffer[unitIndex + 1] & 0xff) / 255.0f, 0.0, 1.0);
                        if (right > 1000.0f) {
                            right = current;
                        }
                        position.y += clamp((current + right) * 0.5f, 0.0, 1.0) - 1.0;
                    }
                }

                output.groundLevel.x = current;
            }
            else {
                position.y = -1.0f;
                output.groundLevel.x = 0.0;
            }

            output.groundLevel.y = position.y;

            VertexPositionInputs vertexInput = GetVertexPositionInputs(position);
            VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(float3(0.0, 0.0, -1.0), float4(1.0, 0.0, 0.0, 1.0));

            // Computes fog factor per-vertex.
            float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

            // TRANSFORM_TEX is the same as the old shader library.
            output.uv = TRANSFORM_TEX(uv, _BaseMap);

            output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
            output.normalWS = vertexNormalInput.normalWS;

            // Here comes the flexibility of the input structs.
            // In the variants that don't have normal map defined
            // tangentWS and bitangentWS will not be referenced and
            // GetVertexNormalInputs is only converting normal
            // from object to world space
#ifdef _NORMALMAP
            output.tangentWS = vertexNormalInput.tangentWS;
            output.bitangentWS = vertexNormalInput.bitangentWS;
#endif

#ifdef _MAIN_LIGHT_SHADOWS
            // shadow coord for the main light is computed in vertex.
            // If cascades are enabled, LWRP will resolve shadows in screen space
            // and this coord will be the uv coord of the screen space shadow texture.
            // Otherwise LWRP will resolve shadows in light space (no depth pre-pass and shadow collect pass)
            // In this case shadowCoord will be the position in light space.
            output.shadowCoord = GetShadowCoord(vertexInput);
#endif
            // We just use the homogeneous clip position from the vertex input
            output.positionCS = vertexInput.positionCS;
            return output;
        }

        half4 LitPassFragment(Varyings input) : SV_Target
        {
            // Surface data contains albedo, metallic, specular, smoothness, occlusion, emission and alpha
            // InitializeStandarLitSurfaceData initializes based on the rules for standard shader.
            // You can write your own function to initialize the surface data of your shader.
            SurfaceData surfaceData;
            InitializeStandardLitSurfaceData(input.uv, surfaceData);

#if _NORMALMAP
            half3 normalWS = TransformTangentToWorld(surfaceData.normalTS,
                half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
#else
            half3 normalWS = input.normalWS;
#endif
            normalWS = normalize(normalWS);
            half3 bakedGI = SampleSH(normalWS);

            float3 positionWS = input.positionWSAndFogFactor.xyz;
            half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

            // BRDFData holds energy conserving diffuse and specular material reflections and its roughness.
            // It's easy to plugin your own shading fuction. You just need replace LightingPhysicallyBased function
            // below with your own.
            BRDFData brdfData;
            InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

            // Light struct is provide by LWRP to abstract light shader variables.
            // It contains light direction, color, distanceAttenuation and shadowAttenuation.
            // LWRP take different shading approaches depending on light and platform.
            // You should never reference light shader variables in your shader, instead use the GetLight
            // funcitons to fill this Light struct.
#ifdef _MAIN_LIGHT_SHADOWS
            // Main light is the brightest directional light.
            // It is shaded outside the light loop and it has a specific set of variables and shading path
            // so we can be as fast as possible in the case when there's only a single directional light
            // You can pass optionally a shadowCoord (computed per-vertex). If so, shadowAttenuation will be
            // computed.
            Light mainLight = GetMainLight(input.shadowCoord);
#else
            Light mainLight = GetMainLight();
#endif

            // Mix diffuse GI with environment reflections.
            half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);

            // LightingPhysicallyBased computes direct light contribution.
            color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);
            if (input.groundLevel.x < 0.5f) {
                float factor = input.groundLevel.x / 0.5;
                color *= half3(factor * 0.7, factor * 0.9, factor);
            }
            if (input.groundLevel.y < 0.1) {
                color *= input.groundLevel.y / 0.1 + 0.1;
            }

            // Additional lights loop
#ifdef _ADDITIONAL_LIGHTS

            // Returns the amount of lights affecting the object being renderer.
            // These lights are culled per-object in the forward renderer
            int additionalLightsCount = GetAdditionalLightsCount();
            for (int i = 0; i < additionalLightsCount; ++i)
            {
                // Similar to GetMainLight, but it takes a for-loop index. This figures out the
                // per-object light index and samples the light buffer accordingly to initialized the
                // Light struct. If _ADDITIONAL_LIGHT_SHADOWS is defined it will also compute shadows.
                Light light = GetAdditionalLight(i, positionWS);

                // Same functions used to shade the main light.
                color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
            }
#endif
            // Emission
            color += surfaceData.emission;

            float fogFactor = input.positionWSAndFogFactor.w;

            // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
            // with a custom one.
            color = MixFog(color, fogFactor);
            return half4(color, surfaceData.alpha);
        }
        ENDHLSL
    }

    // Used for rendering shadowmaps
    UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        // Used for depth prepass
        // If shadows cascade are enabled we need to perform a depth prepass. 
        // We also need to use a depth prepass in some cases camera require depth texture
        // (e.g, MSAA is enabled and we can't resolve with Texture2DMS
        UsePass "Universal Render Pipeline/Lit/DepthOnly"

        // Used for Baking GI. This pass is stripped from build.
        UsePass "Universal Render Pipeline/Lit/Meta"
    }

}