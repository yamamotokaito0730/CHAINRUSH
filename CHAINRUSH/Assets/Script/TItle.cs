/*=====
<Title.cs>
└作成者：mori

＞内容
タイトルの実装

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___22:プログラム作成:mori
_M06
__D
___17:シーン切り替えに1.5秒遅延させるように変更

=====*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private bool isLoading = false;  // 2度押し防止用

    private void Awake()
    {
        BGMManager.Instance.PlayBGMWithFade("Title", 1f);
    }

    void Update()
    {
        if (!isLoading && Input.GetKeyDown(KeyCode.Return))
        {
            isLoading = true;
            StartCoroutine(LoadSceneWithDelay());
        }
    }

    // ２秒遅延用
    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1.5f);  // 1.5秒待つ
        BGMManager.Instance.ChangeBGM("Stage1", 1.5f);
        SceneManager.LoadScene("LoadScene");
    }
}
