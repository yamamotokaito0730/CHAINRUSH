// 処理1
/*=====
<FlyToCamera.cs>
└作成者：mori

＞内容
カメラに向かってEffectCubeを飛ばす
一定距離近づいたら張り付かせる

＞注意事項


＞更新履歴
Y25   
_M05
___14:プログラム作成:mori

=====*/

using UnityEngine;
using System.Collections;

public class FlyToCamera : MonoBehaviour
{
    private UnityEngine.Camera mainCamera;
    private Transform cameraTransform;
    private bool hasReachedTarget = false; // 対象位置に到達したかどうかのフラグ（張り付き処理の切り替えに使用）
    private Vector3 localOffset;           // カメラに対するローカルなオフセット位置

    [Header("カメラ手前の距離")]
    [SerializeField] private float stopDistanceFromCamera = 2f;

    [Header("飛んでくる最大スピード")]
    [SerializeField] private float maxFlySpeed = 40f;

    [Header("加速時間")]
    [SerializeField] private float accelerationTime = 0.3f;

    [Header("停止する判定距離")]
    [SerializeField] private float stopThreshold = 0.1f;

    [Header("張り付くときの位置ランダム半径")]
    [SerializeField] private float attachRandomRadius = 0.5f;

    void Start()
    {
        mainCamera = UnityEngine.Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        cameraTransform = mainCamera.transform;

        // 張り付き位置に対するランダムなローカルオフセットを作る
        Vector2 randomCircle = Random.insideUnitCircle * attachRandomRadius;
        localOffset = cameraTransform.forward * stopDistanceFromCamera
                    + cameraTransform.right * randomCircle.x
                    + cameraTransform.up * randomCircle.y;

        // カメラへ向かう動きを開始
        StartCoroutine(FlyToTargetCoroutine());
    }


    /*＞カメラに向かって飛ばす関数
   引数：なし
   ｘ
   戻値：なし
   ｘ
   概要:カメラに向かって徐々に加速しながら接近するコルーチン。
        一定距離まで接近したら停止し、カメラ手前で張り付く。
   */
    private IEnumerator FlyToTargetCoroutine()
    {
        float elapsedTime = 0f;

        while (true)
        {
            if (mainCamera == null) yield break;

            // 張り付き目標のワールド座標
            Vector3 worldTarget = cameraTransform.position + localOffset;

            // 現在位置から目標位置への方向と距離を計算
            Vector3 direction = worldTarget - transform.position;
            float distance = direction.magnitude;

            // 停止する距離に入ったら停止
            if (distance <= stopThreshold)
                break;

            direction.Normalize();

            // 時間経過に応じて速度を加速
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / accelerationTime);
            float currentSpeed = Mathf.Lerp(0f, maxFlySpeed, t);

            // カメラに向かって進む
            transform.position += direction * currentSpeed * Time.deltaTime;

            yield return null;
        }

        // 張り付き状態に移行
        hasReachedTarget = true;
        transform.rotation = cameraTransform.rotation;
        transform.position = cameraTransform.position + localOffset;

        // Rigidbody があれば isKinematic を true にして物理挙動を停止
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // 少し表示を続けてからオブジェクトを削除
        Destroy(gameObject, 0.5f);
    }

    void Update()
    {
        if (hasReachedTarget && mainCamera != null)
        {
            // 張り付いた状態を維持（カメラの前に固定表示）
            // カメラに対するローカル位置を維持する（見た目はピタ止まり）
            transform.position = cameraTransform.position + localOffset;
            transform.rotation = cameraTransform.rotation;
        }
    }
}
