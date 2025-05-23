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
    [SerializeField] private string sceneNameToLoad = "NatureRenderer";  // 読み込むゲームシーン名
    [SerializeField] private float minLoadingTime = 1.0f;  // 最低表示時間（秒）

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        float timer = 0f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            // 進捗が90％以上で、かつ最低時間を経過したら切り替え可能にする
            if (asyncLoad.progress >= 0.9f && timer >= minLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
