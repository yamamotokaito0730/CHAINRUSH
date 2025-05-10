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
___10:バウンド防止処理を追加:tooyama

=====*/

using UnityEngine;

public class Player : MonoBehaviour
{
    // 変数宣言
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("加速量")] private float m_fBoost;

    [Header("デバッグ")]
    [SerializeField, Tooltip("デバッグ表示")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("デバッグプレハブ取得")] private GameObject debugPrefab;

    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")] private float baseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float gravityGainPerKill = 3.0f;

    private float extraGravity;

    private Rigidbody rb;
    private DebugMode debugModeInstance;
    private Vector3 moveDir = Vector3.forward; // 現在の進行方向を保持
    private int nEnemyKillCount = 0; // 倒した敵の数


    private bool wasInAir = false; // 浮いているかどうか
    private float groundCheckDistance = 1.1f; // レイで地面を検知する距離




    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbodyの取得

        // 初期状態でデバッグ表示ONなら、UIを生成しておく
        if (m_bDebugView && debugModeInstance == null)
        {
            GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity);
            debugModeInstance = obj.GetComponent<DebugMode>();
        }

        extraGravity = baseGravity;
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
        //rb.linearVelocity = transform.forward * m_fSpeed;

        //跳ねる瞬間だけをピンポイントで抑制するパターン
        // 接地状態を判定
        bool grounded = IsGrounded();

        // 空中 → 地面 に切り替わった瞬間
        if (grounded && wasInAir)
        {
            if (rb.linearVelocity.y > 0f)
            {
                Vector3 v = rb.linearVelocity;
                v.y = 0f;
                rb.linearVelocity = v;

                Debug.Log("バウンドガード");
            }

            wasInAir = false; // フラグをリセット
        }
        // 地面から離れた瞬間にフラグを立てる
        else if (!grounded && !wasInAir)
        {
            wasInAir = true;
        }


        // 重力の追加
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);


        //// 常時yに制限を掛けるパターン
        //if (IsGrounded())
        //{
        //    // 1. Y方向の跳ねを抑制
        //    if (rb.linearVelocity.y > 0.05f)
        //    {
        //        Vector3 v = rb.linearVelocity;
        //        v.y = 0f;
        //        rb.linearVelocity = v;
        //    }

        //    // 2. 地形の法線に合わせて前方移動（這うように）
        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.0f))
        //    {
        //        Vector3 normal = hit.normal;

        //        Vector3 forward = transform.forward;
        //        Vector3 slopeAdjusted = Vector3.ProjectOnPlane(forward, normal).normalized;

        //        // 補正の割合（0 = 補正なし、1 = 完全に法線に沿う）
        //        float blend = 0.0f;

        //        Vector3 moveDir = Vector3.Lerp(forward, slopeAdjusted, blend).normalized;
        //        rb.MovePosition(transform.position + moveDir * m_fSpeed * Time.fixedDeltaTime);
        //    }

        //    // 3. 接地中 → 高さ記録
        //    lastGroundY = transform.position.y;
        //    preventYUp = false;
        //}
        //else if (WasGroundedRecently())
        //{
        //    // 4. 空中でYが上がりすぎたら抑制
        //    if (transform.position.y > lastGroundY)
        //    {
        //        Vector3 clamped = transform.position;
        //        clamped.y = lastGroundY;
        //        transform.position = clamped;

        //        Vector3 v = rb.linearVelocity;
        //        if (v.y > 0) v.y = 0f;
        //        rb.linearVelocity = v;

        //        preventYUp = true;
        //    }

        //    // 5. 重力追加（空中時のみ）
        //    rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        //}
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

        //   SlopeSpeedChange(debugModeInstance.slopeAngle);
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
                enemy.Die();
                AddBoost(m_fBoost);
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
        extraGravity += gravityGainPerKill;
        extraGravity = Mathf.Min(extraGravity, 40f); // 上限で制限
    }

    /*＞速度変化関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:坂の角度によってプレイヤー速度を増減させる
    */
    //private void SlopeSpeedChange(float _Slope)
    //{
    //    if(Slope)
    //}

    /*＞接地判定関数
引数：なし
戻値：bool：地面に接しているかどうか
概要: Raycastを用いて足元の地面を検出
*/
    private bool IsGrounded()
    {
        float offset = 0.1f;
        Vector3 origin = transform.position + Vector3.up * offset;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance + offset); // レイキャストを使って判定を行う
    }

    /*＞直前まで地面にいたか判定する関数
    引数：なし
    戻値：bool：空中 → 接地に入るかの判定補助
    概要: 前フレームの状態を記録しておくことで、直前まで地面にいたかを判断
    */
    private bool WasGroundedRecently()
    {
        return wasInAir && !IsGrounded(); // 空中にいる&&地面についていない
    }

    //    /*＞接地判定関数
    //引数：なし
    //戻値：bool：地面に接しているかどうか
    //概要: Raycastを用いて足元の地面を検出
    //*/
    //    private bool IsGrounded()
    //    {
    //        float radius = 0.3f;
    //        float distance = 0.6f;
    //        Vector3 origin = transform.position + Vector3.up * 0.1f;
    //        return Physics.SphereCast(origin, radius, Vector3.down, out _, distance);
    //    }

    //    /*＞直前まで地面にいたか判定する関数
    //    引数：なし
    //    戻値：bool：空中 → 接地に入るかの判定補助
    //    概要: 前フレームの状態を記録しておくことで、直前まで地面にいたかを判断
    //    */
    //    private bool WasGroundedRecently()
    //    {
    //      //  return wasInAir && !IsGrounded();
    //          return !IsGrounded(); // より厳密にしたい場合は接地フラグの履歴を持たせてもOK 3用
    //    }


}