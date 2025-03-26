Shader "Hidden/Pixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }
   

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        Pass 
        {
            Name "PixelBlitPass"

            HLSLINCLUDE
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionHCS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);

            SamplerState sampler_point_clamp;
        
            uniform float2 _BlockCount;
            uniform float2 _BlockSize;
            uniform float2 _HalfBlockSize;


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
		        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionCS = float4(IN.positionHCS.xyz, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                    OUT.positionCS.y *= -1;
                #endif

                OUT.uv = IN.uv;
                return OUT;
            }

        

        ENDHLSL
        

        //Pass
        //{
        //    Name "Pixelation"

            HLSLPROGRAM
                TEXTURE2D_X(_CameraOpaqueTexture);
                //SAMPLER(sampler_CameraOpaqueTexture);


                

                half4 frag(Varyings IN) : SV_TARGET
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                    float2 blockPos = floor(IN.uv * _BlockCount);
                    float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                    float4 tex = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_point_clamp, IN.uv);

                    return tex;
                }
            ENDHLSL
        }
           
    }   
}