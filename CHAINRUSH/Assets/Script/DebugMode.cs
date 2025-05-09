/*=====
<DebugMode.cs>
└作成者：tooyama

＞内容
デバッグ用スクリプト
座標
速度
倒した敵の数
坂の傾斜

この順に数値を表示する

＞注意事項



＞更新履歴
Y25   
_M04    
__D     
___24:プログラム作成:tooyama
___28:坂の傾斜を計測＆表示出来る機能を追加

=====*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    [Header("デバッグテキスト")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI posText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI groundSlopeText;


    //[Header("デバッグプレハブ")]
    //[SerializeField] private GameObject debugPrefab;

    [Header("レイキャスト")]
    [SerializeField, Tooltip("レイの長さ")] private float rayLength = 2.0f;
    [SerializeField, Tooltip("レイの色")] private Color normalColor = Color.red;

    /*＞デバッグ更新関数
    引数1：_PlayerTransform
    引数2：_Speed
    引数3：_KillCount 
    ｘ
    戻値：なし
    ｘ
    概要:引数から受け取った数値に各テキストを更新する
    */
    public void UpdateDebugUI(Transform _PlayerTransform, float _Speed, int _KillCount)
    {
        Vector3 _PlayerPos = _PlayerTransform.position;

        posText.text = "Pos" + _PlayerPos; // 座標
        speedText.text = "Speed" + _Speed; // 速度
        killCountText.text = "Count" + _KillCount; // 倒した敵の数


        RaycastHit hit;
        if (Physics.Raycast(_PlayerPos, Vector3.down, out hit, rayLength))
        {
            Debug.DrawRay(hit.point, hit.normal, normalColor);// 地面の法線ベクトル(赤い線)を可視化

            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);// 傾斜角度を計算

            groundSlopeText.text = $"Slope{slopeAngle:F1}°";// 計算した傾斜を丸めて表示

            Debug.DrawLine(_PlayerPos, hit.point, Color.yellow);// プレイヤーの位置と地面のヒット点を黄色い線でつなぐ
        }
        else
        {
            Debug.DrawRay(_PlayerPos, Vector3.down * rayLength, Color.gray);// 地面に当たっていない時はグレーで表示
        }

    }
}
