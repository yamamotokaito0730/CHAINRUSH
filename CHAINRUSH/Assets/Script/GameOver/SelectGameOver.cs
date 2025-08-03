using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectGameOver : MonoBehaviour
{
    [System.Serializable]
    public class MenuButton
    {
        public Button button;
        public TextMeshProUGUI label;
    }

    public MenuButton[] menuButtons; // 0: リトライ, 1: タイトル
    public TextMeshProUGUI GameOverUI;
    private int selectedIndex = 0;
    private float blinkTime = 0f;
    private bool isMenuActive = false;
    private float timer = 0f;
    private Gamepad gamepad; // ゲームパッドを使えるように宣言

    // アナログ入力用
    private float stickThreshold = 0.5f; // スティックの傾き具合の閾値
    private bool prevStickLeft = false; // 前フレームでスティック左方向に倒していたかどうか
    private bool prevStickRight = false; // 前フレームでスティック右方向に倒していたかどうか


    void Start()
    {
        isMenuActive = false;
        // 最初は非表示
        foreach (var mb in menuButtons)
        {
            mb.button.gameObject.SetActive(false);
        }
        GameOverUI.gameObject.SetActive(false);

        // GameManagerの状態によってボタンのラベルを変更
        if (GameManager.isGameOver)
        {
            menuButtons[0].label.text = "Retry";
        }
        else
        {
            menuButtons[0].label.text = "NextStage";
        }
        gamepad = Gamepad.current;
    }

    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新

        // 左スティックの上下入力検知
        Vector2 stick = (gamepad != null) ? gamepad.leftStick.ReadValue() : Vector2.zero;
        bool stickRight = (stick.x < stickThreshold) && !prevStickRight;
        bool stickLeft = (stick.x > -stickThreshold) && !prevStickLeft;

        // 6.0秒の待機
        if (!isMenuActive)
        {
            timer += Time.deltaTime;
            if (timer >= 6.0f)
            {
                ActivateMenu();
                GameOverUI.gameObject.SetActive(true);
            }
            return;
        }

        // 入力処理
        if (Input.GetKeyDown(KeyCode.RightArrow) || (gamepad != null && gamepad.dpad.right.wasPressedThisFrame) || stickRight)
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || (gamepad != null && gamepad.dpad.left.wasPressedThisFrame) || stickLeft)
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
                SceneManager.LoadScene("LoadScene");
                Title.ToTitle = false;
            }
            else
            {
                SceneManager.LoadScene("Title");
            }
        }
        // 前フレームのスティック入力保存
        prevStickRight = (stick.x < stickThreshold);
        prevStickLeft = (stick.x > -stickThreshold);
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
