using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageEnemyData", menuName = "Scriptable Objects/StageEnemyData")]
public class StageEnemyData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public string poolTag;
        public GameObject enemyPrefab;
        public int maxSpawnCount = 0;
        public float spawnWeight = 0f;
    }

    public List<EnemySpawnInfo> enemySpawnLists;

    [Header("ステージ設定")]
    [Tooltip("ステージ上に生成される上限数")]public int maxStageSpawn = 5;

}
