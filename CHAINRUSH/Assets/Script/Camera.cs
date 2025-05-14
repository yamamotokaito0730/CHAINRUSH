/*=====
<Camera.cs>
└作成者：yamamoto

＞内容
Cameraの挙動を管理するスクリプト

＞注意事項
プレイヤーがダッシュ（加速）する仕様が追加されたとき変更必須


＞更新履歴
Y25   
_M04    
__D     
___11:プログラム作成:yamamoto   //日付:変更内容:施行者
___27:カメラ移動を←→キーで行うように変更:mori
_M05
__D
___14:カメラ移動↑↓キー操作追加:yamamoto

=====*/
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour
{
    //変数宣言
    [Header("ターゲット")]
    [SerializeField] private Transform m_Target;

    [Header("カメラのオフセット")]
    [SerializeField] private Vector3 m_Offset = new Vector3(0.0f, 5.0f, -7.0f);

    [Header("カメラの回転速度")]
    [SerializeField] private float m_RotationSpeed = 100.0f;

    private float m_Yaw = 0.0f; // 水平方向の回転量
    private float m_Pitch = 0.0f; // 垂直方向の回転量

    // シェイク用変数
    private Vector3 m_ShakeOffset = Vector3.zero;
    private float m_ShakeDuration = 0f;
    private float m_ShakeMagnitude = 0.1f;

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    private void Start()
    {
        // ターゲットが未設定ならプレイヤーを探して設定
        if (m_Target == null)
        {
            m_Target = GameObject.FindWithTag("Player").transform;
        }

        m_Yaw = transform.eulerAngles.y; // 現在のY軸角度を取得
        m_Pitch = transform.eulerAngles.x; // 現在のX軸角度を取得
    }

    /*＞LateUpdate関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:Update関数の後に更新される関数
    */
    void LateUpdate()
    {
        // ←→キー入力でカメラのY軸回転
        float horizontalInput = 0.0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1.0f;

        // ↑↓キー入力でX軸（上下）回転
        float verticalInput = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalInput = 1.0f;
        if (Input.GetKey(KeyCode.DownArrow)) verticalInput = -1.0f;

        // 回転角度の更新
        m_Yaw += horizontalInput * m_RotationSpeed * Time.deltaTime;
        m_Pitch += verticalInput * m_RotationSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, -20.0f, 20.0f);  //回転の制限

        // カメラ位置をターゲットの位置＋オフセットに設定
        Vector3 targetPosition = m_Target.position + Quaternion.Euler(m_Pitch, m_Yaw, 0.0f) * m_Offset;
        transform.position = targetPosition;

        // ターゲットを常に見る
        //transform.LookAt(m_Target.position);

        // シェイク処理
        if (m_ShakeDuration > 0f)
        {
            m_ShakeOffset = Random.insideUnitSphere * m_ShakeMagnitude;
            m_ShakeDuration -= Time.deltaTime;
        }
        else
        {
            m_ShakeOffset = Vector3.zero;
        }

        transform.position = targetPosition + m_ShakeOffset;
        transform.LookAt(m_Target.position);
    }

    /// <param name="duration">揺れる時間（秒）</param>
    /// <param name="magnitude">揺れる強さ</param>
    public void ShakeCamera(float duration, float magnitude)
    {
        m_ShakeDuration = duration;
        m_ShakeMagnitude = magnitude;
    }
}