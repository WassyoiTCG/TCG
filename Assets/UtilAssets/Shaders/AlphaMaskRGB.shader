// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
メモ

高精度: float

float は最高精度の浮動小数点値で、通常のプログラミング言語の float と同じように一般な 32 ビットです。

ワールド空間位置、テクスチャ座標、複雑な関数を含むスカラー計算 (三角法、累乗法など) には、 たいてい 32 ビットの float が使用されます。

中精度: half

half は中精度の浮動小数点値で、一般に 16 ビットです (–60000 から +60000 の範囲で、小数点以下約 3 桁)。

Half は、ショートベクトル、方向、オブジェクト空間位置、HDR カラー に使用されます。

低精度: fixed

fixed は低精度で、固定小数点数です。一般的に 11 ビットで、–2.0 から +2.0 の範囲で、1/256 精度です。

固定精度は、標準カラー (一般的に標準テクスチャに保管されるので) とそれらの単純な制御に使用されます。

*/

Shader "OUL/AlphaMaskRGB"
{
	Properties
	{
		// カラー(αいじる用)
		_Color("Main Color", Color) = (1,1,1,1)

		// メインテクスチャ
		_MainTex ("Texture", 2D) = "white" {}

		// マスクするテクスチャ
		_MaskTex("MaskTexture", 2D) = "white" {}

		// マスクするテクスチャのUV値(これをいじってスクロールする)
		_ScrollU("ScrollU", Range(0.0,1.0)) = 0.0
		_ScrollV("ScrollV", Range(0.0,1.0)) = 0.0
	}
	SubShader
	{
		Tags
		{
			/*
			Queue
			描画のプライオリティ(描画順)を指定。透過がある物は特にこれを指定しないとまずいです。

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
			"Queue" = "Transparent"

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
			"RenderType" = "Transparent"

			/*
			Projector
			投影効果（Projectors）の影響を受けるかどうかを指定（True）
			*/
			//"Projector" = "True"

			/*
			IgnoreProjector
			もし IgnoreProjector が使用されていて、値が “True” である場合、このシェーダーを使用するオブジェクトは プロジェクター により影響されません。これは、Projectors が影響するためのよい方法がないため、部分的に透過であるオブジェクトで便利です。
			*/
			"IgnoreProjector" = "True"
		}

		// Zバッファ書き込みオフ(これをオンにすると透明部分より後ろが描画されないため、不自然になるらしい)
		ZWrite Off

		// ライティング影響なし
		Lighting Off

		// フォグ影響なし
		Fog{ Mode Off }

		/*
		次が最も一般的なブレンディングのタイプです。
		Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
		Blend One OneMinusSrcAlpha		// Premultiplied transparency
		Blend One One					// Additive
		Blend OneMinusDstColor One		// Soft Additive
		Blend DstColor Zero				// Multiplicative
		Blend DstColor SrcColor			// 2x Multiplicative
		*/
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
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;	// TRANSFORM_TEXで使用される(テクスチャの名前+_ST)で検索するみたい#define TRANSFORM_TEX(tex, name) (tex.xy * name##_ST.xy + name##_ST.zw)

			// SetValueで設定される値
			sampler2D _MaskTex;
			float _ScrollU;
			float _ScrollV;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 scroll = float2(_ScrollU, _ScrollV);/* * _Time.y*/

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				half4 mask = tex2D(_MaskTex, i.uv + scroll);

				// マスク用のテクスチャの色を乗算してアルファ値を決定(黒に近ければ黒)
				col.rgb *= mask.x;
				col.w = 1;
				return col;
			}
			ENDCG
		}
	}
}
