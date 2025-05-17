/*=====
<Player.cs>
└作成者：yamamoto

＞内容
Playerの挙動を管理するスクリプト

＞注意事項


＞更新履歴
Y25   
_M04    
__D     
___11:プログラム作成:yamamoto   
___12:スコアデバック用のプログラムを追加:yamamoto
___22:移動の仕様変更:yamamoto
___27:プレイヤーの移動をADキーのみに変更:mori
_M05
___01:速度にあわせて重力を増加する処理を追加:tooyama
___09:不必要な引数、変数宣言を削除:yamamoto
___11:バウンド防止処理を追加:tooyama
___14:エネミー分割処理呼び出しを追加:mori
___16:リファクタリング:yamamoto
___17:坂の角度に応じた加減速処理の追加:tooyama
=====*/

using UnityEngine;

public class Player : MonoBehaviour
{
    // 変数宣言
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("加速量")] private float m_fBoost;
/*
    [Header("デバッグ")]
    [SerializeField, Tooltip("デバッグ表示")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("デバッグプレハブ取得")] private GameObject debugPrefab;
*/
    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float m_fAddGravity = 3.0f;

    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // プレイヤーの物理挙動を制御するためのRigidbody
    private int nEnemyKillCount = 0; // 倒した敵の数
    private int m_nPrevSlopeAngleKey = int.MinValue; // 前フレームで適用された傾斜角（10度単位）
    private float m_fRecordedBaseSpeed = 0.0f; // 傾斜に入った瞬間の速度記録用

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        mainCamera = UnityEngine.Camera.main;
        rb = GetComponent<Rigidbody>();  // Rigidbodyの取得
                                         
        m_fRecordedBaseSpeed = m_fSpeed; // 傾斜に入った瞬間の速度記録と初期速度を同期させる

    }

    /*＞FixedUpdate関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:一定間隔で更新
    */
    void FixedUpdate()
    {
        // 向いている方向に進み続ける
        rb.linearVelocity = new Vector3(
            transform.forward.x * m_fSpeed,
            rb.linearVelocity.y,
            transform.forward.z * m_fSpeed
            );

        // Y座標に制限を掛ける
        ClampPlayerHeight();

        // 重力の追加
        rb.AddForce(Vector3.down * m_fBaseGravity, ForceMode.Acceleration);

        // 坂の角度の更新
        float slope = GetGroundSlope();

        // 角度を丸める(状態遷移の検出用)
        int slopeKey = Mathf.RoundToInt(slope / 10.0f) * 10;

        // 地面に立っており、傾斜に入った場合
        if (slope != -1.0f && slopeKey != m_nPrevSlopeAngleKey)
        {
            // 初めて傾斜に入ったときだけ速度を記録
            if (m_nPrevSlopeAngleKey == 0)
                m_fRecordedBaseSpeed = m_fSpeed;

            // 坂の角度から加減速値を決める
            float boost = ApplySlopeSpeedBoost(slope);
            // ApplySlopeSpeedBoost関数の戻り値を加減速に行う
            AddBoost(boost);
            //本フレームの傾斜角を保存し、２度目の加減速を防ぐ
            m_nPrevSlopeAngleKey = slopeKey;
        }

    }

    /*＞Update関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:更新関数
    */

    private void Update()
    {
        //////////////////////////////////////////////////////////
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_fSpeed += m_fBoost; // 加速デバッグ用
            m_fRecordedBaseSpeed += m_fBoost;
            AddGravity();
        }
        ////////////////////////////////////////////////////

        rotation();
    }

    /*＞回転関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:プレイヤーの向きを回転させる
    */
    private void rotation()
    {
        float rotateSpeed = 100.0f; // 回転速度

        float turn = 0.0f;

        if (Input.GetKey(KeyCode.A)) turn = -1.0f; // 左回転
        if (Input.GetKey(KeyCode.D)) turn = 1.0f;  // 右回転

        if (turn != 0.0f)
        {
            // Y軸を中心に回転させる
            transform.Rotate(0.0f, turn * rotateSpeed * Time.deltaTime, 0.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die(mainCamera); // エネミー分割処理
                AddBoost(m_fBoost);
                m_fRecordedBaseSpeed += m_fBoost;
                AddGravity();
                nEnemyKillCount++; // キルカウントの増加
            }
        }
    }

    /*＞加速度増加関数
   引数：float _boost:増加する値
   ｘ
   戻値：なし
   ｘ
   概要:プレイヤーの速度をあげる
   */
    public void AddBoost(float _boost)
    {
        m_fSpeed += _boost;
    }

    /*＞重力増加関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:加速度増加に合わせて重力を増加させる
    */
    private void AddGravity()
    {
        m_fBaseGravity += m_fAddGravity; // 重力の増加
        m_fBaseGravity = Mathf.Min(m_fBaseGravity, 40.0f); // 上限(40.0f)を超えないように設定
    }

    /*＞高度制限関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要: プレイヤーがでこぼこした地形で跳ねるように見えてしまう問題を防ぐため、
          プレイヤーのY座標（高さ）に上限を設けて、地面にすいつくように移動させる
    */
    private void ClampPlayerHeight()
    {
        float rayStartOffsetY = 0.1f; // 地面とのめり込みを防ぐため、Raycastの始点を少し上にずらす
        Vector3 rayOrigin = transform.position + Vector3.up * rayStartOffsetY; // 少し上からRayを発射
        RaycastHit hit; // 地面との当たり判定用

        // 地面に立っていた(足元の地面にRayがヒットした)場合のみ処理を行う
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10.0f))
        {
            
            float groundY = hit.point.y; //地面の高さ
            float maxHeight = groundY + 1.0f;   // 許容する最大の高さ（浮き防止）

            // プレイヤーが指定した高さより浮いている場合は制限をかける
            if (transform.position.y > maxHeight)
            {
                Debug.Log("制限");
                // Y座標に制限を掛けて高さを矯正する
                Vector3 correctedPos = transform.position;
                correctedPos.y = maxHeight;
                transform.position = correctedPos;

                // 上昇中のY速度も0に抑える
                Vector3 velocity = rb.linearVelocity;
                velocity.y =-3.0f;
                rb.linearVelocity = velocity;
            }
        }

    }

    public void DebugMode(DebugMode _debug)
    {
        _debug.UpdateDebugUI(transform, m_fSpeed, nEnemyKillCount); // デバッグUIの更新
    }

    /*＞角度取得関数
    引数：なし
    ｘ
    戻値：坂の角度
    ｘ
    概要: プレイヤーが立っている坂の角度を取得する
          この符号付き角度は ApplySlopeSpeedBoost() で
          10°単位に丸められ、速度補正テーブルに渡される
    */
    private float GetGroundSlope()
    {
        float rayLength = 2.0f; // Raycast 距離（足元判定用）
        Vector3 origin = transform.position; // Ray 生成位置

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            // プレイヤー進行方向と斜面方向を求める
            Vector3 moveDir = rb.linearVelocity.normalized; // 進行方向
            Vector3 slopeDir = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal).normalized; // 斜面方向

            //  傾斜角の絶対値を求める
            float angleAbs = Vector3.Angle(hit.normal, Vector3.up);

            // 登りか下りかを内積で判定し符号を付与
            float dot = Vector3.Dot(moveDir, slopeDir);
            float signedAngle = (dot >= 0) ? angleAbs  // moveDir と同じ向き → 下り
                                           : -angleAbs; // 逆向き → 上り

            return signedAngle; // ここで返した角度を ApplySlopeSpeedBoost関数で使用する
        }
        else
        {
            return -1.0f; // 空中に浮いており、地面が見つからない場合は-1を返し判定を行わない
        }
    }

    /*＞坂の傾斜角による加速・減速処理関数
   引数：傾斜角
   ｘ
   戻値：加速度パラメータ
   ｘ
   概要:坂の傾斜角に応じてプレイヤー速度を増減させる
   */
    private float ApplySlopeSpeedBoost(float _slopeAngle)
    {
        // 地面が検出されなかった
        if (_slopeAngle == -1.0f) return 0.0f;

        // 角度を丸める(ロジック計算用)
        int slopeKey = Mathf.RoundToInt(_slopeAngle / 10.0f) * 10;

        // 30度を超えた傾斜角は30度とする
        if (slopeKey < -30.0f) slopeKey = -30;
        else if (slopeKey > 30.0f) slopeKey = 30;

        // 傾斜角に応じて加速・減速する値を決める
        switch (slopeKey)
        {
            case -30:
                return 2.0f;
            case -20:
                return 1.5f;
            case -10:
                return 1.0f;
            case 0:
                return m_fRecordedBaseSpeed - m_fSpeed; // 平地に戻る際、元の速度に戻す
            case 10:
                return -1.0f;
            case 20:
                return -1.5f;
            case 30:
                return -2.0f;
            default: return 0.0f;

        }
    }

}