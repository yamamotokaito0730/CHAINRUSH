/*=====
<TitleScroll.cs>
└作成者：mori

＞内容
タイトルシーンの背景オブジェクト移動処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___13:プログラム作成:mori

=====*/

using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField, Tooltip("移動速度")] private float m_fSpeed;
    [SerializeField, Tooltip("左端")]     private float m_fResetPosX;
    [SerializeField, Tooltip("右端")]     private float m_fStartPosX;
    
    void Update()
    {
        // 左に移動
        transform.position += Vector3.left * m_fSpeed * Time.deltaTime;

        // 左端に到達したら右端へ戻る
        if (transform.position.x < m_fResetPosX)
        {
            Vector3 pos = transform.position;
            pos.x = m_fStartPosX;
            transform.position = pos;
        }
    }
}
