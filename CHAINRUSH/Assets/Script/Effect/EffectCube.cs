/*=====
<EffectCube.cs>
└作成者：yamamoto

＞内容
敵死亡時の仮エフェクト

＞注意事項

＞更新履歴
Y25   
_M04    
__D     
___23:プログラムの作成:yamamoto

=====*/

using UnityEngine;

public class EffectCube : MonoBehaviour
{
    [SerializeField,Tooltip("表示時間")] private float m_fLifeTime; // 表示時間


    /*＞Update関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:更新関数
    */
    private void Update()
    {
        m_fLifeTime -= Time.deltaTime;

        if (m_fLifeTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
