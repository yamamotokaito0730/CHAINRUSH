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
_M06
__D
___3:木と岩をプールで管理できるように変更:banno
___5:パーティクルもプールに対応:banno
___7:プールに格納されない原因を解消:banno
___11:パーティクルの処理が複数実行されない問題を解消:banno
_M07
__D
___27:プレイヤーとの当たり判定をOnCollisionEnterからOnTriggerEnterに変更:tooyama
=====*/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BreakNationalObject : MonoBehaviour, IPool
{

    public NationalObject objectData;       // オブジェクトのデータをScriptabeObjectから取得

    private ObjectSpawner objectSpawner;    // オブジェクトスポナークラス
    private Player player;                  // プレイヤークラス
    void Start()
    {

        // プレイヤーに入っているスクリプトを参照
        GameObject playerObject = GameObject.FindWithTag("Player");
        player = playerObject.GetComponent<Player>();

        // terrainに入っているスクリプトを参照
        GameObject terrainObject = GameObject.FindWithTag("MainTerrain");   // タグが付いたTerrainを取得
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        objectSpawner = terrain.GetComponent<ObjectSpawner>();
    }

    /*＞OnCollisionEnter関数
       引数：Collision : 当たるオブジェクトのコライダー
       ｘ
       戻値：なし
       ｘ
       概要：プレイヤーがオブジェクトにぶつかった際の処理を行う
       */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerAttackCollider" && player.GetState() >= 3)    // プレイヤーの速度が3以上なら
        {
            Break();
        }
    }

    /*＞Break関数
       引数：なし
       ｘ
       戻値：なし
       ｘ
       概要：木や岩のオブジェクトの破壊処理
            ※もう少しきれいにコードをかけると思うので、後に変更予定
       */
    private void Break()
    {
        // モデルの情報を取得
        Vector3 modelPosition = this.transform.position;  // 座標

        // Terrain上の木を削除
        objectSpawner.RemoveNearbyTerrainTrees(modelPosition, 2.0f);

        switch (objectData.type)
        {
            //-----------------------------------------
            // 木1の破壊処理
            case TypeObject.Tree:

                // 元のモデルを削除
                ObjectPoolManager.Instance.ReturnToPool("Conifer", gameObject);

                SEManager.Instance.Play("Tree");
                // 切断したプレハブを出現
                GameObject tree_up =
                    ObjectPoolManager.Instance.SpawnFromPool(
                        "Tree_Upper", modelPosition + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
                GameObject tree_botom =
                    ObjectPoolManager.Instance.SpawnFromPool(
                        "Tree_Stump", modelPosition, Quaternion.identity);

                // 各パーティクルのPrefabを生成して再生
                foreach (GameObject prefab in objectData.m_particle)
                {
                    GameObject psObj =
                        ObjectPoolManager.Instance.SpawnFromPool(prefab.name, modelPosition, Quaternion.identity);
                    ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.transform.position = modelPosition + objectData.m_shiftPosition;
                        ps.Play();
                    }
                    // それぞれのパーティクルをプールに戻る処理を行う
                    //breakObj = psObj.GetComponent<BreakObject>();
                    //breakObj.Start();
                }
                break;

            //-----------------------------------------
            // 岩1の破壊処理
            case TypeObject.Rock1:

                // 元のモデルを削除
                ObjectPoolManager.Instance.ReturnToPool("Rock_C_01", gameObject);
                SEManager.Instance.Play("Rock");

                // 切断したプレハブを出現
                GameObject rock1_dis1 =
                    ObjectPoolManager.Instance.SpawnFromPool(
                        "Rock_Disconnect", modelPosition + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
                GameObject rock1_dis2 =
                    ObjectPoolManager.Instance.SpawnFromPool(
                        "Rock_Disconnect", modelPosition, Quaternion.identity);

                // 各パーティクルのPrefabを生成して再生
                foreach (GameObject prefab in objectData.m_particle)
                {
                    GameObject psObj =
                        ObjectPoolManager.Instance.SpawnFromPool(prefab.name, modelPosition, Quaternion.identity);
                    //Instantiate(prefab, modelPosition, Quaternion.identity);
                    ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.transform.position = modelPosition + objectData.m_shiftPosition;
                        ps.Play();
                    }
                }

                break;

            default:
                break;
        }
    }

    public void OnSpawn()
    {
    }
    public void OnReturn()
    {

    }

}
