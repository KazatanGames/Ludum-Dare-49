// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Kazatan Games/Transparent Diffuse with Shadow" {
    Properties{
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }

        SubShader{
            Tags
            {
                "Queue" = "AlphaTest"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Lambert alphatest:_Cutoff addshadow

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                float4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb * c.a;
                o.Alpha = c.a;
            }
            ENDCG
    }

        Fallback "Legacy Shaders/Transparent/VertexLit"
}