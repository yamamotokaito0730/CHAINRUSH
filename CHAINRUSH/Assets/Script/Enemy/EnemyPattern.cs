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
___23:プログラム作成:tooya
___25:索敵・移動処理の追加 tooyama
___26:追跡処理の追加＆弾の速度を3→5に変更 tooyama
___30:攻撃エフェクトを追加 mori
_M06
___20:後退処理の追加 tooyama

=====*/
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度(発見時)")] private float m_fFindSpeed = 10.0f;
    [SerializeField, Tooltip("移動速度(通常時)")] private float m_fNormalSpeed = 8.0f;
    [SerializeField, Tooltip("再始動までの時間")] private float m_fRetart = 10.0f;
    [SerializeField, Tooltip("索敵範囲(半径)")] private float m_fSearchRange = 30.0f;    // プレイヤーが侵入したら攻撃する範囲
    [SerializeField, Tooltip("索敵範囲オブジェクト（子オブジェクト）")] private GameObject m_SearchRangeObject;
    [SerializeField, Tooltip("プレイヤーと敵の距離間")] private float m_PlayerToEnemyDistance = 5.0f;
    [SerializeField, Tooltip("後退時の速度現象値")] private float m_SpeedDown = 0.7f;



    [Header("攻撃関係")]
    [SerializeField, Tooltip("糸のプレハブ")] private GameObject m_BulletPrefab;
    [SerializeField, Tooltip("攻撃間隔(秒)")] private float m_fShotInterval = 1.0f;
    [SerializeField, Tooltip("弾の加速度")] private float m_fShotSpeed = 10.0f;
    [SerializeField, Tooltip("攻撃時に止まる秒数")] private float m_fShotStopTime = 0.8f;


    private Transform m_targetPlayer; // 攻撃対象

    private bool m_bIsFinding = false; // プレイヤーを発見したか

    private bool m_bIsAttacking = false; // 攻撃中か


    private SphereCollider m_SearchCollider; // 索敵に使用するスフィアコライダー

    private float m_fSpeed = 0.0f; // 現在の速度

    private Vector3 m_vStartPos; // 初期位置を保存する変数

    // 攻撃エフェクト
    public GameObject effectPrefab;
    public float distanceInFront = 2.0f;     // 出現位置（前方距離）
    public float upwardOffset = 1.0f; // 上方向に1m上げる

    private Player player;

    // 敵の状態管理
    public enum E_EnemyState
    {
        E_EnemyState_Patrol, // 巡回
        E_EnemyState_Chase,  // 追跡 
    }
    // 現在の状態
    private E_EnemyState m_CurrentState;

    // 現在動いているコルーチン
    private Coroutine m_CurrentRoutine;

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        // ゲーム開始時の座標を保存
        m_vStartPos = transform.position;
        m_SearchCollider = m_SearchRangeObject.GetComponent<SphereCollider>();
        // 半径を設定 ※索敵範囲を100分の1に割っているのはスフィアコライダーのradiusに合わせるため
        m_SearchCollider.radius = m_fSearchRange / 100.0f;
        // 初期状態を巡回にする
        ChangeState(E_EnemyState.E_EnemyState_Patrol);

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
        if (other.CompareTag("Player") && !m_bIsFinding)
        {
            m_targetPlayer = other.transform; // 侵入時の位置を記録
            player = other.GetComponent<Player>(); // プレイヤーの情報を取得(後々速度を取得するため)
            m_bIsFinding = true; // 発見フラグをオンに
            // コルーチンの移行
            ChangeState(E_EnemyState.E_EnemyState_Chase);  // 追跡を開始する
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
        if (other.CompareTag("Player") && m_bIsFinding)
        {
            m_targetPlayer = null; // ターゲットをリセット
            m_bIsFinding = false;  // 発見フラグをオフに
            m_vStartPos = transform.position; // 追跡終了地点を新たな拠点とする(長距離を追跡し、離れると固まる問題を防止するため)
            // コルーチンの移行
            ChangeState(E_EnemyState.E_EnemyState_Patrol); // 巡回を開始する
        }
    }

    /*＞追跡コルーチン
   引数：なし
   ｘ
   戻値：なし
   ｘ
   概要:半径30m以内に入ったプレイヤーを速度を上げて追跡する
        自爆特攻しないようにプレイヤーとの距離は15mを保つ
   */
    private IEnumerator ChaseRoutine()
    {
        float _fAttackInterval = m_fShotInterval; // 攻撃間隔(2秒)
        float _fLastAttackTime = -_fAttackInterval;


        while (m_targetPlayer != null)
        {
            if (m_bIsAttacking)
            {
                yield return null;
                continue;
            }
            // 移動速度を発見時の速度に変更
            m_fSpeed = m_fFindSpeed; // 速度8→10

            Vector3 toPlayer = m_targetPlayer.position - transform.position;
            float distance = toPlayer.magnitude; // プレイヤーとの距離
            Vector3 direction = toPlayer.normalized;


            // 向きをゆっくり変える
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }

            float _fTolerance = 1.0f; // 距離の許容範囲（±1m）

            // プレイヤーとの距離を5mに保つ
            if (distance < m_PlayerToEnemyDistance - _fTolerance)
            {
                float overDistance = m_PlayerToEnemyDistance - distance;
                m_fSpeed -= m_SpeedDown;
                float moveDistance = Mathf.Min(m_fSpeed * Time.deltaTime, overDistance);
                transform.position -= direction * moveDistance;
            }
            else if (distance > m_PlayerToEnemyDistance + _fTolerance)
            {
                float underDistance = distance - m_PlayerToEnemyDistance;
                float moveDistance = Mathf.Min(m_fSpeed * Time.deltaTime, underDistance);
                transform.position += direction * moveDistance;
            }

            // 攻撃間隔をチェック
            if (Time.time - _fLastAttackTime >= _fAttackInterval)
            {
                StartCoroutine(AttackCoroutine()); // 弾の発射処理
                _fLastAttackTime = Time.time;
            }

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
    private IEnumerator AttackCoroutine()
    {
        if (!effectPrefab) yield break;

        // 発射方向を敵の forward にする（プレイヤー方向じゃない）
        Vector3 shootDir = transform.forward;

        // 弾の生成位置
        Vector3 spawnPos = transform.position + shootDir * distanceInFront + Vector3.up * upwardOffset;

        // 発射方向に回転を合わせる
        Quaternion rotation = Quaternion.LookRotation(shootDir);

        GameObject bullet = ObjectPoolManager.Instance.SpawnFromPool("spiderATK_001", spawnPos, rotation);
            //Instantiate(effectPrefab, spawnPos, rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();

        rb.useGravity = false;

        // 現在のプレイヤー速度を取得して弾速に加算する
        float currentPlayerSpeed = player.GetSpeed();
        float finalShotSpeed = m_fShotSpeed + currentPlayerSpeed;

        rb.AddForce(shootDir * finalShotSpeed, ForceMode.VelocityChange);

        // 攻撃中は移動処理を行わない
        m_bIsAttacking = true;
        yield return new WaitForSeconds(m_fShotStopTime);
        m_bIsAttacking = false;
    }

    /*＞移動コルーチン
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要: 索敵範囲内をランダムに移動する
    */
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {

            // 移動速度を非発見時の速度に変更
            m_fSpeed = m_fNormalSpeed; // 速度10→8

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
    /*＞状態遷移関数
     引数：E_EnemyState _EnemyState:切り替える状態
     ｘ
     戻値：なし
     ｘ
     概要:状態を切り替え、該当のコルーチンを起動する
     */
    private void ChangeState(E_EnemyState _EnemyState)
    {
        // 既に実行中の処理があれば停止
        if (m_CurrentRoutine != null)
        {
            StopCoroutine(m_CurrentRoutine); // コルーチンの停止
            m_CurrentRoutine = null;
        }

        // 状態切り替え
        m_CurrentState = _EnemyState;

        // 新しい状態に応じた処理を開始
        switch (m_CurrentState)
        {
            case E_EnemyState.E_EnemyState_Patrol:
                m_CurrentRoutine = StartCoroutine(PatrolRoutine()); // 巡回処理
                m_SearchRangeObject.transform.parent = null; // SearchRangeとの親子付けを解除しその場に固定する 
                break;
            case E_EnemyState.E_EnemyState_Chase:
                m_CurrentRoutine = StartCoroutine(ChaseRoutine()); // 追跡処理
                m_SearchRangeObject.transform.SetParent(transform, true); // 親子付けを復元させる
                break;
            default:
                break;
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
