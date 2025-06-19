/*=====
<Result.cs>
└作成者：mori

＞内容
リザルトの実装

＞注意事項

＞更新履歴
Y25         
_M06
__D  
___18:プログラム作成:mori
=====*/

using UnityEngine;

public class Result : MonoBehaviour
{
    public CanvasGroup resultGroup;
    public CanvasGroup rankGroup;
    public CanvasGroup timeGroup;

    float timer = 0f;

    void Start()
    {
        // 最初は全部透明＆小さく
        resultGroup.alpha = 0f;
        rankGroup.alpha = 0f;
        timeGroup.alpha = 0f;

        //resultGroup.transform.localScale = Vector3.one * 0.5f;
        //rankGroup.transform.localScale = Vector3.one * 0.5f;
        //timeGroup.transform.localScale = Vector3.one * 0.5f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // RESULT 表示（0.0〜0.5秒）
        if (timer > 0f && timer < 0.5f)
        {
            float t = (timer - 0f) / 0.5f;
            resultGroup.alpha = t;
            resultGroup.transform.localScale = new Vector3(
                Mathf.Lerp(0.5f, 2.0f, t),  // X方向を1.5倍まで伸ばす
                Mathf.Lerp(0.5f, 1f, t),    // Y方向は通常
                1f
            );
        }
        // ランク表示（1.0〜1.5秒）
        else if (timer > 1.0f && timer < 1.5f)
        {
            float t = (timer - 1.0f) / 0.5f;
            rankGroup.alpha = t;
            rankGroup.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, t);
        }
        // 時間表示（2.0〜2.5秒）
        else if (timer > 2.0f && timer < 2.5f)
        {
            float t = (timer - 2.0f) / 0.5f;
            timeGroup.alpha = t;
            timeGroup.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, t);
        }
    }
}
