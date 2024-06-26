﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/additiveTex_2" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_Intensity ("Intensity", Float) = 1.0
		_MainTexture ("Base (RGB) Alpha(A)", 2D) = "white" {}
		_Mask    ("Mask (ARGB or Grayscale)", 2D) = "white" {}
		_speed("speed",float)=5
	}
	
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			
	    Blend SrcAlpha One
	    AlphaTest Greater .01
	    ColorMask RGB
	    Cull Off
	    Lighting Off
	    ZWrite Off
	    Fog { Color (0,0,0,0) }
		BindChannels {
	        Bind "Color", color
	        Bind "Vertex", vertex
	        Bind "TexCoord", texcoord
    	}

		SubShader {
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				fixed4 _TintColor;
				float  _Intensity;
				sampler2D _MainTexture;
				sampler2D _Mask;
				float _speed;
				float4 _MainTexture_ST;
				float4 _Mask_ST;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
				};

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTexture);
					o.texcoord2 = TRANSFORM_TEX(v.texcoord2,_Mask);
					return o;
				}
					
				fixed4 frag (v2f i) : COLOR
				{
					i.texcoord.x+=_Time*_speed;
					//float4 (r,g,b,a);
					//return 2.0f * i.color * tex2D(_MainTexture, i.texcoord);
					half4 c = i.color * _TintColor * tex2D(_MainTexture, i.texcoord);
					half4 mask = tex2D(_Mask, i.texcoord2);
					c *= mask.a;
					return _Intensity * c;					
				}
				ENDCG 
			}
		}
	} 
	FallBack "Diffuse"
}
