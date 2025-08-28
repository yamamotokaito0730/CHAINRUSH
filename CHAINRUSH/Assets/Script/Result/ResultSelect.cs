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
_M08
__D
___03:パッドに対応:tooyama
=====*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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
    private Gamepad gamepad; // ゲームパッドを使えるように宣言

    // アナログ入力用
    private float stickThreshold = 0.5f; // スティックの傾き具合の閾値
    private bool prevStickUp = false; // 前フレームでスティック上方向に倒していたかどうか
    private bool prevStickDown = false; // 前フレームでスティック下方向に倒していたかどうか

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
        //if (GameManager.isGameOver)
        //{
        //    menuButtons[0].label.text = "Retry";
        //}
        //else
        //{
        //    menuButtons[0].label.text = "NextStage";
        //}
        menuButtons[0].label.text = "NextStage";

        // 特定の条件のとき、次のステージボタンを無効化
        if (GameManager.currentStageIndex == 1 /*&& !GameManager.isGameOver*/)
        {
            isAllClear = true;
        }
        gamepad = Gamepad.current;
    }

    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新する

        // 左スティックの上下入力検知
        Vector2 stick = (gamepad != null) ? gamepad.leftStick.ReadValue() : Vector2.zero;
        bool stickUp = (stick.y > stickThreshold) && !prevStickUp;
        bool stickDown = (stick.y < -stickThreshold) && !prevStickDown;
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
            if (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))
            {
                BGMManager.Instance.StopBGMWithFade(1f);
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
            if (Input.GetKeyDown(KeyCode.DownArrow) || (gamepad != null && gamepad.dpad.down.wasPressedThisFrame) || stickDown)
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
                UpdateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || (gamepad != null && gamepad.dpad.up.wasPressedThisFrame) || stickUp)
            {
                selectedIndex = Mathf.Min(menuButtons.Length - 1, selectedIndex + 1);
                UpdateSelection();
            }

            // 選択中のボタンを点滅
            BlinkSelectedButton();

            // 決定処理
            if (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))
            {
                if (selectedIndex == 0)
                {
                    BGMManager.Instance.ChangeBGM("Stage2", 1.5f);
                    SceneManager.LoadScene("LoadScene");
                    Title.ToTitle = false;
                }
                else
                {
                    BGMManager.Instance.StopBGMWithFade(1f);
                    SceneManager.LoadScene("Title");
                }
            }
        }
        // 前フレームのスティック入力保存
        prevStickUp = (stick.y > stickThreshold);
        prevStickDown = (stick.y < -stickThreshold);
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

