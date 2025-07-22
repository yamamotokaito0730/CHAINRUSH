/*=====
<ResultSelect.cs>
└作成者：mori

＞内容
リザルトのシーン切り替え実装

＞注意事項

＞更新履歴
Y25         
_M07
__D  
___05:プログラム作成:mori
=====*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ResultSelect : MonoBehaviour
{
    [System.Serializable]
    public class MenuButton
    {
        public Button button;
        public TextMeshProUGUI label;
    }

    public MenuButton[] menuButtons; // 0: 次のステージ, 1: タイトル
    public MenuButton menuButton;
    private int selectedIndex = 0;
    private float blinkTime = 0f;
    private bool isMenuActive = false;
    private float timer = 0f;
    private bool isAllClear = false;

    void Start()
    {
        isMenuActive = false;
        // 最初は非表示
        foreach (var mb in menuButtons)
        {
            mb.button.gameObject.SetActive(false);
        }
        menuButton.button.gameObject.SetActive(false);

        // GameManagerの状態によってボタンのラベルを変更
        if (GameManager.isGameOver)
        {
            menuButtons[0].label.text = "Retry";
        }
        else
        {
            menuButtons[0].label.text = "NextStage";
        }

        // 特定の条件のとき、次のステージボタンを無効化
        if (GameManager.currentStageIndex == 1 && !GameManager.isGameOver)
        {
            isAllClear = true;
        }
    }

    void Update()
    {
        if (isAllClear)
        {
            // 6.0秒の待機
            if (!isMenuActive)
            {
                timer += Time.deltaTime;
                if (timer >= 6.0f)
                {
                    menuButton.button.gameObject.SetActive(true);
                    menuButton.label.color = Color.black;
                    isMenuActive = true;
                }
                return;
            }
            // 決定処理
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Title");
            }
        }
        else
        {
            // 6.0秒の待機
            if (!isMenuActive)
            {
                timer += Time.deltaTime;
                if (timer >= 6.0f)
                {
                    ActivateMenu();
                }
                return;
            }

            // 入力処理
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
                UpdateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = Mathf.Min(menuButtons.Length - 1, selectedIndex + 1);
                UpdateSelection();
            }

            // 選択中のボタンを点滅
            BlinkSelectedButton();

            // 決定処理
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (selectedIndex == 0)
                {
                    SceneManager.LoadScene("LoadScene");
                    Title.ToTitle = false;
                }
                else
                {
                    SceneManager.LoadScene("Title");
                }
            }
        }
    }

    void ActivateMenu()
    {
        isMenuActive = true;
        foreach (var mb in menuButtons)
        {
            mb.button.gameObject.SetActive(true);
        }
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var label = menuButtons[i].label;
            label.color = (i == selectedIndex) ? Color.red : Color.black;
            var canvasGroup = menuButtons[i].button.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }
        blinkTime = 0f;
    }

    void BlinkSelectedButton()
    {
        blinkTime += Time.deltaTime * 2f;
        float alpha = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(blinkTime, 1f));

        var canvasGroup = menuButtons[selectedIndex].button.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}

