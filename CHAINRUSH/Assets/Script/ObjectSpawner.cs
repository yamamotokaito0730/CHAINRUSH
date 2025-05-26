/*=====
<ObjectSpawner.cs>
└作成者：matsushima

＞内容
プレイヤーの一定範囲内にあるTerrainのモデルをGameObjectとして処理するためのスクリプト

＞注意事項
配置されている石に関してもTerrainのTreesとして扱う(正確な座標にGameObjectを出力させるため)

＞更新履歴
Y25   
_M05    
__D
___23:プログラム作成:matsushima
___25:石が大量発生する問題を解決:matsushima
___25:BreckNationalObjectからTerrain関係のスクリプトを移動:matsushima
___25:Detailとしていた石もTreesに加えたため、Detailに関するものを削除:matsushima

=====*/
using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    // 変数宣言
    [Header("オブジェクト関連")]
    [SerializeField, Tooltip("テレイン")] public Terrain m_terrain;
    [SerializeField, Tooltip("プレハブ")] public GameObject[] m_treePrefab;

    [Header("座標")]
    [SerializeField, Tooltip("プレイヤー座標")] private Transform m_player;

    private float          m_fSpawnRadius = 1.0f;                      // Terrainで配置されている木や石をGameObjectとして配置する範囲
    private List<Vector3>  m_spawnedPositions = new List<Vector3>();   // GameObjectを出現させるリスト
    private Vector3        m_playerPos;          // プレイヤー座標格納
    private TerrainData    m_terrainData;        // Terrainデータ格納
    private Vector3        m_terrainPos;         // Terrainの座標
    private TreeInstance[] m_originalTree;       // 実行終了後に実行前のTreesに戻す

    void Start()
    {
        // Terrainの情報を入れる
        m_terrainPos = m_terrain.transform.position;     // Terrainの座標
        m_terrainData = m_terrain.terrainData;           // Terrainのデータ
        m_originalTree = m_terrainData.treeInstances;    // 初期の木のデータを保存
    }

    void Update()
    {
        // プレイヤー座標更新
        m_playerPos = m_player.position;

        // GameObject配置
        foreach (TreeInstance tree in m_terrainData.treeInstances)
        {
            // ワールド座標に変換
            Vector3 worldPos = Vector3.Scale(tree.position, m_terrainData.size) + m_terrainPos;

            // 一定距離内に入ったらGameObjectとしてのプレハブを配置
            if (Vector3.Distance(m_playerPos, worldPos) < m_fSpawnRadius && !IsAlreadySpawned(worldPos, 0.5f))
            {
                int index = tree.prototypeIndex;
                if (index >= 0 && index < m_treePrefab.Length)
                {
                    GameObject newTree = Instantiate(m_treePrefab[index], worldPos, Quaternion.identity);
                    m_spawnedPositions.Add(worldPos);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        // UnityエディタでPlayモード終了時に木を元の状態に復元
#if UNITY_EDITOR
        m_terrain.terrainData.treeInstances = m_originalTree;
#endif
    }

    /*IsAlreadySpawned関数
    引数１：vector3 _pos：ポジション
    引数２：float   _proximityThreshold：近接判定のしきい値
    ｘ
    戻値：既にGameObjectが生成されているかどうか
    ｘ
    概要：配置しようとしているモデルのGameObjectが既に配置されているかを判別する
    */
    private bool IsAlreadySpawned(Vector3 _pos, float _proximityThreshold)
    {
        foreach (Vector3 p in m_spawnedPositions)
        {
            // 一定範囲内にオブジェクトが生成されていないならtrueを返す
            if (Vector3.Distance(p, _pos) < _proximityThreshold) return true;
        }
        return false;
    }

    /*RemoveNearbyTerrainTrees関数
   引数１：Vector3 _position：モデルの座標
   引数２：float _radius    ：TerrainのTreesを消す範囲
   ｘ
   戻値：なし
   ｘ
   概要：GameObjectに置き換わったTerrainのTreesを消す関数
   */
    public void RemoveNearbyTerrainTrees(Vector3 _position, float _radius)
    {
        // 消すTerrainのオブジェクトを除いた、新しい配列を用意
        List<TreeInstance> newTrees = new List<TreeInstance>();

        // Terrainで配置されたTreesのぶん判定をとる
        foreach (TreeInstance tree in m_terrainData.treeInstances)
        {
            // ワールド座標に変換
            Vector3 worldPos = Vector3.Scale(tree.position, m_terrainData.size) + m_terrainPos;

            // 消す範囲外なら配列に加える
            if (Vector3.Distance(worldPos, _position) > _radius)
            {
                newTrees.Add(tree); // 残すTrees
            }
        }
        // データのインスタンスに格納
        m_terrainData.treeInstances = newTrees.ToArray();
    }
}
