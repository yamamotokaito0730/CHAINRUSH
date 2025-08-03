/*=====
<TitleStartEffect.cs>
└作成者：mori

＞内容
タイトルシーンのSTART文字のエフェクト処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___13:プログラム作成:mori
_M08
__D
___03:パッドに対応:tooyama

=====*/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleStartEffect : MonoBehaviour
{
    public Image startUI;
    public float duration = 0.5f; // 拡大＆透明化の時間
    private bool hasStarted = false;// 一度だけ反応するようにするフラグ
    private Gamepad gamepad; // ゲームパッドを使えるように宣言


    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新
        if (!hasStarted && (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))) // Enterキー
        {
            hasStarted = true;
            StartCoroutine(PlayStartEffect());
        }
    }

    // テキストを拡大・透過させる処理（コルーチン）
    System.Collections.IEnumerator PlayStartEffect()
    {
        // 元のスケールを記録
        Vector3 originalScale = startUI.rectTransform.localScale;
        // 拡大後のスケール（2.0倍）
        Vector3 targetScale = originalScale * 2.0f;

        // 元の文字色を記録（透明度も含む）
        Color originalColor = startUI.color;
        // 透明度だけ下げた目標色（α = 0.0）
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f); // α下げる

        float elapsed = 0f;

        // durationの間、拡大と透明化を補間していく
        while (elapsed < duration)
        {
            // 0～1の補間係数
            float t = elapsed / duration;
            // スケールと色を徐々に変化させる
            startUI.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            startUI.color = Color.Lerp(originalColor, targetColor, t);
            // 毎フレーム経過時間を加算
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最終状態にする
        startUI.rectTransform.localScale = targetScale;
        startUI.color = targetColor;

        // シーン遷移など（任意）
        yield return new WaitForSeconds(0.5f);
        // SceneManager.LoadScene("NextSceneName");
    }
}
