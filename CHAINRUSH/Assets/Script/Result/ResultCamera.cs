/*=====
<ResultCamera.cs>
└作成者：mori

＞内容
リザルトのカメラ処理

＞注意事項


＞更新履歴
Y25         
_M07
__D  
___04:プログラム作成:mori

=====*/

using UnityEngine;
using System.Collections;

public class ResultCamera : MonoBehaviour
{
    public Transform player;
    public Transform targetPos;
    public float radius = 5f;
    public float height = 2f;
    public float fixedLookHeight = 2f;
    public float duration = 3f;

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = UnityEngine.Camera.main.transform;
        StartCoroutine(CameraSequence());
    }

    private IEnumerator CameraSequence()
    {
        // カメラの開始位置：前方やや下
        Vector3 startOffset = player.forward * 1.0f + Vector3.down * 1.0f;
        Vector3 startPos = player.position + startOffset + Vector3.up * height;

        // 終点：背後位置（targetPosに沿う）
        Vector3 endPos = targetPos.position;

        float t = 0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            // 半周の角度（前→後ろ）
            float angle = Mathf.Lerp(0f, 180f, easedT);
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;
            float y = Mathf.Lerp(startPos.y, endPos.y, easedT); // Y補間

            Vector3 orbitPos = player.position + new Vector3(x, 0f, z);
            orbitPos.y = y;

            cameraTransform.position = orbitPos;

            // 見る対象：プレイヤーの固定高さ（例：1.5m上）
            Vector3 lookTarget = player.position + Vector3.up * fixedLookHeight;
            cameraTransform.LookAt(lookTarget);

            yield return null;
        }

        // 最終位置・角度に滑らかに吸着
        float snapTime = 0.3f;
        float snapT = 0f;
        Vector3 fromPos = cameraTransform.position;
        Quaternion fromRot = cameraTransform.rotation;

        while (snapT < 1.0f)
        {
            snapT += Time.deltaTime / snapTime;
            float easedSnapT = Mathf.SmoothStep(0f, 1f, snapT);
            cameraTransform.position = Vector3.Lerp(fromPos, targetPos.position, easedSnapT);
            cameraTransform.rotation = Quaternion.Slerp(fromRot, targetPos.rotation, easedSnapT);
            yield return null;
        }

        // 完全にターゲット位置に固定
        cameraTransform.position = targetPos.position;
        cameraTransform.rotation = targetPos.rotation;
    }
}

