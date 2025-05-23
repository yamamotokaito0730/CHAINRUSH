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

=====*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // ロードシーンへ遷移
            SceneManager.LoadScene("LoadScene");
        }
    }
}
