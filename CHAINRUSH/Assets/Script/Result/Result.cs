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
___20:UI表示の挙動を変更:mori
___21:ランク画像表示対応:mori
___21:ランクグループ配列化対応:mori
=====*/

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public CanvasGroup resultGroup;      // RESULT のグループ
    public CanvasGroup timeGroup;        // TIME のグループ
    public CanvasGroup[] rankGroups;     // ランク (S,A,B,C,D) の CanvasGroup（5つ）

    public TextMeshProUGUI timeText;     // タイム表示用テキスト

    float timer = 0f;                    // 表示時間管理

    // スライドイン用
    Vector3 resultStartPos;
    Vector3 timeStartPos;
    Vector3 resultTargetPos;
    Vector3 timeTargetPos;

    // ランク表示
    bool rankAnimationPlaying = false;
    float rankAnimTime = 0f;
    Vector3 rankTargetScale;             // 対象ランクの目標スケール

    bool allUiShown = false;             // UI全部終わったか
    int rankIndex = 0;                   // どのランクか (0=S, 4=D)
    public int rank;

    void Start()
    {
        // === RESULT, TIME ===

        resultTargetPos = resultGroup.transform.localPosition;
        timeTargetPos = timeGroup.transform.localPosition;

        resultStartPos = resultTargetPos + new Vector3(-1000f, 0f, 0f);
        timeStartPos = timeTargetPos + new Vector3(-1000f, 0f, 0f);

        resultGroup.alpha = 0f;
        timeGroup.alpha = 0f;

        resultGroup.transform.localPosition = resultStartPos;
        timeGroup.transform.localPosition = timeStartPos;

        // === ランクグループ初期化 ===

        for (int i = 0; i < rankGroups.Length; i++)
        {
            rankGroups[i].alpha = 0f;
        }

        // === ランク判定 ===

        float elapsed = Timer.elapsedTime;

        if (GameManager.isGameOver)
        {
            rankIndex = 4; // D
        }
        else if (elapsed <= 300f)
        {
            rankIndex = 0; // S
        }
        else if (elapsed <= 480f)
        {
            rankIndex = 1; // A
        }
        else if (elapsed <= 600f)
        {
            rankIndex = 2; // B
        }
        else
        {
            rankIndex = 3; // C
        }

        // === 対象ランクだけ表示 ===
        rankIndex = rank;
        rankGroups[rankIndex].alpha = 0.0f;

        // 開始スケール（大きく）
        rankTargetScale = rankGroups[rankIndex].transform.localScale; ;
        rankGroups[rankIndex].transform.localScale = rankTargetScale + new Vector3(3f, 3f, 3f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // === RESULT 表示 ===
        if (timer > 0f && timer < 0.3f)
        {
            float t = (timer - 0f) / 0.3f;
            resultGroup.alpha = t;
            resultGroup.transform.localPosition = Vector3.Lerp(resultStartPos, resultTargetPos, t);
        }

        // === ランク表示開始 ===
        else if (timer > 0.7f && timer < 1.0f && !rankAnimationPlaying)
        {
            rankAnimationPlaying = true;
            rankAnimTime = 0f;

            // 開始時 alpha は 0 に
            rankGroups[rankIndex].alpha = 0f;
        }

        // === ランクアニメ ===
        if (rankAnimationPlaying)
        {
            rankAnimTime += Time.deltaTime;
            float t = Mathf.Clamp01(rankAnimTime / 0.3f);

            Vector3 startScale = rankTargetScale + new Vector3(3f, 3f, 3f);
            Vector3 scale = Vector3.Lerp(startScale, rankTargetScale, t);

            rankGroups[rankIndex].transform.localScale = scale;

            // Alpha も 0 → 1 にフェード
            rankGroups[rankIndex].alpha = t;

            if (t >= 1f)
            {
                rankAnimationPlaying = false;
                rankGroups[rankIndex].transform.localScale = rankTargetScale;
                rankGroups[rankIndex].alpha = 1f;
            }
        }

        // === TIME 表示 ===
        if (timer > 1.4f && timer < 1.7f)
        {
            float t = (timer - 1.4f) / 0.3f;
            timeGroup.alpha = t;
            timeGroup.transform.localPosition = Vector3.Lerp(timeStartPos, timeTargetPos, t);

            int minutes = Mathf.FloorToInt(Timer.elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(Timer.elapsedTime % 60f);
            float fraction = Timer.elapsedTime % 1f;
            int decimalPart = Mathf.FloorToInt(fraction * 10f);

            timeText.text = string.Format("{0:00}:{1:00}.{2}", minutes, seconds, decimalPart);
        }

        // === 全部完了フラグ ===
        if (timer >= 1.7f)
        {
            allUiShown = true;
        }

        // === Enterキーでタイトルへ ===
        if (allUiShown && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
