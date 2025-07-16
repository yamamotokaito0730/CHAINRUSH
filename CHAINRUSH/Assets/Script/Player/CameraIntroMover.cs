using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class CameraIntroMover : MonoBehaviour
{
    public Transform player;
    public float easingDuration = 1f;
    public float height = 5f;
    public float fixedLookHeight = 5f;
    public GameManager gameManager;
    public UIManager UIManager;
    public Camera camera;
    public Vector3 CameraPos= Vector3.zero;
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = UnityEngine.Camera.main.transform;
        StartCoroutine(CameraSequence());
    }

    private IEnumerator CameraSequence()
    {
        Vector3[] points =
        {
            player.forward * 5.0f,  // 前
            player.right * 5.0f,     // 右
            -player.right * 5.0f,    // 左
            -player.forward * 5.0f    // 後ろ
        };

        // 1. 前 → 右（イージング）
        yield return MoveCameraSmoothly(points[0], points[1]);

        // 2. 右 → 左（瞬間切り替え）
        SetCameraInstantly(points[2]);
        yield return new WaitForSeconds(0.3f); // 少し止める演出

        // 3. 左 → 後ろ（イージング）
        yield return MoveCameraSmoothly(points[2], points[3]);

        yield return MoveCameraSmoothly2(points[3], CameraPos);

        // 4. 演出終了 → ゲーム開始
        yield return new WaitForSeconds(0.5f);
       
        cameraTransform.position = player.position+CameraPos;
        cameraTransform.LookAt(player.position + Vector3.up * 1.5f);
        gameManager.StartGame();
        UIManager.ShowUI();
    }

    private IEnumerator MoveCameraSmoothly(Vector3 fromOffset, Vector3 toOffset)
    {
        Vector3 startPos = player.position + fromOffset + Vector3.up * height;
        Vector3 endPos = player.position + toOffset + Vector3.up * height;
        float t = 0.0f;
        Vector3 lerped = Vector3.zero;
        while (t < 1.0f)
        {
            t += Time.deltaTime / easingDuration;
            float easedT = Mathf.SmoothStep(0.0f, 1.0f, t);
            
            lerped = Vector3.Lerp(startPos, endPos, easedT);
            lerped.y = height;

            cameraTransform.position = lerped;

            Vector3 lookTarget = player.position;
            lookTarget.y = fixedLookHeight; // ← プレイヤーの頭や体の高さに固定

            //cameraTransform.LookAt(lookTarget);
            cameraTransform.LookAt(lookTarget + Vector3.up * 1.5f);
            yield return null;
        }
    }

    private IEnumerator MoveCameraSmoothly2(Vector3 fromOffset, Vector3 toOffset)
    {
        Vector3 startPos = player.position + fromOffset ;
        Vector3 endPos = player.position + toOffset ;
        float t = 0.0f;
        Vector3 lerped = Vector3.zero;
        while (t < 1.0f)
        {
            t += Time.deltaTime / easingDuration;
            float easedT = Mathf.SmoothStep(0.0f, 1.0f, t);

            lerped = Vector3.Lerp(startPos, endPos, easedT);

            cameraTransform.position = lerped;

            Vector3 lookTarget = player.position;
            lookTarget.y = fixedLookHeight; // ← プレイヤーの頭や体の高さに固定

            //cameraTransform.LookAt(lookTarget);
            cameraTransform.LookAt(lookTarget + Vector3.up * 1.5f);
            yield return null;
        }
    }



    private void SetCameraInstantly(Vector3 offset)
    {
        Vector3 pos = player.position + offset;
        pos.y = height;
        
        cameraTransform.position = pos;

        cameraTransform.LookAt(player.position);
    }
}
