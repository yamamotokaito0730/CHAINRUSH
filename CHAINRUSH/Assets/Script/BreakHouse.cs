/*=====
<BreakHouse.cs>
└作成者：matsushima

＞内容
家を削除する際の処理を管理するスクリプト

＞更新履歴
Y25   
_M05    
__D
___28:プログラム作成

=====*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakHouse : MonoBehaviour
{
    // 変数宣言
    private GameObject m_breakModel;        // 壊されるモデル
    private bool m_bFell = false;           // 屋根を落としたかどうかのフラグ

    void Start()
    {
        m_breakModel = this.gameObject;
    }

    void Update()
    {
        // 壁が2つ以上破壊されたなら(壁2 + 床 + 屋根 = 4)
        if(m_breakModel.transform.childCount <= 4 && !m_bFell)
        {
            // 瞬間的に下に押し出す
            Transform roof = m_breakModel.transform.Find("house_roof");
            Rigidbody rb = roof.GetComponent<Rigidbody>();
            rb.AddForce(0.0f, -10.0f, 0.0f, ForceMode.Impulse);

            Destroy(m_breakModel, 3.0f);    // 3秒後に削除
            m_bFell = true;     // フラグオン
        }
    }
}
