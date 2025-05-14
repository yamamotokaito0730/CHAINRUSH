/*=====
<FlyToCamera.cs>
└作成者：mori

＞内容
カメラに向かって飛んできて、一定距離になったら張り付く

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___14:プログラム作成:mori

=====*/
using System.Collections;
using UnityEngine;

public class FlyToCamera : MonoBehaviour
{
    private UnityEngine.Camera mainCamera;

    /*＞StartFly関数
    引数：UnityEngine.Camera camera:メインのカメラ
    ｘ
    戻値：なし
    ｘ
    概要:外部から呼び出してカメラを設定し、処理を開始
    */
    public void StartFly(UnityEngine.Camera camera)
    {
        mainCamera = camera;

        if (mainCamera != null)
        {
            // カメラに向かって飛ぶ＆張り付き処理開始
            StartCoroutine(FlyAndStick(camera));
        }
        else
        {
            Debug.LogError("カメラが設定されていません");
        }
    }

    /*＞FlyAndStick関数
    引数：UnityEngine.Camera camera:メインのカメラ
    ｘ
    戻値：なし
    ｘ
    概要:カメラに向かって飛ぶ＆張り付き処理
    */
    private IEnumerator FlyAndStick(UnityEngine.Camera camera)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) yield break;

        // 物理停止＆親子付け
        rb.isKinematic = true;
        transform.SetParent(camera.transform);

        Vector3 startLocalPos = camera.transform.InverseTransformPoint(transform.position);

        // ランダムなポジション設定
        int randX = Random.Range(0, 2) * 2 - 1;
        int randY = Random.Range(0, 2) * 2 - 1;
        Vector3 targetLocalPos = new Vector3(randX, randY, 2); // カメラ前方のローカル位置

        float duration = 0.3f;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;
            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }
        FindObjectOfType<Camera>().ShakeCamera(0.1f, 0.3f); // 0.5秒間、強さ0.3で揺らす
        // 張り付いたまま1秒表示
        yield return new WaitForSeconds(1.0f);

        Destroy(gameObject);
    }
}
