/*=====
<DebugMode.cs>
└作成者：yamamoto

＞内容
デバッグ表示を管理するスクリプト

＞注意事項



＞更新履歴
Y25   
_M05
__D
___16:プログラム作成:yamamoto

=====*/

using UnityEngine;

public class DebugMng : MonoBehaviour
{
    [Header("Playerモデル")]
    [SerializeField] private Player m_Player;

    [Header("canvas")]
    [SerializeField] private GameObject m_DebugCanvas;

    [Header("デバッグモード")]
    [SerializeField] private bool m_On;

    private DebugMode m_DebugMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_DebugMode = m_DebugCanvas.GetComponent<DebugMode>();
        m_DebugCanvas.SetActive(m_On);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_On)
            m_Player.DebugMode(m_DebugMode);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //表示非表示切り替え
            m_On = !m_On;
            m_DebugCanvas.SetActive(m_On);
        }
    }
}
