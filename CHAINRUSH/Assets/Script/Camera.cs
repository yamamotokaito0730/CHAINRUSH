/*=====
<Camera.cs>
���쐬�ҁFyamamoto

�����e
Camera�̋������Ǘ�����X�N���v�g

�����ӎ���
�v���C���[���_�b�V���i�����j����d�l���ǉ����ꂽ�Ƃ��ύX�K�{


���X�V����
Y25   
_M04    
__D     
___11:�v���O�����쐬:yamamoto   //���t:�ύX���e:�{�s��
___27:�J�����ړ��������L�[�ōs���悤�ɕύX:mori
_M05
__D
___14:�J�����ړ������L�[����ǉ�:yamamoto

=====*/
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour
{
    //�ϐ��錾
    [Header("�^�[�Q�b�g")]
    [SerializeField] private Transform m_Target;

    [Header("�J�����̃I�t�Z�b�g")]
    [SerializeField] private Vector3 m_Offset = new Vector3(0.0f, 5.0f, -7.0f);

    [Header("�J�����̉�]���x")]
    [SerializeField] private float m_RotationSpeed = 100.0f;

    private float m_Yaw = 0.0f; // ���������̉�]��
    private float m_Pitch = 0.0f; // ���������̉�]��

    // �V�F�C�N�p�ϐ�
    private Vector3 m_ShakeOffset = Vector3.zero;
    private float m_ShakeDuration = 0f;
    private float m_ShakeMagnitude = 0.1f;

    /*��Start�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:������
    */
    private void Start()
    {
        // �^�[�Q�b�g�����ݒ�Ȃ�v���C���[��T���Đݒ�
        if (m_Target == null)
        {
            m_Target = GameObject.FindWithTag("Player").transform;
        }

        m_Yaw = transform.eulerAngles.y; // ���݂�Y���p�x���擾
        m_Pitch = transform.eulerAngles.x; // ���݂�X���p�x���擾
    }

    /*��LateUpdate�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:Update�֐��̌�ɍX�V�����֐�
    */
    void LateUpdate()
    {
        // �����L�[���͂ŃJ������Y����]
        float horizontalInput = 0.0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1.0f;

        // �����L�[���͂�X���i�㉺�j��]
        float verticalInput = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalInput = 1.0f;
        if (Input.GetKey(KeyCode.DownArrow)) verticalInput = -1.0f;

        // ��]�p�x�̍X�V
        m_Yaw += horizontalInput * m_RotationSpeed * Time.deltaTime;
        m_Pitch += verticalInput * m_RotationSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, -20.0f, 20.0f);  //��]�̐���

        // �J�����ʒu���^�[�Q�b�g�̈ʒu�{�I�t�Z�b�g�ɐݒ�
        Vector3 targetPosition = m_Target.position + Quaternion.Euler(m_Pitch, m_Yaw, 0.0f) * m_Offset;
        transform.position = targetPosition;

        // �^�[�Q�b�g����Ɍ���
        //transform.LookAt(m_Target.position);

        // �V�F�C�N����
        if (m_ShakeDuration > 0f)
        {
            m_ShakeOffset = Random.insideUnitSphere * m_ShakeMagnitude;
            m_ShakeDuration -= Time.deltaTime;
        }
        else
        {
            m_ShakeOffset = Vector3.zero;
        }

        transform.position = targetPosition + m_ShakeOffset;
        transform.LookAt(m_Target.position);
    }

    /// <param name="duration">�h��鎞�ԁi�b�j</param>
    /// <param name="magnitude">�h��鋭��</param>
    public void ShakeCamera(float duration, float magnitude)
    {
        m_ShakeDuration = duration;
        m_ShakeMagnitude = magnitude;
    }
}