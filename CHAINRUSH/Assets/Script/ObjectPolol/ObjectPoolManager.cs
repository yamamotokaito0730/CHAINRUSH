using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        //foreach(Pool pool in pools)
        //{
        //    poolDictionary = 
        //}


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
