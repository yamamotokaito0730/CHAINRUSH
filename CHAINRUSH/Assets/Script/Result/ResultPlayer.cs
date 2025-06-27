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

=====*/

using UnityEngine;

public class ResultPlayer : MonoBehaviour
{
    private float posY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Y座標が下がってくるので固定にする
        Vector3 pos = transform.position;
        pos.y = posY;
        transform.position = pos;
    }
}
