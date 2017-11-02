Shader "OULImageEffect/Distortion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_PutTex("PutTex", 2D) = "white" {}

		_Rate("Rate", Range(0,1)) = 0.5
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		sampler2D _MainTex;		// 緑を描画した後の写真
		sampler2D _PutTex;		// 緑を描画する前の写真

		float _Rate;	// 歪ませる強さ

		fixed4 frag(v2f i) : SV_Target
		{
			fixed2 move = tex2D(_MainTex, i.uv).xy;
			move *= _Rate;

			// 色の値に応じてuvを移動させる
			float2 uv = i.uv;
			uv.x += move.x;
			uv.y += move.y;

			fixed4 col = tex2D(_PutTex, uv);
			//if (move.g > 0) col.rgb = 1 - col.rgb;

			return col;
		}
			ENDCG
		}
	}
}
