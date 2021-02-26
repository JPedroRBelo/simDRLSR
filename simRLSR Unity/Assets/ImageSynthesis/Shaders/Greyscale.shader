// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GreyScale" {
        Properties{
              _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        }

            SubShader{
                Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
                LOD 200

                CGPROGRAM
                #pragma surface surf Lambert alpha

                    sampler2D _MainTex;

                    struct Input {
                        float2 uv_MainTex;
                    };

                    void surf(Input IN, inout SurfaceOutput o) {
                        half4 c = tex2D(_MainTex, IN.uv_MainTex);
                        o.Albedo = dot(c.rgb, float3(0.3, 0.59, 0.11));
                        o.Alpha = c.a;
                    }

                ENDCG
        }

    }