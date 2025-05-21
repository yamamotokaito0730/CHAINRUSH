/*=====
<SpriteSheetAnim.cs>
└作成者：mori

＞内容
スプライトシートのアニメーション実装

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___21:プログラム作成:mori

=====*/

using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnim : MonoBehaviour
{
    public string spriteSheetPath = "flog"; // 読み込むスプライトシート名
    public Image targetImage;               // UIのImageにアニメーション表示
    public float frameRate = 0.1f;          // アニメーション速度

    private Sprite[] sprites;
    private int[] frameIndices = { 16, 17, 18, 19 }; // 17～20番目（配列は0始まり）
    private int currentFrame = 0; // 現在のフレームレート
    private float timer;

    void Start()
    {
        // スプライトをすべて読み込み
        sprites = Resources.LoadAll<Sprite>(spriteSheetPath);
    }

    void Update()
    {
        // スプライト配列がnull、または空の場合は何もしない
        if (sprites == null || sprites.Length == 0) return;

        // 経過時間を加算
        timer += Time.deltaTime;

        // 一定時間（frameRate）ごとに処理を行う
        if (timer >= frameRate)
        {
            // タイマーリセット
            timer = 0f;
            // 現在のフレーム番号を更新（ループするようにする）
            currentFrame = (currentFrame + 1) % frameIndices.Length;
            // 対象のImageコンポーネントに、対応するスプライトを設定
            targetImage.sprite = sprites[frameIndices[currentFrame]];
        }
    }
}
