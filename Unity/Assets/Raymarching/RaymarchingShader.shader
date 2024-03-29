Shader "RaymarchingShader"
{
    Properties
    {
        _BlitTexture ("Main Tex", 2D) = "white" {}
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline" 
        }

        Cull Off 
        ZWrite Off 
        ZTest Off

        Pass
        {
            Name "Raymarching"

            HLSLPROGRAM
            #define MAX_DISTANCE 100
            #define EPSILON 0.001

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "UnityShaderVariables.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Shape
            {
                float4 color;
                float3 position;
                float size;
                float blend;
                int sdf;
            };

            StructuredBuffer<Shape> _Shapes;
            int _NumShapes;
            
            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float SphereDistance(float3 p, float3 center, float radius)
            {
                return length(p - center) - radius;
            }

            float CubeDistance(float3 p, float3 center, float3 size)
            {
                float3 o = abs(p - center) - size;
                float ud = length(max(o, 0));
                float n = max(max(min(o.x, 0), min(o.y, 0)), min(o.z, 0));
                return ud + n;
            }

            // https://github.com/SebLague/Ray-Marching/blob/master/Assets/Scripts/SDF/Raymarching.compute
            float4 Blend(float a, float b, float4 colA, float4 colB, float k)
            {
                float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
                float blendDst = lerp(b, a, h) - k * h * (1.0 - h);
                float3 blendCol = lerp(colB, colA, h).rgb;
                return float4(blendCol, blendDst);
            }

            float SDF(float3 eye, Shape shape)
            {
                if (shape.sdf == 0) // Sphere
                {
                    return SphereDistance(eye, shape.position, shape.size);
                }
                else if (shape.sdf == 1) // Cube
                {
                    return CubeDistance(eye, shape.position, shape.size);
                }
    
                return MAX_DISTANCE;
            }
            
            float4 ProcessMarchingStep(float3 p)
            {
                float4 resultColor = 1;
                float resultDist = MAX_DISTANCE;
                for (int i = 0; i < _NumShapes; i++)
                {
                    Shape shape = _Shapes[i];
                    float dist = SDF(p, shape);
                    float4 blend = Blend(resultDist, dist, resultColor, shape.color, shape.blend);
                    resultColor = float4(blend.rgb, 1);
                    resultDist = blend.w;
                }
    
                return float4(resultColor.rgb, resultDist);
            }

            float3 EstimateNormal(float3 p)
            {
                float x = ProcessMarchingStep(float3(p.x + EPSILON, p.y, p.z)).w - ProcessMarchingStep(float3(p.x - EPSILON, p.y, p.z)).w;
                float y = ProcessMarchingStep(float3(p.x, p.y + EPSILON, p.z)).w - ProcessMarchingStep(float3(p.x, p.y - EPSILON, p.z)).w;
                float z = ProcessMarchingStep(float3(p.x, p.y, p.z + EPSILON)).w - ProcessMarchingStep(float3(p.x, p.y, p.z - EPSILON)).w;
                return normalize(float3(x, y, z));
            }

            // Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl
            v2f vert (uint vertexID : SV_VertexID)
            {
                v2f o;
                o.vertex = GetFullScreenTriangleVertexPosition(vertexID);
                o.uv = GetFullScreenTriangleTexCoord(vertexID);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 c = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, i.uv);
                
                float2 uv = i.uv * 2 - 1;
                float3 rayOrigin = mul(UNITY_MATRIX_I_V, float4(0, 0, 0, 1)).xyz;
                float3 rayDir = mul(unity_CameraInvProjection, float4(uv, 0, 1)).xyz;
                rayDir = mul(UNITY_MATRIX_I_V, float4(rayDir, 0)).xyz;
                rayDir = normalize(rayDir);
    
                float maxDist = 0.0;
                while (maxDist < MAX_DISTANCE)
                {
                    float4 result = ProcessMarchingStep(rayOrigin);

                    if (result.w > EPSILON)
                    {
                        rayOrigin += rayDir * result.w;
                        maxDist += result.w;
                    }
                    else
                    {
                        float3 pointOnSurface = rayOrigin + rayDir * result.w;
                        float3 normal = EstimateNormal(pointOnSurface);
                        float3 lightDir = _WorldSpaceLightPos0.xyz;
                        float lighting = dot(normal, lightDir);
                        c = float4(result.rgb * lighting, 1);
                        break;
                    }
                }
    
                return c;
            }
            ENDHLSL
        }
    }
}
