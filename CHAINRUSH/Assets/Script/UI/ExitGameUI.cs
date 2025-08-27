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
_M07
__D
___27:UI非表示関数呼び出し追加:yamamoto

=====*/

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitGameUI : MonoBehaviour
{
    public GameObject exitPanel;
    public Button yesButton;
    public Button noButton;
    [SerializeField, Tooltip("UIMng")] private UIManager m_UIMng;

    private int selectedIndex = 0;           // 現在選択中のボタン（0:Yes, 1:No）
    private float stickThreshold = 0.5f;     // スティック感度
    private bool prevStickLeft = false;      // 前フレームの左入力
    private bool prevStickRight = false;     // 前フレームの右入力

    private Gamepad gamepad; // ゲームパッドを使えるように宣言

    public TextMeshProUGUI yesButtonLabel;
    public TextMeshProUGUI noButtonLabel;

    private float blinkTime = 0f;

    void Start()
    {
        exitPanel.SetActive(false); // 最初は非表示

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        // ゲーム開始時にカーソルを非表示＆ロック
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // UIテキストを入れるInspectorが未割り当て時のみ、自動で子オブジェクトからTMProを取得する
        if (yesButtonLabel == null && yesButton != null)
            yesButtonLabel = yesButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (noButtonLabel == null && noButton != null)
            noButtonLabel = noButton.GetComponentInChildren<TextMeshProUGUI>(true);

        gamepad = Gamepad.current;
    }

    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新

        // パネルが非表示なら抜ける
        if (!exitPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || (gamepad != null && gamepad.startButton.wasPressedThisFrame))
            {
                exitPanel.SetActive(true);
                // カーソルを表示＆ロック解除
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                if (m_UIMng != null) m_UIMng.HideUI();   //UI非表示
                Time.timeScale = 0.0f;
                selectedIndex = 0; // デフォルト選択をYesに
                UpdateSelection();
            }
            return;
        }

        // --- 選択処理 ---
        Vector2 stick = (gamepad != null) ? gamepad.leftStick.ReadValue() : Vector2.zero;
        bool stickRight = (stick.x > -stickThreshold) && !prevStickRight;
        bool stickLeft = (stick.x < stickThreshold) && !prevStickLeft;

        // 右で選択をNoに
        if (Input.GetKeyDown(KeyCode.RightArrow) || (gamepad != null && gamepad.dpad.left.wasPressedThisFrame) || stickRight)
        {
            selectedIndex = 1;
            UpdateSelection();
        }
        // 左で選択をYesに
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || (gamepad != null && gamepad.dpad.right.wasPressedThisFrame) || stickLeft)
        {
            selectedIndex = 0;
            UpdateSelection();
        }

        // --- 決定処理 ---
        if (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))
        {
            if (selectedIndex == 0)
            {
                OnYesClicked();
            }
            else
            {
                OnNoClicked();
            }
        }

        BlinkSelectedButton();

        // 前フレーム入力の記録
        prevStickRight = (stick.x > -stickThreshold);
        prevStickLeft = (stick.x < stickThreshold);
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
        if (m_UIMng != null) m_UIMng.ShowUI();   //UI再表示

        Time.timeScale = 1.0f;
    }
    private void UpdateSelection()
    {
        yesButtonLabel.color = (selectedIndex == 0) ? Color.red : Color.black;
        noButtonLabel.color = (selectedIndex == 1) ? Color.red : Color.black;

        var canvasGroupYes = yesButton.GetComponent<CanvasGroup>();
        if (canvasGroupYes != null) canvasGroupYes.alpha = 1f;
        var canvasGroupNo = noButton.GetComponent<CanvasGroup>();
        if (canvasGroupNo != null) canvasGroupNo.alpha = 1f;

        blinkTime = 0f;
    }

    private void BlinkSelectedButton()
    {
        blinkTime += Time.unscaledDeltaTime * 2f;
        float alpha = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(blinkTime, 1f));

        if (selectedIndex == 0)
        {
            var canvasGroup = yesButton.GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = alpha;
        }
        else if (selectedIndex == 1)
        {
            var canvasGroup = noButton.GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = alpha;
        }
    }

}
