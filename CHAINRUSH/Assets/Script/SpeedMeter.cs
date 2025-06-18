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
___18:背景と、ゲージ切り替え処理追加:mori

=====*/

using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

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

    [Header("背景")]
    public Image[] backgroundImages;

    [Header("ゲージ")]
    public RawImage[] gaugeImages;

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

    // 段階2
    private readonly (float speed, float uv)[] Stage2 = new (float, float)[]
    {
        (5f,  0.2f),
        (8f,  0.4f),
        (11f, 0.6f),
        (14f, 0.8f),
        (17f, 1.0f)
    };

    // 段階3
    private readonly (float speed, float uv)[] Stage3 = new (float, float)[]
    {
        (20f, 0.16f),
        (23f, 0.33f),
        (26f, 0.5f),
        (29f, 0.66f),
        (32f, 0.83f),
        (35f, 1.0f)
    };

    // 段階4
    private readonly (float speed, float uv)[] Stage4 = new (float, float)[]
    {
        (38f, 0.33f),
        (41f, 0.66f),
        (44f, 1.0f)
    };

    // 段階5
    private readonly (float speed, float uv)[] Stage5 = new (float, float)[]
    {
        (47f, 0.5f),
        (50f, 1.0f)
    };

    private void Start()
    {
        // プレイヤーオブジェクトのplayerスクリプトを取得
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();

        // 背景初期化
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            backgroundImages[i].gameObject.SetActive(false);
        }

        // ゲージ初期化
        for (int i = 0; i < gaugeImages.Length; i++)
        {
            gaugeImages[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        // プレイヤーの現在のスピードを取得
        float speed = player.GetSpeed();
        // プレイヤーの段階（1〜5）を0始まりにして stage とする
        int stage = Mathf.Clamp(player.GetState() - 1, 0, speedStages.Length - 1);

        // 速度に応じて、メーターの表示量を変化させる
        float fill = 0;
        switch (stage - 1)
        {
            case 0:
                fill = GetFillFromTable(Stage2);
                //gaugeImages[stage - 1].fillAmount = fill;
                gaugeImages[stage - 1].uvRect = new Rect(0.0f, 0.0f, fill, 1.0f);
                break;
            case 1:
                fill = GetFillFromTable(Stage3);
                //gaugeImages[stage - 1].fillAmount = fill;
                gaugeImages[stage - 1].uvRect = new Rect(0.0f, 0.0f, fill, 1.0f);
                break;
            case 2:
                fill = GetFillFromTable(Stage4);
                //gaugeImages[stage - 1].fillAmount = fill;
                gaugeImages[stage - 1].uvRect = new Rect(0.0f, 0.0f, fill, 1.0f);
                break;
            case 3:
                fill = GetFillFromTable(Stage5);
                //gaugeImages[stage - 1].fillAmount = fill;
                gaugeImages[stage - 1].uvRect = new Rect(0.0f, 0.0f, fill, 1.0f);
                break;
        }

        // 段階が変わったときに針を左端に戻す処理（必要であれば有効化）
        if (stage != currentStage)
        {
            currentStage = stage;
            baseAngle = minAngle; // 針を一度左端にリセット

            // 背景Image切り替え
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                backgroundImages[i].gameObject.SetActive(i == currentStage);
            }

            // ゲージImage切り替え
            for (int i = 0; i < gaugeImages.Length; i++)
            {
                gaugeImages[i].gameObject.SetActive(i == currentStage - 1);
            }
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

    /*＞GetFillFromTable関数
    引数：参照先テーブル
    ｘ
    戻値：表示するuv量
    ｘ
    概要:各段階ごとに応じたスピードによるメーターの表示幅を変化させる
    */
    float GetFillFromTable((float speed, float uv)[] table)
    {
        float fill = 0f; // 戻り値格納用

        // テーブルの中身を順番に見て、スピードが下回ったらその時点のuvを格納
        for (int i = 0; i < table.Length; i++)
        {
            if (table[i].speed >= player.GetSpeed())
            {
                fill = table[i].uv;
                break;
            }
        }
        return fill;
    }
}
