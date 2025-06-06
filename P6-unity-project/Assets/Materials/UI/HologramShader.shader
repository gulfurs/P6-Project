Shader "Unlit/HologramShader"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0, 0.8, 1, 0.5) // Bright cyan with 50% transparency
        _BorderRadius ("Corner Radius", Range(0, 0.5)) = 0.1 // Corner rounding
        _ScanIntensity ("Scan Lines Intensity", Range(0, 1)) = 0.3 // How visible scan lines are
        _ScanSpeed ("Scan Speed", Range(0, 5)) = 2 // Speed of scan lines
        _ScanDensity ("Scan Line Density", Range(10, 500)) = 100 // How many scan lines
        _BlobColor ("Blob Color", Color) = (1, 1, 1, 1) // White blob by default
        _BlobSize ("Blob Size", Range(0, 1)) = 0.3 // Size of blob effect
        _BlobIntensity ("Blob Intensity", Range(0, 1)) = 0.5 // How bright the blob is
        _BlobSpeed ("Blob Speed", Range(0, 2)) = 0.5 // How fast the blob moves
    }
    
    SubShader {
        Tags { 
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _BlobColor;
            float _BorderRadius;
            float _ScanIntensity;
            float _ScanSpeed;
            float _ScanDensity;
            float _BlobSize;
            float _BlobIntensity;
            float _BlobSpeed;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            // Rounded rectangle SDF
            float roundedRect(float2 position, float2 size, float radius) {
                return length(max(abs(position) - size + radius, 0.0)) - radius;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Round corners
                float2 centered = i.uv - 0.5;
                float cornerDist = roundedRect(centered, float2(0.5, 0.5), _BorderRadius);
                clip(-cornerDist);
                
                // Base color from texture (if any) and color property
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // SCAN LINES - more pronounced
                // Create basic scan line pattern
                float scanPattern = sin(i.uv.y * _ScanDensity - _Time.y * _ScanSpeed);
                
                // Make the pattern sharper (more digital-looking)
                scanPattern = pow(abs(scanPattern), 0.25) * sign(scanPattern);
                
                // Normalize to 0-1 range and apply intensity
                float scanEffect = lerp(1.0, (scanPattern * 0.5 + 0.5), _ScanIntensity);
                
                // MOVING BLOB
                float2 blobCenter = float2(
                    0.5 + sin(_Time.y * _BlobSpeed) * 0.3,
                    0.5 + cos(_Time.y * _BlobSpeed * 0.7) * 0.3
                );
                
                float blobDistance = distance(i.uv, blobCenter);
                float blob = smoothstep(_BlobSize, 0, blobDistance) * _BlobIntensity;
                
                // Apply scan lines to color
                col.rgb *= scanEffect;
                
                // Add blob
                col.rgb += _BlobColor.rgb * blob;
                
                return col;
            }
            ENDCG
        }
    }
}