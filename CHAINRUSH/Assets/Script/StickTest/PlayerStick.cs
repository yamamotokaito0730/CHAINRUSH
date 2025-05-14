// 処理1
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

=====*/

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerStick : MonoBehaviour
{
    // 変数宣言
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("加速量")] private float m_fBoost;

    [Header("デバッグ")]
    [SerializeField, Tooltip("デバッグ表示")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("デバッグプレハブ取得")] private GameObject debugPrefab;

    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float m_fAddGravity = 3.0f;


    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // プレイヤーの物理挙動を制御するためのRigidbody
    private DebugMode debugModeInstance; // デバッグUI（速度・傾斜など）の表示管理用インスタンス
    private int nEnemyKillCount = 0; // 倒した敵の数

    //===============================
    ///坂の角度による加減速用処理
    //private Vector3 moveDir = Vector3.forward; // 現在の進行方向を保持する為の変数
    //
    //// 傾斜角による速度変化
    //private Dictionary<int, float> slopeSpeedTable = new Dictionary<int, float>()
    //{
    //    {-30, 2.0f}, {-20, 1.5f}, {-10, 1.0f}, {0, 0.0f}, {10, -1.0f}, {20, -1.5f}, {30, -2.0f}
    //};
    //private int lastSlopeKey = int.MinValue;
    //===============================



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

        // 初期状態でデバッグ表示ONなら、UIを生成しておく
        if (m_bDebugView && debugModeInstance == null)
        {
            GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity); // デバッグUIの生成
            debugModeInstance = obj.GetComponent<DebugMode>(); // DebugModeの取得
        }
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

        //        UpdateSlopeSpeed();
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
            AddGravity();
        }
        // デバッグUI表示
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            m_bDebugView = !m_bDebugView; // UIの表示非表示切り替え
            if (m_bDebugView && debugModeInstance == null)
            {
                // プレハブからインスタンスを生成し、DebugModeを取得
                GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity); // 座標・回転はプレハブ側で設定d
                debugModeInstance = obj.GetComponent<DebugMode>();
            }
            else if (!m_bDebugView && debugModeInstance != null)
            {
                Destroy(debugModeInstance.gameObject); // UIを非表示(削除)する
                debugModeInstance = null;
            }
        }

        if (debugModeInstance != null)
            debugModeInstance.UpdateDebugUI(transform, m_fSpeed, nEnemyKillCount); // デバッグUIの更新

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
            EnemyStick enemy = collision.gameObject.GetComponent<EnemyStick>();
            if (enemy != null)
            {
                enemy.Die(mainCamera); // エネミー分割処理
                AddBoost(m_fBoost);
                AddGravity();
                nEnemyKillCount++; // キルカウントの増加
                UnityEngine.Debug.Log("atari");
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
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2.0f))
        {
            float groundY = hit.point.y; //地面の高さ
            float maxHeight = groundY + 1.1f;   // 許容する最大の高さ（浮き防止）

            // プレイヤーが指定した高さより浮いている場合は制限をかける
            if (transform.position.y > maxHeight)
            {
                // Y座標に制限を掛けて高さを矯正する
                Vector3 correctedPos = transform.position;
                correctedPos.y = maxHeight;
                transform.position = correctedPos;

                // 上昇中のY速度も0に抑える
                Vector3 velocity = rb.linearVelocity;
                velocity.y = 0.0f;
                rb.linearVelocity = velocity;
            }
        }

    }

    ////////////////////////////
    //速度変化
    /////*＞角度取得関数
    ////引数：なし
    ////ｘ
    ////戻値：坂の角度
    ////ｘ
    ////概要:プレイヤーが立っている坂の角度を取得する
    ////*/
    //private float GetGroundSlope()
    //{
    //    float rayLength = 2.0f;
    //    Vector3 origin = transform.position;

    //    RaycastHit hit;
    //    if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
    //    {
    //        // 登り or 下りの向きを考慮して傾斜角に符号を付ける
    //        Vector3 moveDir = rb.linearVelocity.normalized;
    //        Vector3 slopeDir = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal).normalized;
    //        float dot = Vector3.Dot(moveDir, slopeDir);
    //        float angle = Vector3.Angle(hit.normal, Vector3.up);
    //        return dot >= 0 ? angle : -angle;
    //    }
    //    else
    //    {
    //        return -1f; // 地面が見つからなかった
    //    }
    //}

    ///*＞速度変化関数
    //引数：なし
    //ｘ
    //戻値：なし
    //ｘ
    //概要:坂の角度によってプレイヤー速度を増減させる
    //*/
    //private void UpdateSlopeSpeed()
    //{
    //    float slope = GetGroundSlope();
    //    if (slope == -1.0f || slope < -30 || slope > 30) return;

    //    int rounded = Mathf.RoundToInt(slope / 10.0f) * 10;
    //    if (rounded != lastSlopeKey && slopeSpeedTable.ContainsKey(rounded))
    //    {
    //        float boost = slopeSpeedTable[rounded];
    //        m_fSpeed += boost;
    //        lastSlopeKey = rounded;
    //        Debug.Log($"傾斜: {rounded}° → 速度変化 {boost}（現在速度: {m_fSpeed}）");
    //    }
    //}
    /////////////////////////////////
}