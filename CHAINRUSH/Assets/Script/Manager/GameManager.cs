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
_M07
__D
___18:currentStageIndexをstaticに変更:mori
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
    public static GameManager Instance { get; private set; }
    public static bool IsGameActive { get; private set; } = false;  //ゲームの状態管理用

    [System.Serializable]
    public class StageConfig
    {
        public string sceneName;
        public StageEnemyData enemyData;
        public Terrain terrain;
    }

    public List<StageConfig> stages;

    public static int currentStageIndex = 1;

    public StageEnemyData CurrentStageData => stages[currentStageIndex].enemyData;
    public Terrain CurrentTerrain => stages[currentStageIndex].terrain;

    private Player playerScript; // Playerスクリプト保持用

    public static bool isGameOver = false; // ゲームオーバーフラグ（Resultシーン用にstatic）

    [SerializeField, Tooltip("プレイヤートランスフォーム")] public Transform player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        // Playerスクリプトを取得
        playerScript = player.GetComponent<Player>();


        // フラグリセット
        isGameOver = false;
        IsGameActive = false;
    }

    void Update()
    {
        // プレイヤーオブジェクトのplayerスクリプトを取得
        playerScript = GameObject.FindWithTag("Player")?.GetComponent<Player>();

        // Playerのスピード監視
        if (!isGameOver && playerScript != null)
        {
            float speed = playerScript.GetSpeed();
            Debug.Log(playerScript.GetSpeed());
            if (speed <= 0f)
            {
                Debug.Log("ゲームオーバー！");
                isGameOver = true;

                // リザルトへ遷移
                SceneManager.LoadScene("Result");
            }
        }
    }

    public void LoadStage(int index)
    {
        currentStageIndex = index;
        SceneManager.LoadScene("Stage" + (index));
    }

    public void OnAllEnemiesDefeated()
    {
        Debug.Log("全ての敵を倒しました！");
        // リザルトや次のシーンへの遷移をここに
        ObjectPoolManager.Instance.ReturnAllToPool();
        SceneManager.LoadScene("Result");
    }

    public void StartGame()
    {
        IsGameActive = true;
    }

    public void StopGameTemporarily()
    {
        IsGameActive = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage1" || scene.name == "Stage2" || scene.name == "Stage3")
        {
            // Playerスクリプトを取得
            playerScript = player.GetComponent<Player>();

            isGameOver = false;
            IsGameActive = false;
        }
    }
}