/*=====
<SpeedMeter.cs>
└作成者：mori

＞内容
スピードメーターの実装

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___29:プログラム作成:mori
_M06
___06:プログラム大幅変更:mori

=====*/

using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    [Header("針のTransform")]
    public RectTransform needle;

    [Header("角度の設定")]
    public float minAngle = -90f;
    public float maxAngle = 90f;

    [Header("カタカタ揺れの設定")]
    public float shakeAmount = 1.0f;
    public float shakeSpeed = 20.0f;

    private float baseAngle = 0f;
    private Player player;
    private int currentStage = -1; // 現在の段階（初期値：未設定）

    // 各段階のスピード範囲（最小値と最大値）
    private readonly (float min, float max)[] speedStages = new (float, float)[]
    {
        (0f, 4f),    // 段階1
        (5f, 17f),   // 段階2
        (20f, 35f),  // 段階3
        (38f, 44f),  // 段階4
        (47f, 50f)   // 段階5
    };

    private void Start()
    {
        // プレイヤーオブジェクトのplayerスクリプトを取得
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
    }

    void Update()
    {
        if (player == null) return;

        // プレイヤーの現在のスピードを取得
        float speed = player.GetSpeed();
        // プレイヤーの段階（1〜5）を0始まりにして stage とする
        int stage = Mathf.Clamp(player.GetState() - 1, 0, speedStages.Length - 1);

        // 段階が変わったときに針を左端に戻す処理（必要であれば有効化）
        if (stage != currentStage)
        {
            currentStage = stage;
            baseAngle = minAngle; // 針を一度左端にリセット
        }

        // 現在の段階のスピード範囲を取得
        var (minSpeed, maxSpeed) = speedStages[stage];
        // 現在速度が段階内でどれくらいか（0〜1の範囲で正規化）
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        // 針の基本角度を段階内の相対スピードに応じて算出
        baseAngle = Mathf.Lerp(minAngle, maxAngle, t);

        // カタカタ揺れ
        float shake = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        // 針を回転（Y軸反転）
        needle.localRotation = Quaternion.Euler(0f, 0f, -(baseAngle + shake));
    }
}
