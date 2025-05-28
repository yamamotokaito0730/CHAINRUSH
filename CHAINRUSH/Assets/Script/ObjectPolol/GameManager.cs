// ゲームシーンでの制御・管理を行うスクリプト
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Terrain terrain;
    public GameObject enemyPrefab;
    public int totalkillGoal = 20;      // ステージクリアのための目標数
    public int initialMaxEnemies = 5;   // ステージの最初に湧く敵の数
    public int maxEnemiesLimit = 8;     // ステージに敵が湧く最大数

    private int currentMaxEnemies;      // 現在の最大湧き数
    private int totalKilled = 0;        // 倒した敵の合計数

    private List<GameObject> activeEnemies = new List<GameObject>();

    
    void Start()
    {
        currentMaxEnemies = initialMaxEnemies;
        InvokeRepeating(nameof(SpawnEnemies), 1f, 1f);  // 1秒ごとにスポーンチェック
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
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);

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
    
    Vector3 GetRandomPositionOnTerrain()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        float x = Random.Range(0f, data.size.x);
        float z = Random.Range(0f, data.size.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;


        return new Vector3(x + terrainPos.x, y, z + terrainPos.z);
    }

}
