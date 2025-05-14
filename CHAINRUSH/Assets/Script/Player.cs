/*=====
<Player.cs>
└作成者：yamamoto

＞内容
Playerの挙動を管理するスクリプト

＞注意事項
スコアのデバッグ用プログラムを消すこと


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

=====*/

using UnityEngine;

public class Player : MonoBehaviour
{
    // 変数宣言
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("加速")] private float m_fBoost;
    [Header("デバッグ")]
    [SerializeField, Tooltip("デバッグ表示")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("デバッグプレハブ取得")] private GameObject debugPrefab;

    [Header("重力関係")]
    [SerializeField, Tooltip("ベースの重力")]private float baseGravity = 9.81f;

    [SerializeField, Tooltip("重力の増加量")] private float gravityGainPerKill = 3.0f;

    private float extraGravity;

    private Rigidbody rb;
    private ScoreManager scoreManager;
    private DebugMode debugModeInstance;
    private Vector3 moveDir = Vector3.forward; // 現在の進行方向を保持
    private int nEnemyKillCount = 0; // 倒した敵の数
    [SerializeField] Animator anim;     // プレイヤーアニメーション


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
        scoreManager = FindAnyObjectByType<ScoreManager>();
        // 初期状態でデバッグ表示ONなら、UIを生成しておく
        if (m_bDebugView && debugModeInstance == null)
        {
            GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity);
            debugModeInstance = obj.GetComponent<DebugMode>();
        }
        anim = GetComponent<Animator>();
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
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
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
        if (Input.GetKeyDown(KeyCode.Q)) scoreManager.AddScore(100, 1); // スコア加算用　*必要か分からん
        if (Input.GetKeyDown(KeyCode.E)) m_fSpeed += m_fBoost; // 加速デバッグ用

        // デバッグUI表示
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            m_bDebugView = !m_bDebugView; // UIの表示非表示切り替え
            if (m_bDebugView && debugModeInstance == null)
            {
                // プレハブからインスタンスを生成し、DebugModeを取得
                GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity); // 座標・回転はプレハブ側で設定
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

        ChangeAnimation();

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
        float rotateSpeed = 100f; // 回転速度

        float turn = 0f;

        if (Input.GetKey(KeyCode.A)) turn = -1f; // 左回転
        if (Input.GetKey(KeyCode.D)) turn = 1f;  // 右回転

        if (turn != 0f)
        {
            // Y軸を中心に回転させる
            transform.Rotate(0f, turn * rotateSpeed * Time.deltaTime, 0f);
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
                AddGravity(m_fBoost);
                nEnemyKillCount++; // キルカウントの増加
            }
        }
    }

    /*＞加速度増加関数
   引数：なし
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
    public void AddGravity(float _boost)
    {
        extraGravity += gravityGainPerKill;
        extraGravity = Mathf.Min(extraGravity, 40f); // 上限で制限
    }

    public void ChangeAnimation()
    {

        if (!anim.GetBool("Run") && !anim.GetBool("RunFast"))
        {
            anim.SetBool("Run", true);
        }
        
        if (Input.GetKeyDown("f"))
        {
            anim.SetBool("RunFast", true);
            anim.SetBool("Run", false);
        }



    }
    
}