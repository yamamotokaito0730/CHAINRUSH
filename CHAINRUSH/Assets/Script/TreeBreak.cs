/*=====
<TreeBreak.cs>
└作成者：matsushima

＞内容
木のモデルとプレイヤーが当たったときの処理を管理するスクリプト

＞更新履歴
Y25   
_M05    
__D
___21:プログラム作成:matsushima

=====*/
using System.Collections;
using UnityEngine;

public class TreeBreak : MonoBehaviour
{
    // 変数宣言
    [Header("オブジェクト")]
    [SerializeField, Tooltip("パーティクル")] public GameObject[] m_particle;   // パーティクルのPrefab
    GameObject m_treeModel;

    void Start()
    {
        m_treeModel = this.gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            // 木の情報を取得
            Vector3 treePosition = m_treeModel.transform.position;    // 座標

            // 元の木のモデルを削除
            Destroy(m_treeModel);

            // 各パーティクルのPrefabを生成して再生
            foreach (GameObject prefab in m_particle)
            {
                GameObject psObj = Instantiate(prefab, treePosition, Quaternion.identity);
                ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.position = treePosition + new Vector3(0.0f, 5.0f, 0.0f);
                    ps.Play();
                }
            }
        }
    }
}
