/*=====
<DotText.cs>
└作成者：mori

＞内容
TextMeshProでの"・・・"表示の実装

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___21:プログラム作成:mori

=====*/

using UnityEngine;
using TMPro;

public class DotText : MonoBehaviour
{
    public TextMeshProUGUI dotText;

    // 表示するドットのパターン（何もない → 1つ → 2つ → 3つ → ループ）
    string[] dots = { "",".", "..", "..." };
    // 現在のドットインデックス
    int index = 0;
    float timer = 0f;
    // ドットを切り替える間隔（秒）
    public float interval = 0.5f;

    void Update()
    {
        // 経過時間を加算
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            // dots配列の現在のインデックスに応じた文字列を表示
            dotText.text = dots[index % dots.Length];
            index++;
            timer = 0f;
        }
    }
}
