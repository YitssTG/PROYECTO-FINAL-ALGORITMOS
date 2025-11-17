Shader "Custom/TwoTextureEmissionBlend"
{
    Properties
    {
        _MainTex("Texture A", 2D) = "white" {}
        _SecondTex("Texture B", 2D) = "white" {}
        
        _Blend("Blend", Range(0,1)) = 0.5

        _EmissionA("Emission A", Range(0,5)) = 1
        _EmissionB("Emission B", Range(0,5)) = 1
    }

    SubShader
    {
        Tags{
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SecondTex;

            float4 _MainTex_ST;
            float4 _SecondTex_ST;

            float _Blend;
            float _EmissionA;
            float _EmissionB;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uvA : TEXCOORD0;
                float2 uvB : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvA = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvB = TRANSFORM_TEX(v.uv, _SecondTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 colA = tex2D(_MainTex, i.uvA) * _EmissionA;
                fixed4 colB = tex2D(_SecondTex, i.uvB) * _EmissionB;

                // Mezcla pura sin perder brillo
                fixed4 finalColor = lerp(colA, colB, _Blend);
                return finalColor;
            }

            ENDCG
        }
    }
}
