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

=====*/

using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("エフェクト")]
    [SerializeField, Tooltip("プレハブ")] private GameObject m_EffectCube;       // エフェクトキューブプレハブ
    [SerializeField, Tooltip("生成数")] private int m_nEffectNum;              // エフェクトキューブ生成数
    [SerializeField,Tooltip("範囲")] private float m_fPosRandRange;  // エフェクトキューブを生成するポジションをランダムに生成するための範囲

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*＞消滅関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:この敵を消滅させる
    */
    public void Die()
    {
        float x, y, z = 0.0f;

        // エフェクトキューブ生成
        for (int i = 0; i < m_nEffectNum; i++)
        {
            x = Random.Range(-m_fPosRandRange, m_fPosRandRange);
            y = Random.Range(-m_fPosRandRange, m_fPosRandRange);
            z = Random.Range(-m_fPosRandRange, m_fPosRandRange);

            Instantiate(m_EffectCube, new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z), Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
