// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NewUnlitShader"
{
     Properties {
            _Tex0 ("Tex 0", 2D) = "white" {}
            _Tex1 ("Tex 1", 2D) = "white" {}
            _Tex2 ("Tex 2", 2D) = "white" {}
            _Tex3 ("Tex 3", 2D) = "white" {}
            _Tex4 ("Tex 4", 2D) = "white" {}
           
            _Blend0to1and1to2 ("Blend between 0 and 1, 1 and 2", Vector) = (0,1,2,3)
            _Blend2to3and3to4 ("Blend between 2 and 3, 3 and 4", Vector) = (0,1,2,3)
           
        }
        SubShader {
            Lighting Off
            Fog { Mode Off }
            Pass {
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #include "UnityCG.cginc"
                    #pragma target 3.0
                    sampler2D _Tex0;
                    sampler2D _Tex1;
                    sampler2D _Tex2;
                    sampler2D _Tex3;
                    sampler2D _Tex4;
                    float4 _Blend0to1and1to2;
                    float4 _Blend2to3and3to4;
                    uniform float4 _Tex0_ST;
           
                    struct v2f {
                        float4 pos : SV_POSITION;
                        float2 uv : TEXCOORD0;
                        float4 col : COLOR;
                    };
                   
                    v2f vert (appdata_base vInput) {
                        v2f OUT;
                        OUT.pos = UnityObjectToClipPos (vInput.vertex);
                        OUT.uv = TRANSFORM_TEX(vInput.texcoord, _Tex0);
                        OUT.col = length(vInput.vertex);
                        return OUT;
                    }
                   
                    half4 frag (v2f fInput) : COLOR {
                        half4 c0 = tex2D (_Tex0, fInput.uv);
                        half4 c1 = tex2D (_Tex1, fInput.uv);
                        half4 c2 = tex2D (_Tex2, fInput.uv);
                        half4 c3 = tex2D (_Tex3, fInput.uv);
                        half4 c4 = tex2D (_Tex4, fInput.uv);
     
                        if (fInput.col.x < _Blend0to1and1to2.x) return c0;
                        if (fInput.col.x > _Blend0to1and1to2.x  fInput.col.x < _Blend0to1and1to2.y) return lerp(c0,c1,((fInput.col.x - _Blend0to1and1to2.x)/(_Blend0to1and1to2.y-_Blend0to1and1to2.x)));
                        if (fInput.col.x > _Blend0to1and1to2.y  fInput.col.x < _Blend0to1and1to2.z) return c1;
                        if (fInput.col.x > _Blend0to1and1to2.z  fInput.col.x < _Blend0to1and1to2.w) return lerp(c1,c2,((fInput.col.x - _Blend0to1and1to2.z)/(_Blend0to1and1to2.w-_Blend0to1and1to2.z)));
                        if (fInput.col.x > _Blend0to1and1to2.w  fInput.col.x < _Blend2to3and3to4.x) return c2;
                        if (fInput.col.x > _Blend2to3and3to4.x  fInput.col.x < _Blend2to3and3to4.y) return lerp(c2,c3,((fInput.col.x - _Blend2to3and3to4.x)/(_Blend2to3and3to4.y-_Blend2to3and3to4.x)));
                        if (fInput.col.x > _Blend2to3and3to4.y  fInput.col.x < _Blend2to3and3to4.z) return c3;
                        if (fInput.col.x > _Blend2to3and3to4.z  fInput.col.x < _Blend2to3and3to4.w) return lerp(c3,c4,((fInput.col.x - _Blend2to3and3to4.z)/(_Blend2to3and3to4.w-_Blend2to3and3to4.z)));
                        return c4;
                       
     
                    }
                ENDCG
            }
        }
    }
