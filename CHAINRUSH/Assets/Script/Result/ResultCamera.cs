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
    private float radius = 15f;
    private float height = 2f;
    private float fixedLookHeight = 2f;
    private float halfOrbitDuration = 2f;

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = UnityEngine.Camera.main.transform;
        StartCoroutine(CameraSequence());
    }

    private IEnumerator CameraSequence()
    {
        // 固定カメラ位置（共通）
        Vector3 firstPos = new Vector3(-0.45f, -1f, 7f);
        Vector3 secondPos = new Vector3(-0.45f, -1f, -7f);

        // ===== 0〜1秒：固定位置で静止 =====
        cameraTransform.position = firstPos;
        // 正面方向を見る（プレイヤーのforward方向に向ける）
        cameraTransform.rotation = Quaternion.LookRotation(-player.forward, Vector3.up);
        yield return new WaitForSeconds(1f);

        // ===== 1〜2秒：同じく固定位置で静止 =====
        cameraTransform.position = secondPos;
        cameraTransform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
        yield return new WaitForSeconds(1f);

        //// ===== 0〜1秒：固定位置で静止 =====
        //cameraTransform.position = firstPos;
        //cameraTransform.LookAt(player.position + Vector3.up * fixedLookHeight);
        //yield return new WaitForSeconds(1f);

        //// ===== 1〜2秒：同じく固定位置で静止 =====
        //cameraTransform.position = secondPos;
        //cameraTransform.LookAt(player.position + Vector3.up * fixedLookHeight);
        //yield return new WaitForSeconds(1f);

        // ===== 2〜4秒：その位置から半周移動 =====
        float t = 0f;

        // 固定位置を基準に、中心点との相対位置を計算
        Vector3 center = player.position;
        Vector3 startOffset = secondPos - center;
        float startAngle = Mathf.Atan2(startOffset.x, startOffset.z) * Mathf.Rad2Deg;

        while (t < 1.0f)
        {
            t += Time.deltaTime / halfOrbitDuration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            float angle = Mathf.Lerp(startAngle, startAngle + 180f, easedT);
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;
            float y = Mathf.Lerp(secondPos.y, targetPos.position.y, easedT);

            Vector3 orbitPos = center + new Vector3(x, 0f, z);
            orbitPos.y = y;

            cameraTransform.position = orbitPos;

            Vector3 lookTarget = player.position + Vector3.up * fixedLookHeight;
            cameraTransform.LookAt(lookTarget);

            yield return null;
        }

        // ===== 最終ターゲット位置に吸着 =====
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

        cameraTransform.position = targetPos.position;
        cameraTransform.rotation = targetPos.rotation;
    }
}



