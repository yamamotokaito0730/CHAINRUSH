/*=====
<LoadScene.cs>
└作成者：mori

＞内容
ロード画面でのゲームシーンの非同期読み込み

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___22:プログラム作成:mori

=====*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneNameToLoad;  // 読み込むゲームシーン名
    private float timer = 0f;
    private bool hasLoaded = false;

    void Start()
    {
        timer = 0f;
        hasLoaded = false;

        if(Title.ToTitle)
        {
            GameManager.currentStageIndex = 0;
            sceneNameToLoad = "Stage1";
        }
        else
        {
            if (GameManager.isGameOver)
            {
                sceneNameToLoad = "Stage" + (GameManager.currentStageIndex);
            }
            else
            {
                GameManager.currentStageIndex++;
                sceneNameToLoad = "Stage" + (GameManager.currentStageIndex);
            }
        }
    }

    void Update()
    {
        if (!hasLoaded)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
            {
                GameManager.Instance.LoadStage(GameManager.currentStageIndex);
                hasLoaded = true;
            }
        }
    }
}
