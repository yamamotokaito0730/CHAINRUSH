/*=====
<ObjectPoolManager.cs> 
└作成者：banno

＞内容
オブジェクトプールを一元管理するためのマネージャースクリプト

＞注意事項   


＞更新履歴
Y25   
_M05    
__D       
___31:プログラム作成:banno    
_M06
___2:管理形態を変更:banno
___4:nullエラーの対処:banno
___6:プールの格納・取り出し処理を追加
___8:格納・取り出し処理を一元管理できるように関数化
___10:外部からの参照をできるようにシングルトン化
___11:インターフェイスの追加、オブジェクト共通で処理を行えるように改良
=====*/


using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoBehaviour
{
    [Tooltip("シングルトン用の呼び出し変数")] public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    [Tooltip("プール1個単位のクラス")]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [Tooltip("全てのプールを格納する用データ")]
    //public List<Pool> pools;
    public PoolListData poolData;
    [Tooltip("プールで生成するオブジェクト登録用辞書")]
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // シングルトンを行うための宣言
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複を避ける
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        //InitializedPool();

    }

    void Start()
    {

    }

    private void InitializedPool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in poolData.pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }

    }


    /*＞SpawnFromPool関数
       引数：string : タグ名 , Vecto3 : 生成座標 , Quaternion : 生成時の回転角度
       ｘ
       戻値：GameObject : プールから取り出したオブジェクト
       ｘ
       概要：プールからオブジェクトを取り出して生成する関数
       */
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // 登録されているプールオブジェクトのタグが存在するかどうか
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + "というタグは存在しません");
            return null;
        }

        // 生成されているオブジェクトを取り出す
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);                  // オブジェクトを表示に変更
        objectToSpawn.transform.position = position;    // 生成位置
        objectToSpawn.transform.rotation = rotation;    // オブジェクトの回転角度

        // オブジェクトを取り出したときに行う処理をオブジェクトごとに実行する
        IPool poolable = objectToSpawn.GetComponent<IPool>();
        if (poolable != null)
        {
            poolable.OnSpawn(); // オブジェクトがプールから取り出されたら呼ばれる処理
        }

        return objectToSpawn;
    }

    /*＞RetunToPool関数
       引数：string : タグ名 , GameObject : プールから取り出したオブジェクト 
       ｘ
       戻値：なし
       ｘ
       概要：プールから取り出したオブジェクトをプールに戻す関数
       */
    public void ReturnToPool(string tag, GameObject gameObject)
    {
        // オブジェクトを戻すときに行う処理をオブジェクトごとに実行する
        IPool poolable = gameObject.GetComponent<IPool>();
        if (poolable != null)
        {
            poolable.OnReturn();  // オブジェクトがプールに戻るときに呼ばれる関数
        }

        gameObject.SetActive(false);                // 使用後のオブジェクトを非表示に
        poolDictionary[tag].Enqueue(gameObject);    // プールに戻す
    }

    public void ReturnAllToPool()
    {
        Debug.Log(poolDictionary.Count);
        foreach (var tag in poolDictionary.Keys)
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects)
            {
                if (!obj.activeInHierarchy) continue;

                // 名前にタグが含まれていれば（例："Enemy" タグ → "Enemy(Clone)"）
                if (obj.name.Contains(tag))
                {
                    ReturnToPool(tag, obj);
                }
            }
        }
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
        if(scene.name == "Stage1" || scene.name == "Stage2" || scene.name == "Stage3")
        {
            InitializedPool();
        }

    }


}
