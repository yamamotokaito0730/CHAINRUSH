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
    public GameManager gamemanager;

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
        // Imageコンポーネントまたはプレイヤーが存在しなければ処理を中断
        if (iconImage == null || player == null)
            return;

        // このアイコンがプレイヤー用アイコンであれば、回転だけ処理する
        if (iconImage.name == "PlayerIcon")
        {
            // プレイヤーのY軸回転を取得し、それに合わせてアイコンを逆方向に回転
            float playerYRotation = player.eulerAngles.y;
            transform.localEulerAngles = new Vector3(0, 0, -playerYRotation);
            return; // 他の処理は不要なので終了
        }

        // 対象（敵）がnullなら、アイコンを非表示にしてオブジェクトごと破棄
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Debug.Log(target);
            Debug.Log("消すyo~");
            iconImage.enabled = false;
            Destroy(iconImage.gameObject);
            return;
        }

            // プレイヤーと敵の距離を2D（XZ平面）で計算
            Vector3 offset = target.position - player.position;
        Vector2 offset2D = new Vector2(offset.x, offset.z);
        float distance = offset2D.magnitude;

        // 指定された表示範囲内にいる場合のみ、アイコンを表示
        if (distance <= displayRange)
        {
            // ミニマップ上での位置を計算（スケールを掛ける）
            Vector2 minimapPos = offset2D * mapScale;

            // ミニマップが円形なので、表示位置が円の中にあるかチェック
            float radius = minimapPanel.rect.width / 2f; // パネルの半分が半径（正方形前提）

            if (minimapPos.magnitude <= radius)
            {
                // 円内ならアイコンを表示し、位置を設定
                iconImage.enabled = true;
                ((RectTransform)transform).anchoredPosition = minimapPos;
            }
            else
            {
                // 円外なら非表示
                iconImage.enabled = false;
            }
        }
        else
        {
            // 表示範囲外なら非表示
            iconImage.enabled = false;
        }
    }
    //void Update()
    //{
    //    // プレイヤーアイコンの回転処理
    //    if (iconImage.name == "PlayerIcon")
    //    {
    //        if (player != null && !player.Equals(null))
    //        {
    //            float playerYRotation = player.eulerAngles.y;
    //            transform.localEulerAngles = new Vector3(0, 0, -playerYRotation);
    //        }
    //        return;
    //    }

    //    // 敵アイコンの存在チェック
    //    if (target == null || /*target.Equals(null) ||*/ player == null /*|| player.Equals(null)*/)
    //    {
    //        if (iconImage == null)
    //            return;
    //        iconImage.enabled = false;
    //        gamemanager.DestroyEnemyIcon(iconImage);
    //        Destroy(iconImage);
    //        return;
    //    }

    //    // プレイヤーと敵のオフセットを計算
    //    Vector3 offset = target.position - player.position;
    //    Vector2 offset2D = new Vector2(offset.x, offset.z);
    //    float distance = offset2D.magnitude;

    //    // 表示距離内なら
    //    if (distance <= displayRange)
    //    {
    //        Vector2 minimapPos = offset2D * mapScale;

    //        // ミニマップ円内に収まっているかチェック
    //        float radius = minimapPanel.rect.width / 2f; // 円の半径（正方形前提）
    //        if (minimapPos.magnitude <= radius)
    //        {
    //            iconImage.enabled = true;
    //            ((RectTransform)transform).anchoredPosition = minimapPos;
    //        }
    //        else
    //        {
    //            iconImage.enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        iconImage.enabled = false;
    //    }
    //}
}
