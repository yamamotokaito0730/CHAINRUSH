/*=====
<Enemy.cs>
└作成者：yamamoto

＞内容
Enemyの挙動を管理するスクリプト

＞注意事項

＞更新履歴
Y25   
_M04    
__D     
___23:プログラム作成:yamamoto
_M05
___14:Die関数の仕様変更

=====*/

using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("エフェクト")]
    [SerializeField, Tooltip("パーツ（0:頭, 1:胴体, 2:手, 3:足）")] private GameObject[] m_Parts;  // 頭・胴体・手・足を配列で管理
    [SerializeField, Tooltip("生成数")] private int m_nPartsNum;    // オブジェクトの生成

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*＞消滅関数
    引数：UnityEngine.Camera camera:メインのカメラ
    ｘ
    戻値：なし
    ｘ
    概要:この敵を消滅させる、カメラに向かって飛ばす＆張り付け処理
    */
    public void Die(UnityEngine.Camera camera)
    {
        bool die = true; // 初回ループ時のみEnemyオブジェクト削除

        for (int i = 0; i < m_nPartsNum; i++)
        {

            GameObject obj = Instantiate(m_Parts[i], transform.position, Quaternion.identity);
            FlyToCamera fly = obj.GetComponent<FlyToCamera>();
            fly.StartFly(camera); // カメラに向かって飛ぶ＆張り付き処理開始

            // 初回ループ時のみEnemyオブジェクト削除
            if (die)
            {
                die = false;
                Destroy(gameObject);
            }
        }
    }
}
