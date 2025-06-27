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
___15:カメラの振動呼び出し＆張り付き場所変更:yamamoto

=====*/
using System.Collections;
using UnityEngine;
using static Player;
using UnityEngine.Playables;

public class FlyToCamera : MonoBehaviour
{
   // private UnityEngine.Camera mainCamera;
    private Camera mainCamera;
    private float PlayerSpeed;  //プレイヤー速度保持用

    private int[] m_Type = { 5, 29, 50 };       //張り付き後にどのように画面外に行くかを判別する配列
    private int[] m_Speed = { 11, 20, 35,50 };      //画面に張り付いたりはけたりするときの速度を決める配列
    /// //////////////////////////
    public float harituki;
    public float tobukyori;
    public float tobujikan;
    /////////////////////////////

    /*＞StartFly関数
    引数：UnityEngine.Camera camera:メインのカメラ
    ｘ
    戻値：なし
    ｘ
    概要:外部から呼び出してカメラを設定し、処理を開始
    */
    public void StartFly(UnityEngine.Camera _camera,float _PlayerSpeed)
    {

        mainCamera = _camera.GetComponent<Camera>();
        PlayerSpeed = _PlayerSpeed;

        if (mainCamera != null)
        {
            // カメラに向かって飛ぶ＆張り付き処理開始
            StartCoroutine(FlyAndStick(_camera));
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
        //int randX = Random.Range(0, 2) * 2 - 1;
        //int randY = Random.Range(0, 2) * 2 - 1;
        float randX;
        float randY;
        do
        {
            randX = Random.Range(-1.0f, 1.0f);
            randY = Random.Range(-1.0f, 1.0f);
        } while (Mathf.Abs(randX) < 0.5f || Mathf.Abs(randY) < 0.5f); // 中心に近すぎる場合はやり直し

        Vector3 targetLocalPos = new Vector3(randX, randY, 2); // カメラ前方のローカル位置

        float flyDuration = 0.0f;

        int num=Check(m_Speed);     //速度をチェック
        switch (num)
        {
            case 1:
                flyDuration = 0.3f;
                break;
            case 2:
                flyDuration = 0.2f;
                break;
            case 3:
                flyDuration = 0.1f;
                break;
            case 4:
                flyDuration = 0.05f;
                break;
            default:
                break;
        }

        float t = 0.0f;


        //画面まで飛ばす処理
        while (t < 1.0f)
        {
            t += Time.deltaTime / flyDuration;
            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }
       mainCamera.ShakeCamera(0.1f, 0.3f);
        


        //画面に張り付いている物が画面外に行く処理

        Vector3 currentLocalPos = transform.localPosition;
        Vector3 flyStart = currentLocalPos;
        Vector3 flyEnd = Vector3.zero;

        //プレイヤーの速度と比較しどの方向にはけるか決める
        for (int i = 0; i < m_Type.Length; i++)
        {
            if (PlayerSpeed <= m_Type[i])
            {
                num = i + 1;
                break;
            }
        }

        switch (num)
        {
            case 1:
                // 真下に移動
                flyEnd = new Vector3(flyStart.x, -1.5f, 2);
                break;
            case 2:
                //画面の四隅
                Vector3[] corners =
                {
                    new Vector3(-1, -1, 1),
                    new Vector3(-1,  1, 1),
                    new Vector3( 1, -1, 1),
                    new Vector3( 1,  1, 1)
                };

                // 現在のローカル座標との距離が最も短い角を探す

                Vector3 nearestCorner = corners[0];
                float minDist = Vector3.Distance(currentLocalPos, corners[0]);

                for (int i = 1; i < corners.Length; i++)
                {
                    float dist = Vector3.Distance(currentLocalPos, corners[i]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestCorner = corners[i];
                    }
                }
                flyEnd = nearestCorner + (nearestCorner.normalized * 0.5f);

                harituki = 0.1f;
                break;
            case 3:
                flyEnd = new Vector3(flyStart.x, 1.5f, 2);
                harituki = 0.05f;
                break;
            default:
                break;
        }
        if (PlayerSpeed == 4 || PlayerSpeed == 5) harituki = 0.3f;  //怠惰でこの処理に

        // 張り付かせる
        yield return new WaitForSeconds(harituki);
        

        // 風に吹かれるように少しカーブ＋イージングで飛ばす

        float flyT = 0;

        while (flyT < 1.0f)
        {
            flyT += Time.deltaTime / flyDuration;
            float easedT = Mathf.SmoothStep(0, 1, flyT); // イージング
            transform.localPosition = Vector3.Lerp(flyStart, flyEnd, easedT);
            yield return null;
        }

        Destroy(gameObject);
    }

    private int Check(int[] _Check) 
    {
        for (int i = 0; i < _Check.Length; i++)
        {
            if (PlayerSpeed <= _Check[i])
            {
                return i + 1;
                 
            }
        }
        return 0;
    }
}
