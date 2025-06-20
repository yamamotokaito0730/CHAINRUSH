/*=====
<GameManager.cs> 
└作成者：banno

＞内容
ゲームシーンでの制御・管理を行うスクリプト

＞注意事項   


＞更新履歴
Y25   
_M05
__D
___26:スクリプトを作成:banno
_M06
__D
___09:スクリプトの
___17:スクリプトのエネミー生成部分を変更:banno
___19:
=====*/
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        [SerializeField, Tooltip("敵のタイプ")]public GameObject prefab;
        //[Range(0f, 1f)]
        [SerializeField, Tooltip("敵の出現割合")] public float spawnRate;
    }

    [Header("敵の種類と出現割合")]
    [SerializeField, Tooltip("敵の種類と出現率を含むクラスのリスト")]public List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("参照オブジェクト")]
    [SerializeField, Tooltip("使用するマップ")]public Terrain terrain;
    [SerializeField, Tooltip("ミニマップのUIパネル")]public RectTransform minimapPanelPrefab;
    [SerializeField, Tooltip("プレイヤートランスフォーム")]public Transform player;
    [SerializeField, Tooltip("ミニマップの敵UI")]public Image enemyIconPrefab;

    [Header("敵の生成、管理で扱う変数")]
    [SerializeField, Tooltip("ステージクリアのための目標数")]public int totalkillGoal = 20;
    [SerializeField, Tooltip("ステージの最初に湧く敵の数")] public int initialMaxEnemies = 5;
    [SerializeField, Tooltip("ステージに敵が湧く最大数")] public int maxEnemiesLimit = 8;
    private int currentMaxEnemies;  // 現在の最大湧き数
    private int totalKilled = 0;    // 倒した敵の合計数


    private MiniMapIcon miniMapIcon;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<Image> activeEnemyIcones = new List<Image>();

    

    void Start()
    {
        currentMaxEnemies = initialMaxEnemies;
        InvokeRepeating(nameof(SpawnEnemies), 1f, 1f);  // 1秒ごとにスポーンチェック

        // MiniMapIconスクリプトに target と player を設定
        miniMapIcon = enemyIconPrefab.GetComponent<MiniMapIcon>();
        miniMapIcon.minimapPanel = minimapPanelPrefab;
        miniMapIcon.player = player;
    }

    void SpawnEnemies()
    {
        if(totalKilled >= totalkillGoal)
        {
            Debug.Log("ステージクリア");
            CancelInvoke(nameof(SpawnEnemies));
            return;
        }

        while(activeEnemies.Count < currentMaxEnemies)
        {
            Vector3 spawnPos = GetRandomPositionOnTerrain();
            GameObject prefab = ChoseEnemyPrefab(); // 割合に応じた敵を生成する
            GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(prefab.name, spawnPos, Quaternion.identity);
            Image enemyIcon = Instantiate(enemyIconPrefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);
            activeEnemyIcones.Add(enemyIcon);
            enemyIcon.transform.SetParent(minimapPanelPrefab.transform, false);
            miniMapIcon.target = enemy.transform;

            // Enemyが倒されたときに通知するスクリプトをアタッチ
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.gamemanager = this;
        }


    }

    public void OnEnemyKilled(GameObject enemy)
    {
        totalKilled++;
        activeEnemies.Remove(enemy);

        // 最大出現数を増やす
        if(currentMaxEnemies < maxEnemiesLimit)
        {
            currentMaxEnemies++;
        }

        Debug.Log($"敵撃破: {totalKilled}/{totalkillGoal} (最大出現数: {currentMaxEnemies})");
    }

    public void DestroyEnemyIcon(Image enemyIcon)
    {
        activeEnemyIcones.Remove(enemyIcon);
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        float x = Random.Range(0f, data.size.x);
        float z = Random.Range(0f, data.size.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;


        return new Vector3(x + terrainPos.x, y, z + terrainPos.z);
    }

    GameObject ChoseEnemyPrefab()
    {
        float total = 0f;
        foreach (var e in enemyTypes)
        {
            total += e.spawnRate;
        }

        float rand = Random.Range(0f, 1f);
        float accum = 0f;
        foreach (var e in enemyTypes)
        {
            accum += e.spawnRate;
            if (rand <= accum)
            {
                return e.prefab;
            }
        }

        // 設定ミスまたは、上の処理に入らなかった場合に返す
        return enemyTypes[0].prefab;
    }

    // 敵を倒した合計数を取得してくる
    public int GetTotalKilled()
    {
        return totalKilled;
    }

}
