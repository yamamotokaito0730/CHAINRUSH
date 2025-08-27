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
___16:カメラ移動をマウス操作に変更、Rキーで初期位置にリセット:mori
_M06
__D
___15:カメラがTerrainやGameObjectの裏面を映さないように修正:matsushima
_M08
__D
___16:pad操作に対応:tooyama

=====*/
using UnityEngine;
using UnityEngine.InputSystem;
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

    // カメラの初期相対位置記録用
    private float m_InitialYaw;
    private float m_InitialPitch;

    // シェイク用変数
    private Vector3 m_ShakeOffset = Vector3.zero;
    private float m_ShakeDuration = 0f;
    private float m_ShakeMagnitude = 0.1f;

    private Gamepad gamepad; // ゲームパッドを使えるように宣言

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

        m_Yaw = m_Target.eulerAngles.y; // 現在のY軸角度を取得
        m_Pitch = m_Target.eulerAngles.x; // 現在のX軸角度を取得

        // 初期の相対オフセットと角度を保存
        m_InitialYaw = m_Yaw;
        m_InitialPitch = m_Pitch;
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
        //ゲーム開始前は停止
        if (!GameManager.IsGameActive) return;

        gamepad = Gamepad.current;  // 毎フレーム更新

        float mouseX = 0f;
        float mouseY = 0f;

        // コントローラーが接続されている場合
        if (gamepad != null)
        {
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            mouseX = rightStick.x * m_RotationSpeed * Time.deltaTime;
            mouseY = rightStick.y * m_RotationSpeed * Time.deltaTime;
        }
        // コントローラー未接続時のみキーボード・マウス入力を有効化
        else
        {
            // マウス入力によるカメラの回転
            mouseX = Input.GetAxis("Mouse X") * m_RotationSpeed * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * m_RotationSpeed * Time.deltaTime;
        }

        // マウス入力にスパイク制限をかける
        mouseX = Mathf.Clamp(mouseX, -5.0f, 5.0f);
        mouseY = Mathf.Clamp(mouseY, -5.0f, 5.0f);

        m_Yaw += mouseX;
        m_Pitch -= mouseY;

        m_Pitch = Mathf.Clamp(m_Pitch, -50.0f, 20.0f);  //回転の制限


        // Rキーで初期のオフセットと角度にリセット（プレイヤーの向きに対応）
        if (Input.GetKeyDown(KeyCode.R) || (gamepad != null && gamepad.rightStickButton.wasPressedThisFrame))
        {
            ResetCamera();
        }

        // カメラの理想位置をターゲットの位置＋オフセットに設定
        Vector3 targetPosition = m_Target.position + Quaternion.Euler(m_Pitch, m_Yaw, 0.0f) * m_Offset;
        transform.position = targetPosition;

        // ターゲットを常に見る
        //transform.LookAt(m_Target.position);

        // Terrainなどの遮蔽物との衝突をチェック
        RaycastHit hit;
        Vector3 direction = targetPosition - m_Target.position;    // プレイヤーから理想位置への方向ベクトル
        float distance = direction.magnitude;                       // 方向ベクトルの長さ
        //Vector3 finalPosition = targetPosition;                    // 最終的な位置
        Vector3 finalPosition = transform.position;                    // 最終的な位置
        if (Physics.Raycast(m_Target.position, direction.normalized, out hit, distance))
        {
            // 遮蔽物があれば、ヒットポイントの少し手前にカメラを配置
            finalPosition = hit.point - direction.normalized * 0.2f;
        }

        // 水面下にカメラがいかないように補正
        finalPosition.y = Mathf.Max(finalPosition.y, 3.0f);

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

        transform.position = finalPosition + m_ShakeOffset;
        transform.LookAt(m_Target.position);
    }

    /*＞ShakeCamera関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:カメラを振動させる関数
    */
    /// <param name="duration">揺れる時間（秒）</param>
    /// <param name="magnitude">揺れる強さ</param>
    public void ShakeCamera(float duration, float magnitude)
    {
        m_ShakeDuration = duration;
        m_ShakeMagnitude = magnitude;
    }

    public void ResetCamera()
    {
        // カメラのYawもプレイヤーの向きに合わせてリセット
        m_Yaw = m_Target.eulerAngles.y + m_InitialYaw;
        m_Pitch = m_InitialPitch;
    }
}