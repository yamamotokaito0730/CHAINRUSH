/*=====
<ResultPlayer.cs>
└作成者：mori

＞内容
リザルトシーンのプレイヤー座標固定処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___27:プログラム作成:mori
_M07
__D
___04:アニメーション再生処理を追加

=====*/

using UnityEngine;

public class ResultPlayer : MonoBehaviour
{
    //private float posY;
    private Animator animator;

    private float moveDuration = 2f; // 0〜2秒で移動
    private float elapsed = 0f;
    private Vector3 startPos;
    private Vector3 endPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //posY = transform.position.y;

        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // "Result" というステート名のアニメーションを再生
            animator.Play("Result");
        }
        else
        {
            Debug.LogWarning("Animator が見つかりませんでした。");
        }

        // 初期位置と目標位置を設定（Z座標だけ変化）
        startPos = transform.position;
        startPos.z = 7f;

        endPos = transform.position;
        endPos.z = -7f;

        // 初期位置に設定
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        // Y座標が下がってくるので固定にする
        //Vector3 pos = transform.position;
        //pos.y = posY;
        //transform.position = pos;

        if (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            transform.position = new Vector3(transform.position.x, transform.position.y, newPos.z);
        }
    }
}
