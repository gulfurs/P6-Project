Shader "Unlit/ScanlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.5)
        _ScanlineSpeed ("Scanline Speed", Range(0, 10)) = 1
        _ScanlineWidth ("Scanline Width", Range(0, 100)) = 10
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _NoiseScale ("Noise Scale", Range(0, 100)) = 10
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 1
        // New blob properties
        _BlobColor ("Blob Color", Color) = (1, 1, 1, 1) // White blob by default
        _BlobSize ("Blob Size", Range(0, 1)) = 0.3 // Size of blob effect
        _BlobIntensity ("Blob Intensity", Range(0, 1)) = 0.5 // How bright the blob is
        _BlobSpeed ("Blob Speed", Range(0, 2)) = 0.5 // How fast the blob moves
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _ScanlineSpeed;
            float _ScanlineWidth;
            float _ScanlineIntensity;
            float _NoiseScale;
            float _NoiseIntensity;
            float _NoiseSpeed;
            // New blob properties
            float4 _BlobColor;
            float _BlobSize;
            float _BlobIntensity;
            float _BlobSpeed;
            
            // Simple noise function
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Generate scanlines
                float scanline = sin((i.uv.y * _ScanlineWidth) + (_Time.y * _ScanlineSpeed)) * 0.5 + 0.5;
                scanline = pow(scanline, 2.0); // Make the scanlines sharper
                
                // Generate noise
                float2 noiseUV = i.uv * _NoiseScale;
                noiseUV.y += _Time.y * _NoiseSpeed;
                float noiseValue = noise(noiseUV) * _NoiseIntensity;
                
                // Apply scanlines and noise
                col.rgb = col.rgb * (1.0 - (_ScanlineIntensity * scanline));
                col.rgb += noiseValue;
                
                // MOVING BLOB (added to existing shader)
                float2 blobCenter = float2(
                    0.5 + sin(_Time.y * _BlobSpeed) * 0.3,
                    0.5 + cos(_Time.y * _BlobSpeed * 0.7) * 0.3
                );
                
                float blobDistance = distance(i.uv, blobCenter);
                float blob = smoothstep(_BlobSize, 0, blobDistance) * _BlobIntensity;
                
                // Add blob to final color
                col.rgb += _BlobColor.rgb * blob;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}