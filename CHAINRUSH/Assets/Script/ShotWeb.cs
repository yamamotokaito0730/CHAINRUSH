/*=====
<ShotWeb.cs>
└作成者：tooyama

＞内容
Enemyが発射する糸(弾)を管理するスクリプト

＞注意事項

＞更新履歴
Y25   
_M05    
__D     
___23:プログラム作成:tooyama

=====*/
using UnityEngine;


public class ShotWeb : MonoBehaviour
{
    [Header("弾のステータス")]
    [SerializeField, Tooltip("デスポーン秒数")] private float m_fLifeTime = 5.0f;

    private Vector3 m_direction; // 糸を飛ばす位置
    private float m_fShotSpeed;

    /*＞Init関数
   引数1：ターゲットの位置
   ｘ
   引数2：弾の速度
   ｘ
   引数3：プレイヤー速度
   ｘ
   戻値：なし
   ｘ
   概要:糸の目標位置決定とプレイヤー速度を糸の速度に加算させる
   */
    public void Init(Vector3 _Direction,float _fShotSpeed, float _fPlayerSpeed)
    {
        m_direction = _Direction.normalized;
        m_fShotSpeed = _fPlayerSpeed + _fShotSpeed; // 糸の速度をプレイヤー速度に加算させる
        Destroy(gameObject, m_fLifeTime); // 一定時間後に消滅させる
    }
    /*＞Update関数
     引数：なし
     ｘ
     戻値：なし
     ｘ
     概要:糸をターゲットに向かって飛ばす
     */
    void Update()
    {
        transform.position += m_direction * m_fShotSpeed * Time.deltaTime;  
    }

    /*＞isDestroy関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:呼ばれたら糸を消滅させる(プレイヤーに弾が当たった時に使用)
    */
    public void isDestroy()
    {
        Destroy(gameObject);
    }
}
