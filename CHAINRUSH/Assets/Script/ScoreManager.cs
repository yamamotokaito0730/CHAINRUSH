/*=====
<ScoreManager.cs>
└作成者：yamamoto

＞内容
スコアを計算するスクリプト

＞注意事項
敵を実装していないので存在はしてるだけ
ScoreUIを実装していないので実装したら更新が必要


＞更新履歴
Y25   
_M04    
__D     
___12:プログラム作成:yamamoto

=====*/
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    static private float m_Score;
    [SerializeField] private TextMeshProUGUI scoreText;

    /*＞Start関数
    引数：なし
    ｘ
    戻値：なし
    ｘ
    概要:初期化
    */
    void Start()
    {
        // 初期化
        m_Score = 0;
        UpdateScoreUI();
    }

    /*＞スコア加算関数
    引数1：int _score:加算する値
    引数1：float _rate:加算倍率
    ｘ
    戻値：なし
    ｘ
    概要:スコア加算を行う
    */
    public void AddScore(int _score,float _rate)
    {
        m_Score += _score * _rate;
        // ScoreUIを呼び出し更新させる
        UpdateScoreUI();

    }

    /*＞スコア減算関数
   引数：int _score:減算する値
   ｘ
   戻値：なし
   ｘ
   概要:スコア減算を行う
   */
    public void SubScore(int _score)
    {
        m_Score -= _score;
        // ScoreUIを呼び出し更新させる
        UpdateScoreUI();
    }

    /*＞スコアUI更新関数
   引数：なし
   ｘ
   戻値：なし
   ｘ
   概要:UIを更新する
   */
    private void UpdateScoreUI()
    {
        scoreText.text = "score:"+ m_Score;
    }
}
