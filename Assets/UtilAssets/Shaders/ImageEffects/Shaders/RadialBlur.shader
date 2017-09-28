// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "OULImageEffect/RadialBlur"
{
	Properties
	{ 
		_MainTex("", any) = "" {} 
		//_CenterX("CenterX", Float) = 0
		//_CenterY("CenterY", Float) = 0
		//_BlurPower("BlurPower", Float) = 1
		//_ImageSize("ImageSize", Float) = 512.0
		//_NumOfSampling("NumOfSampling", Int) = 8
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Fog{ Mode Off }
			Tags
			{
				/*
				Queue
				描画のプライオリティを指定。透過がある物は特にこれを指定しないとまずいです。

				・Background
				あらゆる描画の前に描画。背景やスカイボックスなどはこれにするべき。
				・Geometry（デフォルト）
				特にプライオリティを付けない。通常描画に従う。
				・AlphaTest
				αテスト用のジオメトリとして使用。
				・Transparent
				GeometryとAlphaTestの後にBack to Front順で描画する。透過オブジェクトはこれにしないとうまく描画されない。
				・Overlay
				すべての描画の後に描画される。スクリーンに上書きする系の描画はこれで。
				*/
				"Queue" = "Geometry"

				/*
				RenderType
				SubShaderがどういう性質の描画をするかを特定のシェーダに伝える。

				・Opaque （ソリッド（不透明）です）
				・Transparent （透過してます）
				・TransparentCutout （透過してます）
				・Background （スカイボックスなどの背景描画用のシェーダです）
				・Overlay （スクリーンに上書き用のシェーダです）
				・TreeOpaque （木の幹を描画するためのシェーダです）
				・TreeTransparentCutout （木の葉を描画するためのシェーダです）
				・TreeBillboard （ビルボードな木を描画するシェーダです）
				・Grass （草を描画する為のシェーダです）
				・GrassBillboard （ビルボードな草を描画するためのシェーダです）
				*/
				"RenderType" = "Opaque"

				/*
				Projector
				投影効果（Projectors）の影響を受けるかどうかを指定（True）
				*/
				//"Projector" = "True"
			}

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

			// SetValueで設定される値
			sampler2D _MainTex;
			float _CenterX= 0;
			float _CenterY= 0;
			float _BlurPower = 0;
			//float _ImageSize = 512;
			int _NumOfSampling = 8;

			// 頂点シェーダ
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// ピクセルシェーダ
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 0;

				// 放射中心
				fixed2 ss_center = float2((_CenterX + 1.0f) * 0.5f, (-_CenterY + 1.0f) * 0.5f);

				// オフセット
				fixed2 uvOffset = (ss_center - i.uv) * (_BlurPower / 512);

				// サンプリング数の逆数 　for文で回す回数分色を減らし　完成したときに元の色にする。
				float InvSampling = 1.0f / _NumOfSampling;

				// テクスチャ座標　動かすために今のテクスチャーの場所を渡す。
				fixed2 uv = i.uv;

				// サンプリングの回数だけ実行
				for (int i = 0;i<_NumOfSampling;i++)
				{
					col += tex2D(_MainTex, uv) * InvSampling;
					uv += uvOffset;
				}

				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col;

				return col;
			}
			ENDCG
		}
	}
}
