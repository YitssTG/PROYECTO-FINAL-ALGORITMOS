Shader "Custom/ToonMaterialSpecular"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

        _ShadeColor ("Shade Color", Color) = (0.2,0.2,0.2,1)
        _ShadeThreshold ("Shade Threshold", Range(0,1)) = 0.5

        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _SpecIntensity ("Specular Intensity", Range(0,3)) = 1
        _SpecSize ("Specular Size", Range(1,128)) = 32

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.03)) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // ==========================
        // OUTLINE PASS
        // ==========================
        Pass
        {
            Name "Outline"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _OutlineColor;
            float _OutlineWidth;

            struct vIn { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct vOut { float4 pos : SV_POSITION; };

            vOut vert(vIn v)
            {
                vOut o;
                float3 n = normalize(v.normal);
                v.vertex.xyz += n * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(vOut i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // ==========================
        // MAIN TOON + SPECULAR PASS
        // ==========================
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            fixed4 _ShadeColor;
            float _ShadeThreshold;

            fixed4 _SpecColor;
            float _SpecIntensity;
            float _SpecSize;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir  = normalize(_WorldSpaceCameraPos - i.worldPos);

                float NdotL = saturate(dot(i.normal, lightDir));

                // TOON LIT / SHADED
                float toonStep = step(_ShadeThreshold, NdotL);

                fixed4 baseTex = tex2D(_MainTex, i.uv) * _Color;
                fixed4 toonLit = lerp(_ShadeColor * baseTex, baseTex, toonStep);

                // =======================
                // SPECULAR CARTOON
                // =======================
                float3 halfDir = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(i.normal, halfDir));

                // Exageramos la luz especular tipo anime
                float spec = pow(NdotH, _SpecSize) * _SpecIntensity;

                fixed4 specular = _SpecColor * spec;

                return toonLit + specular;
            }
            ENDCG
        }
    }
}
