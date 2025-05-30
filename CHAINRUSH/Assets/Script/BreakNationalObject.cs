/*=====
<BreakNationalObject.cs>
└作成者：matsushima

＞内容
木のモデルとプレイヤーが当たったときの処理を管理するスクリプト

＞更新履歴
Y25   
_M05    
__D
___21:プログラム作成:matsushima
___23:スクリプト名、一部変数名変更:matsushima
___25:Terrainを参照するスクリプトをObjectSpawnerに移動:matsushima

=====*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakNationalObject : MonoBehaviour
{
    // 変数宣言
    [Header("パーティクル")]
    [SerializeField, Tooltip("表示するパーティクル")] public GameObject[] m_particle;
    [SerializeField, Tooltip("モデルからずらす分の座標")] public Vector3 m_shiftPosition;

    [Header("オブジェクト")]
    [SerializeField, Tooltip("切断されたプレハブの上部")] public GameObject m_upperDisconnectedPrefab;
    [SerializeField, Tooltip("切断されたプレハブの下部")] public GameObject m_bottomDisconnectedPrefab;

    private GameObject m_breakModel;        // 壊されるモデル
    private ObjectSpawner objectSpawner;    // オブジェクトスポナークラス
    private Player player;                  // プレイヤークラス

    void Start()
    {
        m_breakModel = this.gameObject;

        // プレイヤーに入っているスクリプトを参照
        GameObject playerObject = GameObject.FindWithTag("Player");
        player = playerObject.GetComponent<Player>();

        // terrainに入っているスクリプトを参照
        GameObject terrainObject = GameObject.FindWithTag("MainTerrain");   // タグが付いたTerrainを取得
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        objectSpawner = terrain.GetComponent<ObjectSpawner>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player" && player.GetState() >= 3)    // プレイヤーの速度が3以上なら
        {
            // モデルの情報を取得
            Vector3 modelPosition = m_breakModel.transform.position;    // 座標

            // Terrain上の木を削除
            objectSpawner.RemoveNearbyTerrainTrees(modelPosition, 2.0f);

            // 元のモデルを削除
            Destroy(m_breakModel);

            // 切断したプレハブを出現
            GameObject prefab1 =
                Instantiate(m_upperDisconnectedPrefab, modelPosition + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
            GameObject prefab2 =
                Instantiate(m_bottomDisconnectedPrefab, modelPosition, Quaternion.identity);
            Destroy(prefab1, 3.0f);  // 3秒後に削除
            Destroy(prefab2, 3.0f);

            // 各パーティクルのPrefabを生成して再生
            foreach (GameObject prefab in m_particle)
            {
                GameObject psObj = Instantiate(prefab, modelPosition, Quaternion.identity);
                ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.transform.position = modelPosition + m_shiftPosition;
                    ps.Play();
                }
            }
        }
    }
}
