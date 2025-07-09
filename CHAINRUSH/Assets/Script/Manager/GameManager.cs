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
___20:ゲームオーバー処理を追加
=====*/
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{ get; private set; }

    [System.Serializable]
    public class StageConfig
    {
        public string sceneName;
        public StageEnemyData enemyData;
        public Terrain terrain;
    }

    public List<StageConfig> stages;

    private int currentStageIndex = 0;

    public StageEnemyData CurrentStageData => stages[currentStageIndex].enemyData;
    public Terrain CurrentTerrain => stages[currentStageIndex].terrain;

    [Header("参照オブジェクト")]
    [SerializeField, Tooltip("使用するマップ")]public Terrain terrain;
    [SerializeField, Tooltip("ミニマップのUIパネル")]public RectTransform minimapPanelPrefab;
    [SerializeField, Tooltip("プレイヤートランスフォーム")]public Transform player;
    [SerializeField, Tooltip("ミニマップの敵UI")]public Image enemyIconPrefab;


    private MiniMapIcon miniMapIcon;

    private List<Image> activeEnemyIcones = new List<Image>();

    private Player playerScript; // Playerスクリプト保持用
    public static bool isGameOver = false; // ゲームオーバーフラグ（Resultシーン用にstatic）


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void LoadStage(int index)
    {
        currentStageIndex = index;
        SceneManager.LoadScene("Stage" + (index + 1));
    }

    public void OnAllEnemiesDefeated()
    {
        Debug.Log("全ての敵を倒しました！");
        // リザルトや次のシーンへの遷移をここに
        SceneManager.LoadScene("ResultScene");
    }


    void Start()
    {

        // MiniMapIconスクリプトに target と player を設定
        miniMapIcon = enemyIconPrefab.GetComponent<MiniMapIcon>();
        miniMapIcon.minimapPanel = minimapPanelPrefab;
        miniMapIcon.player = player;

        // Playerスクリプトを取得
        playerScript = player.GetComponent<Player>();

        // ゲームオーバーフラグリセット
        isGameOver = false;
    }

    void Update()
    {
        // Playerのスピード監視
        if (!isGameOver && playerScript != null)
        {
            float speed = playerScript.GetSpeed();
            if (speed <= 0f)
            {
                Debug.Log("ゲームオーバー！");
                isGameOver = true;

                // リザルトへ遷移
                SceneManager.LoadScene("Result");
            }
        }
    }

    //void SpawnEnemies()
    //{
    //    if(totalKilled >= totalkillGoal)
    //    {
    //        Debug.Log("ステージクリア");
    //        CancelInvoke(nameof(SpawnEnemies));

    //        // クリア時は false
    //        isGameOver = false;
    //        // リザルトシーンへ遷移
    //        SceneManager.LoadScene("Result");

    //        return;
    //    }

    //    while(activeEnemies.Count < currentMaxEnemies)
    //    {
    //        Vector3 spawnPos = GetRandomPositionOnTerrain();
    //        GameObject prefab = ChoseEnemyPrefab(); // 割合に応じた敵を生成する
    //        GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(prefab.name, spawnPos, Quaternion.identity);
    //        Image enemyIcon = Instantiate(enemyIconPrefab, spawnPos, Quaternion.identity);
    //        activeEnemies.Add(enemy);
    //        activeEnemyIcones.Add(enemyIcon);
    //        enemyIcon.transform.SetParent(minimapPanelPrefab.transform, false);
    //        miniMapIcon.target = enemy.transform;

    //        // Enemyが倒されたときに通知するスクリプトをアタッチ
    //        Enemy enemyScript = enemy.GetComponent<Enemy>();
    //        if (enemyScript != null)
    //            enemyScript.gamemanager = this;
    //    }


    //}

    //public void OnEnemyKilled(GameObject enemy)
    //{
    //    totalKilled++;
    //    activeEnemies.Remove(enemy);

    //    // 最大出現数を増やす
    //    if(currentMaxEnemies < maxEnemiesLimit)
    //    {
    //        currentMaxEnemies++;
    //    }

    //    Debug.Log($"敵撃破: {totalKilled}/{totalkillGoal} (最大出現数: {currentMaxEnemies})");
    //}

    public void DestroyEnemyIcon(Image enemyIcon)
    {
        activeEnemyIcones.Remove(enemyIcon);
    }


    // 敵を倒した合計数を取得してくる
    //public int GetTotalKilled()
    //{
    //    return totalKilled;
    //}

}
