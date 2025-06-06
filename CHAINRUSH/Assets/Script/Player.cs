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
___23:読み取り専用プロパティの追加:tooyama
_M06
___06:坂をスムーズに昇り降り出来る処理の追加
=====*/

using System.Data;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum E_State
    {
        Danger=1,
        Normal,
        TreeDestroy,
        HomeDestroy,
        Strongest
    }

    // 変数宣言
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("加速量")] private float m_fBoost;
    [SerializeField, Tooltip("最高速度")] private float m_fMaxSpeed;
    private E_State PlayerState;
    private int[] thresholds = { 4, 17, 35, 44, 50 };

    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float m_fAddGravity = 3.0f;

    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // プレイヤーの物理挙動を制御するためのRigidbody
    private int nEnemyKillCount = 0; // 倒した敵の数
    private int m_nPrevSlopeAngleKey = int.MinValue; // 前フレームで適用された傾斜角（10度単位）
    private float m_fRecordedBaseSpeed = 0.0f; // 傾斜に入った瞬間の速度記録用

    [SerializeField] private Animator Player_Animator;

    // 読み取り専用プロパティを追加(ShotWebクラスで発射する糸の速度に乗算させる為)
    public float PlayerSpeed => m_fSpeed;

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

        PlayerState=E_State.Normal;

        Player_Animator = GetComponent<Animator>();

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

        // 坂の角度の更新
        float slope = GetGroundSlope();

        // 角度を丸める(状態遷移の検出用)
        int slopeKey = Mathf.RoundToInt(slope / 10.0f) * 10;

        //// 地面に立っており、傾斜に入った場合
        //if (slope != -1.0f && slopeKey != m_nPrevSlopeAngleKey)
        //{
        //    // 初めて傾斜に入ったときだけ速度を記録
        //    if (m_nPrevSlopeAngleKey == 0)
        //        m_fRecordedBaseSpeed = m_fSpeed;

        //    // 坂の角度から加減速値を決める
        //    float boost = ApplySlopeSpeedBoost(slope);
        //    // ApplySlopeSpeedBoost関数の戻り値を加減速に行う
        //    AddBoost(boost);
        //    //本フレームの傾斜角を保存し、２度目の加減速を防ぐ
        //    m_nPrevSlopeAngleKey = slopeKey;
        //}

        //===== ProjectOnPlane関数を使い斜面の補正を行う
        const float rayLen = 100.0f;
        const float radius = 10.4f;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float range = 0.5f; // 許容範囲
                            //        float frontAngle= 45.0f; // 許容範囲


        Debug.DrawRay(origin, direction * rayLen, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLen))
        {

            if (range > Vector3.Distance(transform.position, hit.point))
            {
                //Debug.Log("地についている");
                // 地形ベクトル、内積を使い、プレイヤーの移動ベクトルを補正する
                Vector3 groundN = hit.normal;
                Vector3 moveDir = rb.linearVelocity.sqrMagnitude > 0.01f
                                      ? rb.linearVelocity.normalized
                                      : transform.forward;

                Vector3 slopeDir = Vector3.ProjectOnPlane(moveDir, groundN).normalized;

                Vector3 newVel = slopeDir * m_fSpeed;   // XZ を置き換え
                newVel.y = rb.linearVelocity.y;   // Y は重力分を維持
                rb.linearVelocity = newVel; //
            }
            else
            {
                // 下りで適用する処理
                Debug.Log("浮いている");
                // 重力の追加
                //    rb.AddForce(Vector3.down * m_fBaseGravity * 10.0f, ForceMode.Acceleration);
                Vector3 corrected = transform.position;
                corrected.y = Mathf.Lerp(transform.position.y, hit.point.y, Time.fixedDeltaTime * 20.0f);
                rb.MovePosition(corrected);
            }
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
            AddBoost(m_fBoost); // 加速デバッグ用
            m_fRecordedBaseSpeed += m_fBoost;
            AddGravity();
        }
        ////////////////////////////////////////////////////

        rotation();
        ChangeAnimation();
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
        if (m_fSpeed > m_fMaxSpeed)
        {
            m_fSpeed = m_fMaxSpeed;
            PlayerState = (E_State)5;
        }
        else
        {
            StateCheck();
        }
    }

    /*＞減速関数
   引数：float _down:減少する値
   ｘ
   戻値：なし
   ｘ
   概要:プレイヤーの速度を下げる
   */
    public void SubSpeed(float _down)
    {
        m_fSpeed += _down;
        StateCheck();
    }

    /*＞状態変化関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:プレイヤーの速度によって状態を変える
    */
    private void StateCheck()
    {
        for (int i = 0; i<thresholds.Length; i++)
        {
            if (m_fSpeed <= thresholds[i])
            {
                PlayerState = (E_State)i + 1;
                break;
            }
        }
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
    //    m_fBaseGravity = Mathf.Min(m_fBaseGravity, 40.0f); // 上限(40.0f)を超えないように設定
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
        float rayLength = 10.0f;
        float radius = 0.4f;          // ← 地形サイズに合わせて調整（0.2〜0.5 m が目安）

        RaycastHit hit;
        // 地面の法線をスフィアキャストで取得
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            Vector3 onNormal = hit.normal; // 斜面の法線
                                           // 現在の進行方向（停止中は forward 扱い）
            Vector3 inputVector = rb.linearVelocity.sqrMagnitude > 0.01f
                                ? rb.linearVelocity.normalized
                                : transform.forward;

            // 斜面面上の単位ベクトル
            Vector3 onPlane = Vector3.ProjectOnPlane(inputVector, onNormal).normalized;

            float angleAbs = Vector3.Angle(onNormal, Vector3.up);          // 0–90
            bool isDownHill = Vector3.Dot(inputVector, onPlane) >= 0;              // 内積で判定
            float signed = isDownHill ? angleAbs : -angleAbs; // 上り坂か下り坂か

            //Debug.Log("法線方向のベクトル(青)" + onNormal);
            //Debug.Log("平面に沿った方向のベクトル(緑)" + onPlane);
            //Debug.Log("平面に沿わせたいベクトル(赤)" + inputVector);

            //// デバッグ可視化
            //Debug.DrawRay(hit.point, onNormal, Color.blue);
            //Debug.DrawRay(hit.point, onPlane, Color.green);
            //Debug.DrawRay(hit.point, inputVector, Color.red);

            return signed;     // +下り / –上り
        }
        return -1.0f;          // 地面取得失敗
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
        slopeKey = Mathf.Clamp(slopeKey, -30, 30);

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


    /*＞アニメーションを切り替える関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:ダッシュの再生速度とアニメーションを変更
    */
    private void ChangeAnimation()
    {
        // --- アニメーション再生速度 ---
        if (m_fSpeed > 8.0f)    // 加算
        {
            float nSpeed = m_fSpeed - 8.0f;
            Player_Animator.speed = nSpeed * 0.0357f + 1.0f;
        }
        else    // 減算
        {
            float nSpeed = m_fSpeed - 8.0f;
            Player_Animator.speed = 1.0f + nSpeed * 0.0625f;
        }

        // --- アニメーション切り替え ---
        if (m_fSpeed < thresholds[0] + 1) 
        {
            Player_Animator.SetInteger("AnimNo", 0);
            return;
        }
        if (m_fSpeed > thresholds[0])
        {
            Player_Animator.SetInteger("AnimNo", 1);
            if (m_fSpeed > thresholds[1])
            {
                Player_Animator.SetInteger("AnimNo", 2);
                if (m_fSpeed > thresholds[2])
                {
                    Player_Animator.SetInteger("AnimNo", 3);
                    if (m_fSpeed > thresholds[3])
                    {
                        Player_Animator.SetInteger("AnimNo", 4);
                        return;
                    }
                    return;
                }
                return ;
            }
        }
        return;
    }

    /*＞状態を送る関数
   引数：なし
   ｘ
   戻値：状態を表す数値
   ｘ
   概要:プレイヤーの状態を送る
   */
    public int GetState()
    {
        return (int)PlayerState;
    }

    /*＞状態を送る関数
    引数：なし
    ｘ
    戻値：現在の速度を表す数値
    ｘ
    概要:プレイヤーの現在の速度を送る
    */
    public float GetSpeed()
    {
        return m_fSpeed;
    }
}