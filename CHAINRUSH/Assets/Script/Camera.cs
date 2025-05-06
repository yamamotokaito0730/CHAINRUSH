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

=====*/
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour
{
    //変数宣言
    [Header("ターゲット")]
    [SerializeField] private Transform m_Target;

    [Header("カメラのオフセット")]
    [SerializeField] private Vector3 m_Offset = new Vector3(0f, 5f, -7f);

    [Header("カメラの回転速度")]
    [SerializeField] private float m_RotationSpeed = 100f;

    private float m_Yaw = 0f; // 水平方向の回転量

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
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

        m_Yaw += horizontalInput * m_RotationSpeed * Time.deltaTime;

        // カメラ位置をターゲットの位置＋オフセットに設定
        Vector3 targetPosition = m_Target.position + Quaternion.Euler(0f, m_Yaw, 0f) * m_Offset;
        transform.position = targetPosition;

        // ターゲットを常に見る
        transform.LookAt(m_Target.position);
    }
}
