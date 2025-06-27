/*=====
<ObjectScroll.cs>
└作成者：mori

＞内容
リザルトシーンのオブジェクトスクロール処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___27:プログラム作成:mori

=====*/

using UnityEngine;

public class ObjectScroll : MonoBehaviour
{
    public float scrollSpeed;       // スクロール速度
    public float resetZ;           // 再配置する位置
    public float limitZ;            // UIのZ位置（ここに来る前にリセット）

    void Update()
    {
        // カメラ → UI に向かって移動（Zマイナス方向）
        transform.position += Vector3.back * scrollSpeed * Time.deltaTime;

        // UIより手前に来る前にリセット
        if (transform.position.z <= limitZ)
        {
            Vector3 pos = transform.position;
            pos.z = resetZ;
            transform.position = pos;
        }
    }
}
