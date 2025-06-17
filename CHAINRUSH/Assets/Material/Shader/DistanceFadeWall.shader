Shader "Custom/DistanceFadeWall"
{
    Properties
    {
        // 表示する基本テクスチャ
        _MainTex("Main Texture", 2D) = "white" {}
        // プレイヤーのワールド座標（スクリプトからセットする）
        _PlayerPos("Player Position", Vector) = (0,0,0,0)
        // テクスチャが表示され始める距離
        _Radius("Visible Radius", Float) = 3.0

        //X方向のシフトとスピードに関するパラメータを追加
		_XShift("Xuv Shift", Range(-1.0, 1.0)) = 0.1
		_XSpeed("X Scroll Speed", Range(1.0, 100.0)) = 10.0

		//Y方向のシフトとスピードに関するパラメータを追加
		_YShift("Yuv Shift", Range(-1.0, 1.0)) = 0.1
		_YSpeed("Y Scroll Speed", Range(1.0, 100.0)) = 10.0
    }

    SubShader
    {
        // 描画順：通常オブジェクトの少し後（スカイボックスより前にするなら Background+1 などに変更可）
        Tags { "Queue" = "Geometry+1" "RenderType" = "Transparent" }

        // 深度バッファへは書き込まない（透明処理のため）
        ZWrite Off

        // アルファブレンド：透明処理
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // プロパティ
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _PlayerPos;
            float _Radius;

			float _XShift;
			float _YShift;
			float _XSpeed;
			float _YSpeed;

            // 頂点入力構造体
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

             // 頂点→フラグメントに渡すデータ
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;    // ワールド座標
            };

            v2f vert(appdata v)
            {
                v2f o;
                 // ワールド→クリップ座標変換
                o.vertex = UnityObjectToClipPos(v.vertex);
                // UV変換
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                 // ワールド座標を取得
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
               
                return o;
            }

            // フラグメントシェーダー
            fixed4 frag(v2f i) : SV_Target
            {
                 // プレイヤーとの距離を計算
                float dist = distance(i.worldPos, _PlayerPos);

                // 距離に応じてアルファを決める（近いほど1.0、不透明）
                float alpha = smoothstep(_Radius * 0.2f, _Radius * 2.0f, dist);
                alpha = 1.0 - alpha;// 近いほど alpha=1、不透明に

                // 時間に応じたUVスクロール（速度×方向）
			    _XShift = _XShift * _XSpeed;
			    _YShift = _YShift * _YSpeed;
			   
			    i.uv.x = i.uv.x + _XShift * _Time;
			    i.uv.y = i.uv.y + _YShift * _Time;

                 // テクスチャ取得＋アルファ適用
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}
