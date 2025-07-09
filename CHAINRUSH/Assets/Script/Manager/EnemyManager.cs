/*=====
<EnemyManager.cs> 
└作成者：banno

＞内容
ゲームシーンでの敵の管理を行うスクリプト

＞注意事項   
基本的に変更するような値は、Inspectorで触れるようにしているので、
スクリプトの中身の値などはあまり変更しないでください

＞更新履歴
Y25   
_M07    
__D4:スクリプトを作成
__D5:GameManagerと併用できるようにプログラムを追加
__D7:生成は完了、回収時にエラーが出るので後日それの対応
__D9:回収時のエラー修復、敵が増える仕様の追加

=====*/

using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public LayerMask overlapCheckMask;
    public float spawnCheckRadius = 1.0f;

    private StageEnemyData currentStageData;
    private Terrain spawnTerrain;
    private int enemiesAlive = 0;
    private int totalDefeated;
    private Dictionary<string, int> spawnedCount = new();
    private Dictionary<string, int> defeatedCount = new();

    public static EnemyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        currentStageData = GameManager.Instance.CurrentStageData;
        spawnTerrain = GameManager.Instance.CurrentTerrain;

        foreach (var info in currentStageData.enemySpawnLists)
        {
            spawnedCount[info.poolTag] = 0;
            defeatedCount[info.poolTag] = 0;
        }

        SpawnInitialEnemies();

       

        //StartCoroutine(EnemySpawnAndWait());
    }

    void SpawnInitialEnemies()
    {
        while (enemiesAlive < currentStageData.maxStageSpawn)
        {
            TrySpawnEnemyByWeight();
        }

    }

    void TrySpawnEnemyByWeight()
    {
        var selected = ChooseEnemyInfoByWeight();
        if (selected == null) return;

        if (SpawnHelper.TryGetValidSpawnPoint(spawnTerrain, overlapCheckMask, spawnCheckRadius, out Vector3 spawnPos))
        {
            GameObject enemyObj = ObjectPoolManager.Instance.SpawnFromPool(selected.poolTag, spawnPos, Quaternion.identity);
            if (enemyObj == null) return;

            spawnedCount[selected.poolTag]++;
            enemiesAlive++;

        }
    }

    StageEnemyData.EnemySpawnInfo ChooseEnemyInfoByWeight()
    {
        List<StageEnemyData.EnemySpawnInfo> candidates = new();

        foreach(var info in currentStageData.enemySpawnLists)
        {
            if (spawnedCount[info.poolTag] < info.maxSpawnCount)
                candidates.Add(info);
        }

        if (candidates.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var info in candidates)
        {
            totalWeight += info.spawnWeight;
        }

        float rand = Random.Range(0f, totalWeight);
        float accum = 0f;

        foreach (var info in candidates)
        {
            accum += info.spawnWeight;
            if (rand <= accum)
                return info;
        }

        return candidates[0];
    }

    public void OnEnemyDefeated(GameObject enemyObj, string poolTag)
    {
        string cleanTag = NameUtility.GetCleanName(enemyObj);
        poolTag = cleanTag;

        enemiesAlive--;
        defeatedCount[poolTag]++;
        totalDefeated++;
        
        ObjectPoolManager.Instance.ReturnToPool(cleanTag, enemyObj);

        // スポーン数の段階的増加処理
        if (totalDefeated % currentStageData.increaseThreshold == 0)
        {
            int before = currentStageData.maxStageSpawn;
            currentStageData.maxStageSpawn += currentStageData.increaseAmount;
            currentStageData.maxStageSpawn = Mathf.Min(
                currentStageData.maxStageSpawn,
                currentStageData.maxLimitEnemies
            );

            int increasedBy = currentStageData.maxStageSpawn - before;

            // 新たにスポーンできる枠があれば即座にスポーン
            for (int i = 0; i < increasedBy; i++)
            {
                TrySpawnEnemyByWeight();
            }
        }

        TrySpawnEnemyByWeight();

        bool allDone = true;
        foreach (var info in currentStageData.enemySpawnLists)
        {
            if (defeatedCount[info.poolTag] < info.maxSpawnCount)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            GameManager.Instance.OnAllEnemiesDefeated();
        }
    }

    public int GetEnemiesDefeatedTotal()
    {
        int sum = 0;
        foreach (var kv in defeatedCount)
        {
            sum += kv.Value;
        }
        return sum;
    }



   // IEnumerator EnemySpawnAndWait()
   // {
   //     yield return GameStop();
   //     yield return new WaitForSeconds(5f);
   //     yield return SpawnEnemies();
   // }

   //IEnumerator GameStop()
   // {
   //     // ゲームを一時停止しておく処理
   //     yield return null;
   // }

   // IEnumerator SpawnEnemies()
   // {
   //     // ここで一時停止のフラグを下げる
   //     // その後に敵を生成



   //     yield return null;
   // }




}
