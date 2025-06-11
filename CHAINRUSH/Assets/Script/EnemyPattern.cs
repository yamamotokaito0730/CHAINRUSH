/*=====
<EnemyPattern.cs>
└作成者：tooyama

＞内容
敵の索敵・攻撃を行うスクリプト

＞注意事項

＞更新履歴
Y25   
_M05    
__D     
___23:プログラム作成:tooyama
___25:索敵・移動処理の追加 tooyama
___26:追跡処理の追加＆弾の速度を3→5に変更 tooyama
___30:攻撃エフェクトを追加 mori

=====*/
using System.Collections;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度(発見時)")] private float m_fFindSpeed = 5.0f;
    [SerializeField, Tooltip("移動速度(通常時)")] private float m_fNormalSpeed = 3.0f;
    [SerializeField, Tooltip("再始動までの時間")] private float m_fRetart = 10.0f;
    [SerializeField, Tooltip("索敵範囲(半径)")] private float m_fSearchRange = 15.0f;    // プレイヤーが侵入したら攻撃する範囲
    [SerializeField, Tooltip("索敵範囲オブジェクト（子オブジェクト）")] private GameObject m_SearchRangeObject;

    [Header("攻撃関係")]
    [SerializeField, Tooltip("糸のプレハブ")] private GameObject m_BulletPrefab;
    [SerializeField, Tooltip("攻撃間隔(秒)")] private float m_fShotInterval = 2.0f;
    [SerializeField, Tooltip("弾の速度")] private float m_fShotSpeed = 5.0f;

    private Transform m_targetPlayer; // 攻撃対象


    private bool m_bIsFinding = false; // プレイヤーを発見したか

    private SphereCollider m_SearchCollider; // 索敵に使用するスフィアコライダー

    private float m_fSpeed = 0.0f; // 現在の速度

    private Vector3 m_vStartPos; // 初期位置を保存する変数

    // コルーチン
    private Coroutine m_attackCoroutine;
    private Coroutine m_moveCoroutine;
    private Coroutine m_chaseCoroutine;

    // 攻撃エフェクト
    public GameObject effectPrefab;
    public float distanceInFront = 2.0f;     // 出現位置（前方距離）
    public float shootSpeed = 10.0f;          // 前方に飛ばす速度
    public float upwardOffset = 1.0f; // 上方向に1m上げる

    private Player player;

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        m_vStartPos = transform.position; // Start時の位置を保存
        m_SearchCollider = m_SearchRangeObject.GetComponent<SphereCollider>();
        // 半径を設定 ※索敵範囲を100分の1に割っているのはスフィアコライダーのradiusに合わせるため
        m_SearchCollider.radius = m_fSearchRange / 100.0f;
        // 索敵範囲を初期位置に固定させる(ローカル→ワールド座標に固定)
        m_SearchRangeObject.transform.position = m_vStartPos;  // 拠点に設置
        m_SearchRangeObject.transform.parent = null;        // 親を外す = ワールド固定
        // 移動処理を始める
        m_moveCoroutine = StartCoroutine(MoveRoutine());
    }

    /*＞索敵範囲侵入検知関数
    引数：Collider other : 索敵範囲に侵入したオブジェクト
    ｘ
    戻値：なし
    ｘ
    概要：プレイヤーが索敵範囲に入ったときに攻撃状態へ移行する
    */
    public void HandleSensorEnter(Collider other)
    {
        // プレイヤーが索敵範囲に侵入した時
        if (other.gameObject.CompareTag("Player") && !m_bIsFinding)
        {
            // 侵入時の位置を記録
            m_targetPlayer = other.transform;
            // プレイヤーの速度を取得
            player = other.GetComponent<Player>();
            m_fShotSpeed = player.GetSpeed() + 5.0f;
            // 発見フラグをオンに
            m_bIsFinding = true;

            // コルーチン開始（糸を定期的に発射する）
            if (m_attackCoroutine == null)
                m_attackCoroutine = StartCoroutine(ShootWebPeriodically());

            StateChange(); // 状態変更に応じて切り替え
        }
    }

    /*＞索敵範囲離脱検知関数
    引数：Collider other : 索敵範囲から離れたオブジェクト
    ｘ
    戻値：なし
    ｘ
    概要：プレイヤーが索敵範囲から出たときに追跡・攻撃を中止する
    */
    public void HandleSensorExit(Collider other)
    {
        // プレイヤーが索敵範囲から離れた時
        if (other.CompareTag("Player") && m_attackCoroutine != null)
        {
            // ターゲットをリセット
            m_targetPlayer = null;
            // 発見フラグをオフに
            m_bIsFinding = false;
            // 状態変更に応じて切り替え
            StateChange();
        }
    }

    /*＞索敵範囲描画関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:シーン内で索敵範囲を表示させる
    */
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(m_vStartPos, m_fSearchRange);
        else
            Gizmos.DrawWireSphere(transform.position, m_fSearchRange);
    }

    /*＞追跡コルーチン
   引数：なし
   ｘ
   戻値：なし
   ｘ
   概要:半径30m以内に入ったプレイヤーを速度を上げて追跡する
   */
    private IEnumerator ChaseRoutine()
    {

        while (m_targetPlayer != null)
        {
            // 拠点との距離チェック
            if (Vector3.Distance(transform.position, m_vStartPos) > m_fSearchRange)
            {
                // 索敵範囲を越えたら追跡中断 → 帰還へ
                m_targetPlayer = null;
                m_bIsFinding = false;
                StateChange();               // 帰還 or 巡回に切替
                yield break;                 // 追跡コルーチン終了
            }
            // 移動速度を発見時の速度に変更
            m_fSpeed = m_fFindSpeed;

            Vector3 direction = (m_targetPlayer.position - transform.position).normalized;
            float speed = m_fFindSpeed;

            // 向きをゆっくり変える
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }
            // プレイヤーに向かって前進
            transform.position += transform.forward * speed * Time.deltaTime;

            yield return null;
        }
    }


    /*＞攻撃関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:プレイヤーに向けて弾を発射する
    */
    private void Attack()
    {
        if (!effectPrefab) return;

        // 発射方向を敵の forward にする（プレイヤー方向じゃない）
        Vector3 shootDir = transform.forward;

        // 弾の生成位置
        Vector3 spawnPos = transform.position + shootDir * distanceInFront + Vector3.up * upwardOffset;

        // 発射方向に回転を合わせる
        Quaternion rotation = Quaternion.LookRotation(shootDir);

        GameObject bullet = Instantiate(effectPrefab, spawnPos, rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.AddForce(shootDir * m_fShotSpeed, ForceMode.VelocityChange);
        
    }

    /*＞移動コルーチン
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要: 索敵範囲内をランダムに移動する
    */
    private IEnumerator MoveRoutine()
    {
        while (true)
        {

            // 移動速度を非発見時の速度に変更
            m_fSpeed = m_fNormalSpeed;

            // ランダムな方向に移動
            // 方向を決める
            Vector3 randomDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
            // 移動距離を決める(0～5m)
            float distance = Random.Range(0.0f, 5.0f);
            // 移動計算
            Vector3 targetPos = transform.position + randomDirection * distance;

            // 初期位置からの距離が15mを超えているか？
            if (Vector3.Distance(m_vStartPos, targetPos) > m_fSearchRange)
            {
                yield return null;   // 必ずCPUを開放させる(フリーズ防止)
                continue; // その場合移動計算をやり直す
            }
            // 移動する方向に向きを変える
            Vector3 lookDir = (targetPos - transform.position).normalized;
            // 有効な方向があるか確認（ゼロベクトルでないか）
            if (lookDir != Vector3.zero)
            {
                Quaternion startRot = transform.rotation; // 現在の回転を記録
                Quaternion targetRot = Quaternion.LookRotation(lookDir); // 目的地に向いた回転を計算

                float rotateTime = 0.5f; // 回転にかける時間（秒）
                float elapsedRot = 0.0f; // 回転開始からの経過時間

                // rotateTime に達するまで徐々に回転させる
                while (elapsedRot < rotateTime)
                {
                    transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedRot / rotateTime); // 補間で滑らかに回転
                    elapsedRot += Time.deltaTime; // 経過時間を加算
                    yield return null; // 次のフレームまで待機
                }

                // 最終調整
                transform.rotation = targetRot;
            }
            // 移動にかかる時間を計算（距離 ÷ 速度）
            float moveTime = distance / m_fSpeed;
            // 移動に費やした時間のカウント用
            float elapsed = 0.0f;

            // 目的地へ向かって移動
            while (elapsed < moveTime)
            {
                // 現在位置から目的地へ一定速度で移動させる（1フレーム分）
                transform.position = Vector3.MoveTowards(transform.position, targetPos, m_fSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;  // 経過時間を加算
                yield return null; // 次のフレームまで待機
            }
            yield return new WaitForSeconds(m_fRetart); // 次の移動まで指定の時間(10秒)待機させる

        }
    }

    /*＞糸発射コルーチン
    引数：なし
    ｘ
    戻値：攻撃頻度(m_fShotInterval)の秒数
    ｘ
    概要:設定した秒数毎に糸を発射させる
    */
    private IEnumerator ShootWebPeriodically()
    {
        while (true)
        {
            if (m_targetPlayer != null) Attack(); // 攻撃関数の呼び出し

            yield return new WaitForSeconds(m_fShotInterval); // 指定時間(2秒)攻撃を待機させる
        }
    }

    /*＞状態遷移関数
     引数：なし
     ｘ
     戻値：なし
     ｘ
     概要:移動・追跡状態を切り替える
     */
    private void StateChange()
    {
        // プレイヤーを発見しているか
        if (m_bIsFinding)
        {
            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine); // 移動コルーチンの停止
                m_moveCoroutine = null; // 移動コルーチンの破棄     
            }
            if (m_chaseCoroutine == null) m_chaseCoroutine = StartCoroutine(ChaseRoutine()); // 追跡コルーチンの開始
        }
        else
        {
            if (m_chaseCoroutine != null)
            {
                StopCoroutine(m_chaseCoroutine); // 追跡コルーチンの停止
                m_chaseCoroutine = null; // 追跡コルーチンの破棄   
            }
            if (m_moveCoroutine == null) m_moveCoroutine = StartCoroutine(MoveRoutine()); // 移動コルーチンの開始
        }

    }

    /*＞OnDestroy関数
     引数：なし
     ｘ
     戻値：なし
     ｘ
     概要:破壊されると自動的に呼び出され、コルーチンを削除させる
          破壊済みオブジェクトのコルーチンを呼び出すことを防ぐ目的で使用
     */
    private void OnDestroy()
    {
        StopAllCoroutines();   // すべてのコルーチンを停止
    }
}
