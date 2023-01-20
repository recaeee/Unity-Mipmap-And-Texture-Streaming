Shader "Tool Shaders/MipmapDebugger"
{
    Properties
    {
        _MainTex("Main Tex",2D) = "white"{}
        _NonMipColor("Non Mip Color",Color) = (0.5,0,0.2,1)
        [Header(Debug Color)]
        _Mip0Color("Mip 0 Color",Color) = (0,1,0,1)
        _Mip1Color("Mip 1 Color",Color) = (0,0.7,0,1)
        _Mip2Color("Mip 2 Color",Color) = (0,0.5,0,1)
        _Mip3Color("Mip 3 Color",Color) = (0,0.3,0,1)
        _MipHigherColor("Mip Higher Color",Color) = (0,0.2,0,1)
        [Header(Debug Settings)]
        _BlendWeight("Blend Weight",Range(0,1)) = 0.7
    }

    Subshader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex MipmapDebugPassVertex;
            #pragma fragment MipmapDebugPassFragment;
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary//Core.hlsl"
            
            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);float4 _MainTex_ST;float4 _MainTex_MipInfo;
            float4 _NonMipColor, _Mip0Color, _Mip1Color, _Mip2Color, _Mip3Color, _MipHigherColor;
            float _BlendWeight;

            struct Attributes
            {
                float3 positionOS:POSITION;
                float2 uv:TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            int GetMipCount(Texture2D tex)
            {
                int mipLevel,width,height,mipCount;
                mipLevel = width = height = mipCount = 0;
                //in参数：mipmapLevel
                //out参数：width：纹理宽度，以纹素为单位
                //out参数：height：纹理高度，以纹素为单位
                //out参数：mipCount：纹理mipmap级别数
                tex.GetDimensions(mipLevel,width,height,mipCount);
                return mipCount;
            }

            float3 GetCurMipColorByManualColor(float4 mipInfo)
            {
                //mipInfo:
                //x:系统设置的maxReductionLevel
                //y:纹理的mip总数
                //z:纹理串流系统计算出应该使用的纹理Mip等级
                //w:当前加载的Mip等级
                int desiredMipLevel = int(mipInfo.z);
                int mipCount = int(mipInfo.y);
                int loadedMipLevel = int(mipInfo.w);
                if(mipCount == 0)
                {
                    return _NonMipColor;
                }
                else
                {
                    if(loadedMipLevel == 0)
                    {
                        return _Mip0Color;
                    }
                    else if (loadedMipLevel == 1)
                    {
                        return _Mip1Color;
                    }
                    else if(loadedMipLevel == 2)
                    {
                        return _Mip2Color;
                    }
                    else if(loadedMipLevel == 3)
                    {
                        return _Mip3Color;
                    }
                    else if(loadedMipLevel > 3)
                    {
                        return _MipHigherColor;
                    }
                    else
                    {
                        return _NonMipColor;
                    }
                }
            }

            float4 GetCurMipColorByAuto(float4 mipInfo)
            {
                                //mipInfo:
                //x:系统设置的maxReductionLevel
                //y:纹理的mip总数
                //z:纹理串流系统计算出应该使用的纹理Mip等级
                //w:当前加载的Mip等级
                int desiredMipLevel = int(mipInfo.z);
                int mipCount = int(mipInfo.y);
                int loadedMipLevel = int(mipInfo.w);
                float mipIntensity = 1 - (float)loadedMipLevel / (float)mipCount;
                return float4(mipIntensity,0,0,1);
            }

            Varyings MipmapDebugPassVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            float4 MipmapDebugPassFragment(Varyings input):SV_TARGET
            {
                float3 originColor = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv);
                float3 debugColor =  GetCurMipColorByManualColor(_MainTex_MipInfo);
                float3 blendedColor = lerp(originColor,debugColor,_BlendWeight);
                return float4(blendedColor,1);
            }

            
            ENDHLSL
        }
    }
}