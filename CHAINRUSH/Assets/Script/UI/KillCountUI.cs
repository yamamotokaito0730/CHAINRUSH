/*=====
<KillCountDisplay.cs>
└作成者：mori

＞内容
敵を倒した数を表示する

＞更新履歴
Y25
_M06
__D  
___21:プログラム作成:mori
=====*/

using TMPro;
using UnityEngine;

public class KillCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI killCountText; // 表示するテキスト
    public GameManager gameManager;       // GameManagerへの参照

    void Update()
    {
        if (gameManager == null || killCountText == null) return;

        int killCount = gameManager.GetTotalKilled();

        killCountText.text = string.Format("{0}/40", killCount);
    }
}
