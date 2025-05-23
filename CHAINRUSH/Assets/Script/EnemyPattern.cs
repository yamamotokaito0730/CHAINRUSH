/*=====
<EnemyPattern.cs>
└作成者：tooyama

＞内容
Enemyの索敵・攻撃を行うスクリプト

＞注意事項

＞更新履歴
Y25   
_M05    
__D     
___23:プログラム作成:tooyama

=====*/
using System.Collections;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("索敵範囲(半径)")] private float m_fSearchRange = 15.0f;    // プレイヤーが侵入したら攻撃する範囲
    [SerializeField, Tooltip("索敵範囲オブジェクト（子オブジェクト）")] private GameObject m_SearchRangeObject;

    [Header("攻撃関係")]
    [SerializeField, Tooltip("糸のプレハブ")] private GameObject m_BulletPrefab;
    [SerializeField, Tooltip("攻撃間隔(秒)")] private float m_fShotInterval = 2.0f;
    [SerializeField, Tooltip("弾の速度")] private float m_fShotSpeed = 3.0f;

    private Transform m_targetPlayer; // 攻撃対象

    private Coroutine m_attackCoroutine; // コルーチン

    private bool m_bIsAttacking = false; // 攻撃中か

    private SphereCollider m_SearchCollider; // 索敵に使用するスフィアコライダー

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        m_SearchCollider = m_SearchRangeObject.GetComponent<SphereCollider>();
        // 半径を設定 ※索敵範囲を100分の1に割っているのはスフィアコライダーのradiusに合わせるため
        m_SearchCollider.radius = m_fSearchRange / 100.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが索敵範囲に侵入した時
        if (other.gameObject.CompareTag("Player") && !m_bIsAttacking)
        {
            // 侵入時の位置を記録
            m_targetPlayer = other.transform; 
            // 2秒ごとに弾を発射するコルーチン開始
            m_attackCoroutine = StartCoroutine(ShootWebPeriodically());
            // 攻撃中フラグをオンに
            m_bIsAttacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // プレイヤーが索敵範囲から離れた時
        if (other.CompareTag("Player") && m_attackCoroutine != null)
        {
            // 攻撃コルーチンを停止 
            StopCoroutine(m_attackCoroutine);
            // コルーチンの参照をクリア
            m_attackCoroutine = null;
            // ターゲットをリセット
            m_targetPlayer = null;
            // 攻撃中フラグをオフに
            m_bIsAttacking = false;
        }
    }

    /*＞索敵範囲描画関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:シーン内で索敵範囲を描画させる
    */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_fSearchRange);
    }
    /*＞攻撃関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:プレイヤーに向けて糸を発射する
    */
    private void Attack()
    {
        // nullチェック
        if (!m_targetPlayer || !m_BulletPrefab) return;
        // 糸の生成位置を決める
        Vector3 spawnPos = transform.position;
        // 糸を飛ばす位置を決める
        Vector3 direction = (m_targetPlayer.position - spawnPos).normalized;

        // プレイヤーの速度を取得
        Player player = m_targetPlayer.GetComponent<Player>();
        if (!player) return;

        float f_PlayerSpeed = player.PlayerSpeed;

        GameObject ShotWeb = Instantiate(m_BulletPrefab, spawnPos, Quaternion.LookRotation(direction));

        // ShotWebスクリプトを取得し、初期化
        ShotWeb webScript = ShotWeb.GetComponent<ShotWeb>();
        if (webScript)
        {
            webScript.Init(direction, m_fShotSpeed, f_PlayerSpeed);
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
            if (m_targetPlayer != null) Attack();

            yield return new WaitForSeconds(m_fShotInterval);
        }
    }
}
