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
___16:�J�����ړ����}�E�X����ɕύX�AR�L�[�ŏ����ʒu�Ƀ��Z�b�g:mori

=====*/
using UnityEngine;

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

    // �J�����̏������Έʒu�L�^�p
    private float m_InitialYaw;
    private float m_InitialPitch;

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

        // �����̑��΃I�t�Z�b�g�Ɗp�x��ۑ�
        m_InitialYaw = m_Yaw;
        m_InitialPitch = m_Pitch;
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
        // �}�E�X���͂ɂ��J�����̉�]
        float mouseX = Input.GetAxis("Mouse X") * m_RotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_RotationSpeed * Time.deltaTime;

        // �}�E�X���͂ɃX�p�C�N������������
        mouseX = Mathf.Clamp(mouseX, -5.0f, 5.0f);
        mouseY = Mathf.Clamp(mouseY, -5.0f, 5.0f);

        m_Yaw += mouseX;
        m_Pitch -= mouseY;

        m_Pitch = Mathf.Clamp(m_Pitch, -50.0f, 20.0f);  //��]�̐���


        // R�L�[�ŏ����̃I�t�Z�b�g�Ɗp�x�Ƀ��Z�b�g�i�v���C���[�̌����ɑΉ��j
        if (Input.GetKeyDown(KeyCode.R))
        {
            // �J������Yaw���v���C���[�̌����ɍ��킹�ă��Z�b�g
            m_Yaw = m_Target.eulerAngles.y + m_InitialYaw;
            m_Pitch = m_InitialPitch;
        }

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

    /*��ShakeCamera�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�J������U��������֐�
    */
    /// <param name="duration">�h��鎞�ԁi�b�j</param>
    /// <param name="magnitude">�h��鋭��</param>
    public void ShakeCamera(float duration, float magnitude)
    {
        m_ShakeDuration = duration;
        m_ShakeMagnitude = magnitude;
    }
}