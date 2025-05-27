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
___27:プレイヤーに合わせてアイコンが回転する処理を追加:mori
___27:ミニマップを円形に変更

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
        // プレイヤーアイコンの回転処理
        if (iconImage.name == "PlayerIcon")
        {
            if (player != null && !player.Equals(null))
            {
                float playerYRotation = player.eulerAngles.y;
                transform.localEulerAngles = new Vector3(0, 0, -playerYRotation);
            }
            return;
        }

        // 敵アイコンの存在チェック
        if (target == null || target.Equals(null) || player == null || player.Equals(null))
        {
            iconImage.enabled = false;
            return;
        }

        // プレイヤーと敵のオフセットを計算
        Vector3 offset = target.position - player.position;
        Vector2 offset2D = new Vector2(offset.x, offset.z);
        float distance = offset2D.magnitude;

        // 表示距離内なら
        if (distance <= displayRange)
        {
            Vector2 minimapPos = offset2D * mapScale;

            // ミニマップ円内に収まっているかチェック
            float radius = minimapPanel.rect.width / 2f; // 円の半径（正方形前提）
            if (minimapPos.magnitude <= radius)
            {
                iconImage.enabled = true;
                ((RectTransform)transform).anchoredPosition = minimapPos;
            }
            else
            {
                iconImage.enabled = false;
            }
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}
