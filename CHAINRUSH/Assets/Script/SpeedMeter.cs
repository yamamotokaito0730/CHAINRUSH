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

=====*/

using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    public GameObject[] bars; // Bar1〜Bar5を順に設定
    public int level = 2;

    private Player player;
    public float currentSpeed = 0.0f; // プレイヤーの現在速度
    private int[] thresholds = { 4, 17, 35, 44, 50 };

    void Start()
    {
        // "Player" タグの GameObject から Player スクリプトを取得
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Playerオブジェクトが見つかりません。タグが正しいか確認してください。");
        }
    }

    void Update()
    {
        if (player == null) return; // null チェック
        currentSpeed = player.m_fSpeed;
        UpdateSpeedMeter(currentSpeed);
    }

    void UpdateSpeedMeter(float speed)
    {
        level = 0;

        for (int i = 0; i < thresholds.Length; i++)
        {
            if (speed <= thresholds[i])
            {
                level = i + 1;
                break;
            }
        }

        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].SetActive(i < level);
        }
    }
}
