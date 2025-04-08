Shader "Custom/BeconLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}         // The main texture of the beam.
        _Color ("Color", Color) = (1,1,1,0.2)           // Base color with alpha (0.5 makes it semi-transparent)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1) // Emissive color for glow.
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.0  // Multiplies emission strength.
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // This blend mode supports transparency.
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _EmissionColor;
            float _GlowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture.
                fixed4 texCol = tex2D(_MainTex, i.uv);
                // Multiply texture and base color. The alpha of _Color controls overall transparency.
                fixed4 col = texCol * _Color;
                // Add emission (glow). Adjust _GlowIntensity for more or less glow.
                col.rgb += _EmissionColor.rgb * _GlowIntensity;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
