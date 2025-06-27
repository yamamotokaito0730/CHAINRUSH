/*=====
<TextureScroll.cs>
└作成者：mori

＞内容
テクスチャのスクロール処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___27:プログラム作成:mori

=====*/

using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public enum ScrollAxis { X, Y, Z }  // 選択可能な軸

    [Header("スクロール設定")]
    public ScrollAxis scrollDirection;  // スクロールの向き
    public float scrollSpeed;           // スクロール速度

    private Renderer rend;
    private Vector2 offset = Vector2.zero;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // 時間によるオフセット変化
        float delta = scrollSpeed * Time.deltaTime;

        switch (scrollDirection)
        {
            case ScrollAxis.X:
                offset.x += delta;
                break;
            case ScrollAxis.Y:
                offset.y += delta;
                break;
            case ScrollAxis.Z:
                // ZはYと同じに扱う（UVにはZがないのでYで代用）
                offset.y += delta;
                break;
        }

        // オフセットをマテリアルに適用
        rend.material.mainTextureOffset = offset;
    }
}
