/*=====
<Enemy.cs>
└作成者：yamamoto

＞内容
Enemyの挙動を管理するスクリプト

＞注意事項

＞更新履歴
Y25   
_M04    
__D     
___23:プログラム作成:yamamoto

=====*/

using UnityEngine;

public class EnemyStick : MonoBehaviour
{

    [Header("エフェクト")]
    [Header("エフェクトCubeのプレハブ")]
    [SerializeField] private GameObject cubePrefab;

    [Header("生成するCubeの数")]
    [SerializeField] private int cubeCount;

    [Header("Cubeの出現範囲")]
    [SerializeField] private float spawnRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GenerateEffectCubes();
            Destroy(gameObject); // 敵自身を破壊
            Debug.Log("プレイヤーに当たった！");
        }
    }

    /*＞エフェクト用Cube生成関数
   引数：なし
   ｘ
   戻値：なし
   ｘ
   概要:指定した半径内のランダムな位置にエフェクト用のキューブを複数生成する
   */
    public void GenerateEffectCubes()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            // 指定された半径内のランダムな位置を算出
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPos = transform.position + randomOffset;

            // キューブを生成
            GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

            // Rigidbody が付いていない場合は追加（念のため）
            if (!cube.TryGetComponent(out Rigidbody rb))
            {
                rb = cube.AddComponent<Rigidbody>();
            }
        }
    }
}
