﻿Shader "Custom/Portal"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _LeftEyeTexture("Texture", 2D) = "white" {}
        _RightEyeTexture("Texture", 2D) = "white" {}
        _TextureOffset("Texture Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" 
               "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //// make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float1 eyeIndex : COLOR1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            //sampler2D _MainTex;
            float4 _TextureOffset;

            sampler2D _LeftEyeTexture;
            sampler2D _RightEyeTexture;

            v2f vert (appdata v)
            {
                 v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                //This says it should be the URP version of the above commmand, but its not recognized
                //o.vertex = TransformObjectToHClip(v.vertex);

                o.screenPos = ComputeScreenPos(o.vertex); // use the screen position coordinates of the portal to sample the render texture (which is our screen)
                o.eyeIndex = unity_StereoEyeIndex;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w; // clip space -> normalized texture (?)

                fixed4 col;
                if(i.eyeIndex == 0) {
                    col = tex2D(_LeftEyeTexture, uv);
                } 
                else {
                    col = tex2D(_RightEyeTexture, uv + _TextureOffset.xy);
                }
                return col;
            }
            ENDCG
        }
    }
}