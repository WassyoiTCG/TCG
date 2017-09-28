// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "OUL/Grid"
{
	Properties
	{
		_Color0 ("Color0", Color) = (1,1,1,1)
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Period ("Period", Float) = 1	// 波の周期
		_Width ("Width", Float) = 1		// 波の1になる部分の幅
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldPosition : TEXCOORD1;
				//fixed4 color : COLOR;
			};

			fixed4 _Color0;
			fixed4 _Color1;
			float _Period;
			float _Width;

			// 矩形波
			fixed3 SquareWave(float3 worldPos, float period, float width)
			{
				// 矩形波の周期(0～_Period)のおいて、各座標がどの周期(0～_Period)にいるのか計算している？
				float modX = abs(fmod(worldPos.x, period));
				float modY = abs(fmod(worldPos.y, period));
				float modZ = abs(fmod(worldPos.z, period));

				float minBorder = width * 0.5f;
				float maxBorder = period - minBorder;

				//fixed factorX = abs(dot(worldNormal, ))

				// 
				fixed x = max(-1 * sign(minBorder - modX) * sign(modX - maxBorder), 0);
				fixed y = max(-1 * sign(minBorder - modY) * sign(modY - maxBorder), 0);
				fixed z = max(-1 * sign(minBorder - modZ) * sign(modZ - maxBorder), 0);

				// saturate(x) = max(0, min(x, 1));	用は0～1のClamp。PowerVR系に関してのみhalfやfloatだとmax(0, min(x, 1))の用が有利
				fixed v = saturate(x + y + z);

				return fixed3(v, v, v);
			}

			// ノコギリ波
			fixed3 SawtoothWave(float3 worldPos, float period, float fade)
			{
				// カメラまでの距離
				float distFromCamera = length(worldPos - _WorldSpaceCameraPos);

				float x = distFromCamera / period - _Time.y / 4;
				float sawtooth = (x - floor(x)) * period;

				// float型なのでsaturateを使わずに
				fixed v = max(min((sawtooth - (period - fade)) / fade / (1 + distFromCamera), 1), 0);
				return fixed3(v, v, v);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				fixed3 thick = SquareWave(i.worldPosition, 1.0f, 0.04f);
				fixed3 thin = SquareWave(i.worldPosition, 0.2f, 0.02f);
				fixed3 strength = SawtoothWave(i.worldPosition, 8.0f, 7.0f);
				fixed3 ruleResult = _Color0 * (4.0f * thick * thick + 4.0f * thin * thin) * strength;
				fixed3 radarResult = _Color1 * SawtoothWave(i.worldPosition, 8.0f, 1.0f);
				col.rgb = ruleResult * radarResult;
				col.a = 1;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
