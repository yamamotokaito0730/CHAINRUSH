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
___25:オーラエフェクトに関する処理を追加:matsushima
_M07
___01:プレイヤーのパーティクルの再生、位置をずらす処理の追加:matsushima
___07:パーティクルの位置の調整、ボーンに合わせたパーティクルの移動(実装中のためコメントアウト):matsushima
___09:パーティクルの位置の再調整:matsushima
___13:パーティクル関連を変更しいらない部分を削除:matsushima
___16:蒸気のパーティクルに関する処理を削除:matsushima
___25:効果数値が上がる度にオブジェクトへの当たり判定を広くする処理の追加 tooyama
___31:瀕死時に糸が巻き付いているエフェクトを追加:matsushima
=====*/

using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

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
    private E_State preState = E_State.Normal;  // プレイヤーの状態退避
    [SerializeField, Tooltip("ステージ")] private int m_Stage = 0;
    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float m_fAddGravity = 3.0f;

    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // プレイヤーの物理挙動を制御するためのRigidbody
    private Renderer renderer;       // オーラエフェクトのレンダラー
    private int nEnemyKillCount = 0; // 倒した敵の数
    private int m_nPrevSlopeAngleKey = int.MinValue; // 前フレームで適用された傾斜角（10度単位）
    private float m_fRecordedBaseSpeed = 0.0f; // 傾斜に入った瞬間の速度記録用
    private ParticleSystem[] particleSystems;  // パーティクルの配列
    private GameObject thread;                 // 糸のパーティクル(個別取得)

    [SerializeField] private Animator Player_Animator;

    // 読み取り専用プロパティを追加(ShotWebクラスで発射する糸の速度に乗算させる為)
    public float PlayerSpeed => m_fSpeed;

    // 当たり判定関係
    private CapsuleCollider playerAttackCollider; // 破壊オブジェクトとのコライダー
    private float m_fBaseRadius = 1.0f; // カプセルコライダー(playerAttackCollider)の基準半径
    private float[] m_fColliderSizeTable = { 1.0f, 1.6f, 1.9f, 2.5f }; // 当たり判定サイズテーブル(速度が上がるにつれて半径を広げる Max2.5倍)

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

        // オーラエフェクトのマテリアルを取得
        GameObject childObject = transform.Find("PlayerCharacter_006/Body").gameObject; // マテリアルが入っている子オブジェクトを取得 // マテリアルが入っている子オブジェクトを取得
        renderer = childObject.GetComponent<Renderer>();

        // パーティクルを取得
        Transform particleParent = transform.Find("Particle");
        List<ParticleSystem> particles = new List<ParticleSystem>();
        foreach (Transform child in particleParent)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            particles.Add(ps);
        }
        particleSystems = particles.ToArray();
        thread = transform.Find(
            "PlayerCharacter_006/arm/hips/spine/chest/chest_001/restraintEffect001").gameObject;    // 糸(直接取得)

        if (m_Stage == 2)
        {
            BGMManager.Instance.ChangeBGM("Stage2", 1.5f);
        }

        // playerAttackCollider（子オブジェクト）に入ってるカプセルコライダーを取得
        playerAttackCollider = transform.Find("PlayerAttackCollider").GetComponent<CapsuleCollider>();
        m_fBaseRadius = playerAttackCollider.radius; // 最初の半径を基準として保存
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
        //ゲーム開始前は停止
        if (!GameManager.IsGameActive)
        {
            //todo
           //アニメーション関係書くと思う

            return;
        }

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
        const float rayLen = 100.0f; // レイを飛ばす距離

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float range = 0.5f; // 地面に当たるまでの許容範囲


        Debug.DrawRay(origin, direction * rayLen, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLen))
        {

            if (range > Vector3.Distance(transform.position, hit.point))
            {
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
                // 重力の追加
                //    rb.AddForce(Vector3.down * m_fBaseGravity * 10.0f, ForceMode.Acceleration);
                Vector3 corrected = transform.position;
                corrected.y = Mathf.Lerp(transform.position.y, hit.point.y, Time.fixedDeltaTime * 20.0f);
                rb.MovePosition(corrected);
            }
        }

        // オーラエフェクトの色変更
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        switch (PlayerState)
        {
            case E_State.Danger:
                // "_OutLineColor"というReferenceを持つマテリアルの色を変更
                block.SetColor("_OutLineColor", Color.clear * 5.0f);
                break;
            case E_State.Normal:
                block.SetColor("_OutLineColor", Color.yellow * 5.0f);
                break;
            case E_State.TreeDestroy:
                block.SetColor("_OutLineColor", Color.red * 5.0f);
                break;
            case E_State.HomeDestroy:
                block.SetColor("_OutLineColor", Color.magenta * 5.0f);
                break;
            case E_State.Strongest:
                block.SetColor("_OutLineColor", new Color(0.5f, 0.8f, 1.0f, 1.0f) * 5.0f);  // 水色
                break;
        }
        renderer.SetPropertyBlock(block);   // 適用

        //---パーティクル関連
        if (preState != PlayerState)
        {
            // 加速時のエフェクト(速度が次の状態まで上昇した時だけ再生する)
            if (preState < PlayerState)
            {
                particleSystems[1].Play();
            }

            // 火花のエフェクト(最大速度の時のみ再生)
            if (PlayerState == E_State.Strongest)
            {
                particleSystems[3].Play();
            }
            else
            {
                particleSystems[3].Stop();
            }
            // 糸のエフェクト(瀕死時に再生)
            if (PlayerState == E_State.Danger)
            {
                thread.SetActive(true);
            }
            else
            {
                thread.SetActive(false);
            }
        }      

        this.ColliderScaleUp(); // 状態に応じてコライダーのサイズを上げる

        preState = PlayerState; // 状態の退避
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

    /*＞衝突検知破壊関数
      引数：Collider 衝突した相手のコライダー
      ｘ
      戻値：なし
      ｘ
      概要:衝突したオブジェクトとの当たり判定を取り
           衝突した相手が敵だったらその敵を破壊する
      */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die(mainCamera, this); // エネミー分割処理
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

    /*＞状態を送る関数
    引数：なし
    ｘ
    戻値：現在の破壊可能半径(playerAttackCollider)を表す数値
    ｘ
    概要:プレイヤーの現在の破壊オブジェクトに対するカプセルコライダー(playerAttackCollider)の半径を送る
    */
    public float GetRadius()
    {
        return playerAttackCollider.radius;
    }

    /*＞当たり判定変更関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:プレイヤーの破壊オブジェクトに対するカプセルコライダー(playerAttackCollider)の半径を変更する
    */
    private void ColliderScaleUp()
    {
        int nColliderTableIndex = 0; // 半径テーブル用インデックス
        switch (PlayerState)
        {
            case E_State.Danger:
                nColliderTableIndex = 0; // 1.0倍
                break;
            case E_State.Normal:
                nColliderTableIndex = 0; // 1.0倍
                break;
            case E_State.TreeDestroy:
                nColliderTableIndex = 1; // 1.6倍
                break;
            case E_State.HomeDestroy:
                nColliderTableIndex = 2; // 1.9倍
                break;
            case E_State.Strongest:
                nColliderTableIndex = 3; // 2.5倍
                break;
        }
        playerAttackCollider.radius = m_fBaseRadius * m_fColliderSizeTable[nColliderTableIndex]; // 半径を拡大率で変更
    }

}