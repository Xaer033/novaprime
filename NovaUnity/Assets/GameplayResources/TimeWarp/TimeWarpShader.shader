﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TimeWarpShader"
{
    Properties
    {
        _position ("Position",  vector) = (0, 0, 0, 0)
        _radius ("Radius",  Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off
        ZWrite Off
        LOD 100

// Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 localPos : TEXCOORD1;
                float4 objPos: TEXCOORD2;
                float3 normal: TEXCOORD3;
                float3 viewDir:TEXCOORD4;
                float4 pos : SV_POSITION;
            };

            float4 _position;
            float _radius;
            sampler2D _BackgroundTexture;
            
            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.viewDir = mul(unity_ObjectToWorld,ObjSpaceViewDir(v.vertex));    
                o.grabPos = ComputeGrabScreenPos(o.pos);
                
                o.objPos = mul(unity_ObjectToWorld, v.vertex);
                o.objPos.z = 0;
                
                o.localPos =  mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                o.localPos.z = 0;
//                o.objPos = ComputeScreenPos(v.vertex);
//                o.localPos =ComputeScreenPos(float4(0, 0, 0, 1));
                return o;
            }


            half4 frag(v2f i) : SV_Target
            {
                half4 diff = (i.localPos - i.objPos);
                float diffLength = length(diff);
                
                float range = _radius /  (diffLength * diffLength * 8);
                
                float4 warp = float4(cos(_Time.y + _Time.x), sin(_Time.x), 0, 0);
                warp = (warp) * 0.4;
                
                float4 offset = diff * range * 1;

                float4 uv = i.grabPos - (offset + warp);
                half4 bgcolor = tex2Dproj(_BackgroundTexture, uv);
                
                float fresnel = saturate(dot(normalize(i.normal), normalize(i.viewDir)));
                
                half4 tintColor = float4(0.2, 1.0, 0.2, 1);
                half4 colorFactor = lerp(tintColor * 10, float4(1, 1, 1, 1), fresnel) * tintColor ;
                
//                return (diff);
                return bgcolor * colorFactor;
            }
            ENDCG
        }
    }
}
