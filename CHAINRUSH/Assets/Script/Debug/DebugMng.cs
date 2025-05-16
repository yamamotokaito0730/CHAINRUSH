/*=====
<DebugMode.cs>
���쐬�ҁFyamamoto

�����e
�f�o�b�O�\�����Ǘ�����X�N���v�g

�����ӎ���



���X�V����
Y25   
_M05
__D
___16:�v���O�����쐬:yamamoto

=====*/

using UnityEngine;

public class DebugMng : MonoBehaviour
{
    [Header("Player���f��")]
    [SerializeField] private Player m_Player;

    [Header("canvas")]
    [SerializeField] private GameObject m_DebugCanvas;

    [Header("�f�o�b�O���[�h")]
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
            //�\����\���؂�ւ�
            m_On = !m_On;
            m_DebugCanvas.SetActive(m_On);
        }
    }
}
