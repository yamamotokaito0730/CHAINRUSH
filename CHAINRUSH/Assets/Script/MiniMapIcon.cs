/*=====
<MiniMapIcon.cs>
└作成者：mori

＞内容
ミニマップの実装

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___23:プログラム作成:mori

=====*/

using UnityEngine;
using UnityEngine.UI;

public class MiniMapIcon : MonoBehaviour
{
    public RectTransform minimapPanel;  // ミニマップのUIパネル
    public Transform target;            // 対象（敵）
    public Transform player;            // プレイヤーのTransform
    private float mapScale = 2.0f;         // ミニマップ上のスケール
    public float displayRange = 60f;    // 表示する最大距離（ワールド座標上）

    private Image iconImage;

    void Start()
    {
        iconImage = GetComponent<Image>();
    }

    void Update()
    {
        // Destroyされたオブジェクトにも対応したnullチェック
        if (target == null || target.Equals(null) || player == null || player.Equals(null))
        {
            iconImage.enabled = false; // 念のため非表示に
            return;
        }

        Vector3 offset = target.position - player.position;

        Vector2 offset2D = new Vector2(offset.x, offset.z);
        float distance = offset2D.magnitude;

        if (distance <= displayRange)
        {
            iconImage.enabled = true;
            Vector2 minimapPos = offset2D * mapScale;
            ((RectTransform)transform).anchoredPosition = minimapPos;
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}
