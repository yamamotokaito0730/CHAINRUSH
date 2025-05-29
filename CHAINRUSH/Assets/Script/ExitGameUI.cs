/*=====
<ExitGameUI.cs>
└作成者：mori

＞内容
ゲーム強制終了の処理

＞注意事項


＞更新履歴
Y25         
_M05
__D  
___29:プログラム作成:mori

=====*/

using UnityEngine;
using UnityEngine.UI;

public class ExitGameUI : MonoBehaviour
{
    public GameObject exitPanel;
    public Button yesButton;
    public Button noButton;

    void Start()
    {
        exitPanel.SetActive(false); // 最初は非表示

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        // ゲーム開始時にカーソルを非表示＆ロック
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitPanel.SetActive(true);
            // カーソルを表示＆ロック解除
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnYesClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnNoClicked()
    {
        exitPanel.SetActive(false);
        // カーソルを再び非表示＆ロック
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
