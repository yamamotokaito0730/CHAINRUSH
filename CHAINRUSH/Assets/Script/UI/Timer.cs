/*=====
<Timer.cs>
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
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public static float elapsedTime = 0f;

    void Start()
    {
        // タイマーリセット
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        float fraction = elapsedTime % 1f;

        // 小数点1桁まで表示（四捨五入）
        int decimalPart = Mathf.FloorToInt(fraction * 10f);

        timerText.text = string.Format("{0:00}:{1:00}.{2}", minutes, seconds, decimalPart);
    }
}
