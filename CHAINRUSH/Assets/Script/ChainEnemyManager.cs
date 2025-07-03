/*=====
<ChainEnemyManager.cs>
└作成者：tooyama

＞内容
連結蜘蛛専用スクリプト
連結蜘蛛の編成と昇格を行う(リーダー・フォロワー1・フォロワー2)

＞注意事項

＞更新履歴
Y25   
_M06
___20:スクリプトの作成 tooyama

=====*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEnemyManager : MonoBehaviour
{
    [Header("連結蜘蛛グループ（リーダーは必ず先頭に入れる）")]
    public EnemyPattern leaderSpider;
    public List<FollowEnemy> followerSpiders = new();

    [Header("連携攻撃インターバル")]
    public float chainAttackInterval = 2.0f;

    private Coroutine chainAttackRoutine;

    /*＞Awake関数
    引数：なし   
    ｘ
    戻値：なし
    ｘ
    概要：このオブジェクトが生成された時に一度だけ処理を行う
    */
    void Awake()
    {
        // 子オブジェクトから自動取得
        if (!leaderSpider)
            leaderSpider = GetComponentInChildren<EnemyPattern>();

        followerSpiders.Clear();
        foreach (var f in GetComponentsInChildren<FollowEnemy>())
            followerSpiders.Add(f);

        // フォロワーを隊列順に正しく連結する
        MonoBehaviour prev = leaderSpider; // 最初はリーダー
        foreach (var f in followerSpiders)
        {
            f.SetLeader(prev); // 直前キャラをリーダーとしてセット
            prev = f;          // 次はこのfが「前」になる
        }
    }

    void Start()
    {
        // 連携攻撃ルーチン開始
        chainAttackRoutine = StartCoroutine(ChainAttackLoop());
    }

    /*＞連携攻撃コルーチン
    引数：なし   
    ｘ
    戻値：IEnumerator
    ｘ
    概要：リーダーおよび全フォロワーの連携攻撃を一定間隔で順番に実行する
    */
    private IEnumerator ChainAttackLoop()
    {
        while (true)
        {
            // リーダー攻撃 (条件未定)
            if (leaderSpider != null && leaderSpider.gameObject.activeInHierarchy)
                yield return leaderSpider.StartChainAttack();

            // 順番にフォロワーも攻撃
            foreach (var f in followerSpiders)
            {
                if (f != null && f.gameObject.activeInHierarchy)
                    yield return f.StartChainAttack();
            }

            yield return new WaitForSeconds(chainAttackInterval);
        }
    }

    /*＞OnLeaderDead関数
    引数：なし   
    ｘ
    戻値：なし
    ｘ
    概要：リーダー死亡時に呼ばれる
         フォロワーが残っていれば先頭をリーダー昇格、全滅なら攻撃ルーチンを停止
    */
    public void OnLeaderDead()
    {
        // フォロワーが残っていれば新リーダー昇格
        if (followerSpiders.Count > 0)
        {
            var newLeaderObj = followerSpiders[0];
            followerSpiders.RemoveAt(0);

            // 先頭のフォロワーにEnemyPatternを付与する
            EnemyPattern newLeader = newLeaderObj.gameObject.AddComponent<EnemyPattern>();

            leaderSpider = newLeader;
            // 残りのフォロワーに新リーダーを設定
            foreach (var f in followerSpiders)
                f.SetLeader(newLeader);
        }
        else
        {
            // 全滅（チェイングループ破壊など）
            StopCoroutine(chainAttackRoutine);
        }
    }
}
