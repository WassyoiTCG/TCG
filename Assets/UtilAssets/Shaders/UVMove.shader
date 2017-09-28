Shader "OUL/UVMove"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScrollU("ScrollU", Range(0,1)) = 0
		_ScrollV("ScrollV", Range(0,1)) = 0
		_Color("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		ZWrite Off
		Lighting Off
		Fog{ Mode Off }

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

			float _ScrollU;
			float _ScrollV;

			// SetValueで設定される値
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//// 放射中心
				//half2 ss_center = float2(0.5f, 0.5f);

				//// 向きベクトル
				//half2 vec = ss_center - i.uv;

				//// ベクトル正規化
				//vec = normalize(vec);

				//// 移動量をベクトルにかける
				//vec *= _MoveUV;

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv + float2(_ScrollU, _ScrollV)) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
