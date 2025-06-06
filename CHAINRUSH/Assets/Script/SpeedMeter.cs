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

    [Header("速度の設定")]
    public float currentSpeed = 0f;
    public float maxSpeed = 50f;

    [Header("角度の設定")]
    public float minAngle = -90f; // 0km/h のとき
    public float maxAngle = 90f;  // MAX速度のとき

    [Header("カタカタ揺れの設定")]
    public float shakeAmount = 1.0f; // 揺れの強さ（角度）
    public float shakeSpeed = 20.0f; // 揺れの速さ

    private float baseAngle = 0f;
    private Player player;

    private void Start()
    {
        // "Player" タグの GameObject から Player スクリプトを取得
        player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
    }

    void Update()
    {
        SetSpeed(player.GetSpeed());
        // スピードを角度に変換
        float t = Mathf.Clamp01(currentSpeed / maxSpeed);
        baseAngle = Mathf.Lerp(minAngle, maxAngle, t);

        // ランダムなカタカタ揺れ（時間ベースでノイズを入れる）
        float shake = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        // 実際に針を回転させる
        needle.localRotation = Quaternion.Euler(0f, 0f, -(baseAngle + shake));
    }

    // 外部から速度を更新するメソッド（必要なら）
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
}
