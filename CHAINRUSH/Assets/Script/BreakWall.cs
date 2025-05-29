/*=====
<BreakWall.cs>
└作成者：matsushima

＞内容
家の壁のモデルとプレイヤーが当たったときの処理を管理するスクリプト

＞更新履歴
Y25   
_M05    
__D
___28:プログラム作成

=====*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    // 変数宣言
    [Header("オブジェクト")]
    [SerializeField, Tooltip("切断されたプレハブ")] public GameObject[] m_disconnectedPrefab;

    private GameObject m_breakModel;        // 壊されるモデル

    void Start()
    {
        m_breakModel = this.gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // モデルの情報を取得
            Vector3 modelPosition = m_breakModel.transform.position;    // 座標

            // 元のモデルを削除
            Destroy(m_breakModel);

            // 切断されたプレハブを出す
            foreach (GameObject prefab in m_disconnectedPrefab)
            {
                GameObject fragment = Instantiate(prefab, modelPosition, Quaternion.identity);
                Destroy(fragment, 3.0f);    // 3秒後に削除
            }
        }
    }
}
