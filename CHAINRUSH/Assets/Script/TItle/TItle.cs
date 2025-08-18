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
___20:SE追加:nakashima
_M08
__D
___03:パッドに対応:tooyama
=====*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Title : MonoBehaviour
{
    private bool isLoading = false;  // 2度押し防止用
    static public bool ToTitle = false;

    private Gamepad gamepad; // ゲームパッドを使えるように宣言

    private void Awake()
    {
        BGMManager.Instance.PlayBGMWithFade("Title", 1f);
        gamepad = Gamepad.current; // 毎フレーム更新
    }

    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新
        if (!isLoading && (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame)))
        {
            //SE////////////////////////////////////////
            SEManager.Instance.Play("chainsaw");
            ////////////////////////////////////////////
            isLoading = true;
            //ObjectPoolManager.Instance.ReturnAllToPool();
            StartCoroutine(LoadSceneWithDelay());
        }
    }

    // ２秒遅延用
    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1.5f);  // 1.5秒待つ
        BGMManager.Instance.ChangeBGM("Stage1", 1.5f);
        SceneManager.LoadScene("LoadScene");
        ToTitle = true;
    }
}
