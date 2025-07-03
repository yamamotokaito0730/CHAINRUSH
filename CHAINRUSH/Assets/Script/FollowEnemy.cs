/*=====
<FollowEnemy.cs>
└作成者：tooyama

＞内容
連結蜘蛛専用スクリプト
追従処理を行う(ドラクエのパーティ移動のイメージ)

＞注意事項

＞更新履歴
Y25   
_M06
___20:スクリプトの作成 tooyama

=====*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPattern;

public class FollowEnemy : MonoBehaviour
{
    EnemyPattern enemyPattern;

    [Header("追従設定")]
    [SerializeField,Tooltip("追従するリーダー蜘蛛")] private Transform leaderSpider;
    [SerializeField] private EnemyPattern leaderPattern;        // リーダーのEnemyPattern参照
    [SerializeField, Tooltip("追従距離（m）")] private float m_fFollowDistance = 5.0f;
    [SerializeField, Tooltip("遅延フレーム")] private int m_fDelayFrames = 15;  // 何フレーム遅れて動くか（追従ディレイ）
    [SerializeField, Tooltip("移動速度(発見時)")] private float m_fFindSpeed = 10.0f;
    [SerializeField, Tooltip("移動速度(通常時)")] private float m_fNormalSpeed = 8.0f;


    [Header("攻撃設定")]
    [SerializeField, Tooltip("糸のプレハブ")] private GameObject m_BulletPrefab;
    [SerializeField, Tooltip("攻撃間隔(秒)")] private float m_fShotInterval = 1.0f;
    [SerializeField, Tooltip("弾の加速度")] private float m_fShotSpeed = 10.0f;
    [SerializeField, Tooltip("攻撃時に止まる秒数")] private float m_fShotStopTime = 0.8f;

    // エフェクト関係
    public GameObject effectPrefab;
    public float distanceInFront = 2.0f;     // 出現位置（前方距離）
    public float upwardOffset = 1.0f; // 上方向に1m上げる

    private MonoBehaviour leaderObject;  // 直前のキャラ(EnemyPattern or FollowEnemy)
    public Queue<Vector3> positionHistory = new(); // リーダー位置の履歴
    public Queue<Quaternion> rotationHistory = new();  // リーダー回転の履歴
    private bool m_bIsAttacking = false;

    // Update is called once per frame
    void Update()
    {
        if (!leaderSpider) return;

        // 1. 自分の現在位置を履歴として記録
        positionHistory.Enqueue(transform.position);
        rotationHistory.Enqueue(transform.rotation);
        if (positionHistory.Count > 60) { positionHistory.Dequeue(); rotationHistory.Dequeue(); } // メモリ制限

        // Debug: 自分の履歴数
        Debug.Log($"{gameObject.name} の履歴数: {positionHistory.Count}");


        // 2. 直前キャラ（リーダー or フォロワー）の履歴Queueを取得
        Queue<Vector3> leadPosQueue = null;
        Queue<Quaternion> leadRotQueue = null;

        if (leaderObject is EnemyPattern ep)
        {
            leadPosQueue = ep.positionHistory;
            leadRotQueue = ep.rotationHistory;
            Debug.Log($"{gameObject.name} は EnemyPattern({ep.gameObject.name}) を追従中 (リーダー履歴: {leadPosQueue.Count})");
        }
        else if (leaderObject is FollowEnemy fe)
        {
            leadPosQueue = fe.positionHistory;
            leadRotQueue = fe.rotationHistory;
            Debug.Log($"{gameObject.name} は FollowEnemy({fe.gameObject.name}) を追従中 (リーダー履歴: {leadPosQueue.Count})");
        }

        if (leadPosQueue == null || leadRotQueue == null)
        {
            Debug.Log($"{gameObject.name}: リーダーの履歴Queueがnullです");
            return;
        }
        // 履歴が足りなければ待機
        if (leadPosQueue.Count < m_fDelayFrames) 
        {
            Debug.Log($"{gameObject.name}: リーダーの履歴が不足 ({leadPosQueue.Count}/{m_fDelayFrames}) しています");
            return;
        }

        // 3. 遅延分前の位置・回転を取得
        Vector3[] leadPosArr = leadPosQueue.ToArray();
        Quaternion[] leadRotArr = leadRotQueue.ToArray();
        Vector3 targetPos = leadPosArr[0];          // 一番古い（遅延分前の）位置
        Quaternion targetRot = leadRotArr[0];

        // Debug: 追従目標位置と現在地
        Debug.Log($"{gameObject.name} の移動: {transform.position} → {targetPos}");
        // 速度切り替え
        float speed = m_fNormalSpeed; // 通常速度
        if (leaderObject is EnemyPattern ep2 && ep2.IsChasing()) // リーダーが追跡中か
            speed = m_fFindSpeed; // 追跡速度に変える

        // Debug: 現在の速度
        Debug.Log($"{gameObject.name} の現在速度: {speed}");

        // 5. 追従移動・回転
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.2f);
    }

    /*＞StartChainAttackコルーチン
    引数：なし
    ｘ
    戻値：IEnumerator
    ｘ
    攻撃中フラグが立っていなければエフェクトを生成し、指定時間停止する
    */
    public IEnumerator StartChainAttack()
    {
        if (m_bIsAttacking) yield break;
        m_bIsAttacking = true;

        // 発射方向・位置は自身のforward基準
        Vector3 shootDir = transform.forward;
        Vector3 spawnPos = transform.position + shootDir * distanceInFront + Vector3.up * upwardOffset;
        Quaternion rotation = Quaternion.LookRotation(shootDir);

        GameObject bullet = Instantiate(effectPrefab, spawnPos, rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(shootDir * m_fShotSpeed, ForceMode.VelocityChange);

        yield return new WaitForSeconds(m_fShotStopTime);
        m_bIsAttacking = false;
    }

    /*＞SetLeader関数
    引数：MonoBehaviour prev : 直前のキャラ（EnemyPattern or FollowEnemy）
    ｘ
    戻値：なし
    ｘ
    概要：リーダーまたは直前のフォロワーを登録し、追従履歴をリセットする
           -リーダーがEnemyPatternかFollowEnemyか動的に判別してTransformもセット
           - 隊列再編成やリーダー昇格時にも利用
    */
    public void SetLeader(MonoBehaviour prev)
    {
        leaderObject = prev; // 前にいるキャラの参照（リーダー(EnemyPattern)かフォロワー(FollowEnemy)か判別）
        // リーダーの場合
        if (prev is EnemyPattern ep)
            leaderSpider = ep.transform;
        // フォロワーの場合
        else if (prev is FollowEnemy fe)
            leaderSpider = fe.transform;
        // 型が合わない場合はエラー防止のためnullにする
        else
            leaderSpider = null;
        // 追従履歴をリセット
        positionHistory.Clear();
        rotationHistory.Clear();
    }
}
