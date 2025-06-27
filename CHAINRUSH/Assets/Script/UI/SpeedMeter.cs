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

    [Header("ゲージ２段階目")]
    public Image[] gaugeImages2;
    [Header("ゲージ３段階目")]
    public Image[] gaugeImages3;
    [Header("ゲージ４段階目")]
    public Image[] gaugeImages4;
    [Header("ゲージ５段階目")]
    public Image[] gaugeImages5;

    [Header("速度 10の位表示用 Image(0〜9)")]
    public Image[] tensImages;
    [Header("速度 1の位表示用 Image(0〜9)")]
    public Image[] onesImages;

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
    private readonly float[] Stage2 = new float[]
    {
        5f,
        8f,
        11f,
        14f,
        17f
    };

    // 段階3
    private readonly float[] Stage3 = new float[]
    {
        20f,
        23f,
        26f,
        29f,
        32f,
        35f
    };

    // 段階4
    private readonly float[] Stage4 = new float[]
    {
        38f,
        41f,
        44f
    };

    // 段階5
    private readonly float[] Stage5 = new float[]
    {
        47f,
        50f,
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
    }

    void Update()
    {
        if (player == null) return;

        // プレイヤーの現在のスピードを取得
        float speed = player.GetSpeed();
        // プレイヤーの段階（1〜5）を0始まりにして stage とする
        int stage = Mathf.Clamp(player.GetState() - 1, 0, speedStages.Length - 1);

        // 10の位と1の位を計算
        int tens = ((int)speed) / 10;
        int ones = ((int)speed) % 10;

        // 10の位表示更新
        for (int i = 0; i < tensImages.Length; ++i)
        {
            tensImages[i].gameObject.SetActive(i == tens);
        }

        // 1の位表示更新
        for (int i = 0; i < onesImages.Length; ++i)
        {
            onesImages[i].gameObject.SetActive(i == ones);
        }

        // 速度に応じて、メーターの表示量を変化させる
        int num = 0;
        switch (stage - 1)
        {
            case 0: // 段階２
                num = GetNumFromTable(Stage2);
                for (int i = 0; i < gaugeImages2.Length; ++i)
                {
                    gaugeImages2[i].gameObject.SetActive(i <= num);
                }
                break;
            case 1: // 段階３
                num = GetNumFromTable(Stage3);
                for (int i = 0; i < gaugeImages3.Length; ++i)
                {
                    gaugeImages3[i].gameObject.SetActive(i <= num);
                }
                break;
            case 2: // 段階４
                num = GetNumFromTable(Stage4);
                for (int i = 0; i < gaugeImages4.Length; ++i)
                {
                    gaugeImages4[i].gameObject.SetActive(i <= num);
                }
                break;
            case 3: // 段階５
                num = GetNumFromTable(Stage5);
                for (int i = 0; i < gaugeImages5.Length; ++i)
                {
                    gaugeImages5[i].gameObject.SetActive(i <= num);
                }
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

            // ゲージImageリセット
            for (int i = 0; i < gaugeImages2.Length; i++)
            {
                gaugeImages2[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gaugeImages3.Length; i++)
            {
                gaugeImages3[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gaugeImages4.Length; i++)
            {
                gaugeImages4[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gaugeImages5.Length; i++)
            {
                gaugeImages5[i].gameObject.SetActive(false);
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

    /*＞GetNumFromTable関数
    引数：参照先テーブル
    ｘ
    戻値：表示するメーター量
    ｘ
    概要:各段階ごとに応じたスピードによるメーターの表示幅を変化させる
    */
    int GetNumFromTable(float[] table)
    {
        int num = 0; // 戻り値格納用

        // テーブルの中身を順番に見て、スピードが下回ったらその時点の配列番号を格納
        for (int i = 0; i < table.Length; i++)
        {
            if (table[i] >= player.GetSpeed())
            {
                num = i;
                break;
            }
        }
        return num;
    }
}
