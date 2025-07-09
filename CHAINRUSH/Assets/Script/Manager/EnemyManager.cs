using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public LayerMask overlapCheckMask;
    public float spawnCheckRadius = 1.0f;

    private StageEnemyData currentStageData;
    private Terrain spawnTerrain;
    private int enemiesAlive = 0;
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
        enemiesAlive--;
        defeatedCount[poolTag]++;

        ObjectPoolManager.Instance.ReturnToPool(poolTag, enemyObj);
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
   //     // ƒQ[ƒ€‚ðˆêŽž’âŽ~‚µ‚Ä‚¨‚­ˆ—
   //     yield return null;
   // }

   // IEnumerator SpawnEnemies()
   // {
   //     // ‚±‚±‚ÅˆêŽž’âŽ~‚Ìƒtƒ‰ƒO‚ð‰º‚°‚é
   //     // ‚»‚ÌŒã‚É“G‚ð¶¬



   //     yield return null;
   // }




}
