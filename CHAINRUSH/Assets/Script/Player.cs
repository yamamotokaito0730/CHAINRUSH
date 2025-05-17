/*=====
<Player.cs>
���쐬�ҁFyamamoto

�����e
Player�̋������Ǘ�����X�N���v�g

�����ӎ���


���X�V����
Y25   
_M04    
__D     
___11:�v���O�����쐬:yamamoto   
___12:�X�R�A�f�o�b�N�p�̃v���O������ǉ�:yamamoto
___22:�ړ��̎d�l�ύX:yamamoto
___27:�v���C���[�̈ړ���AD�L�[�݂̂ɕύX:mori
_M05
___01:���x�ɂ��킹�ďd�͂𑝉����鏈����ǉ�:tooyama
___09:�s�K�v�Ȉ����A�ϐ��錾���폜:yamamoto
___11:�o�E���h�h�~������ǉ�:tooyama
___14:�G�l�~�[���������Ăяo����ǉ�:mori
___16:���t�@�N�^�����O:yamamoto
___17:��̊p�x�ɉ����������������̒ǉ�:tooyama
=====*/

using UnityEngine;

public class Player : MonoBehaviour
{
    // �ϐ��錾
    [Header("�X�e�[�^�X")]
    [SerializeField, Tooltip("�ړ����x")] private float m_fSpeed;
    [SerializeField, Tooltip("������")] private float m_fBoost;
/*
    [Header("�f�o�b�O")]
    [SerializeField, Tooltip("�f�o�b�O�\��")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("�f�o�b�O�v���n�u�擾")] private GameObject debugPrefab;
*/
    [Header("�d�͊֌W")]
    [SerializeField, Tooltip("�x�[�X�̏d��")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("�d�͂̑�����")] private float m_fAddGravity = 3.0f;

    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // �v���C���[�̕��������𐧌䂷�邽�߂�Rigidbody
    private int nEnemyKillCount = 0; // �|�����G�̐�
    private int m_nPrevSlopeAngleKey = int.MinValue; // �O�t���[���œK�p���ꂽ�X�Ίp�i10�x�P�ʁj
    private float m_fRecordedBaseSpeed = 0.0f; // �X�΂ɓ������u�Ԃ̑��x�L�^�p

    /*��Start�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:������
    */
    void Start()
    {
        mainCamera = UnityEngine.Camera.main;
        rb = GetComponent<Rigidbody>();  // Rigidbody�̎擾
                                         
        m_fRecordedBaseSpeed = m_fSpeed; // �X�΂ɓ������u�Ԃ̑��x�L�^�Ə������x�𓯊�������

    }

    /*��FixedUpdate�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:���Ԋu�ōX�V
    */
    void FixedUpdate()
    {
        // �����Ă�������ɐi�ݑ�����
        rb.linearVelocity = new Vector3(
            transform.forward.x * m_fSpeed,
            rb.linearVelocity.y,
            transform.forward.z * m_fSpeed
            );

        // Y���W�ɐ������|����
        ClampPlayerHeight();

        // �d�͂̒ǉ�
        rb.AddForce(Vector3.down * m_fBaseGravity, ForceMode.Acceleration);

        // ��̊p�x�̍X�V
        float slope = GetGroundSlope();

        // �p�x���ۂ߂�(��ԑJ�ڂ̌��o�p)
        int slopeKey = Mathf.RoundToInt(slope / 10.0f) * 10;

        // �n�ʂɗ����Ă���A�X�΂ɓ������ꍇ
        if (slope != -1.0f && slopeKey != m_nPrevSlopeAngleKey)
        {
            // ���߂ČX�΂ɓ������Ƃ��������x���L�^
            if (m_nPrevSlopeAngleKey == 0)
                m_fRecordedBaseSpeed = m_fSpeed;

            // ��̊p�x����������l�����߂�
            float boost = ApplySlopeSpeedBoost(slope);
            // ApplySlopeSpeedBoost�֐��̖߂�l���������ɍs��
            AddBoost(boost);
            //�{�t���[���̌X�Ίp��ۑ����A�Q�x�ڂ̉�������h��
            m_nPrevSlopeAngleKey = slopeKey;
        }

    }

    /*��Update�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�X�V�֐�
    */

    private void Update()
    {
        //////////////////////////////////////////////////////////
        //�f�o�b�O�p
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_fSpeed += m_fBoost; // �����f�o�b�O�p
            m_fRecordedBaseSpeed += m_fBoost;
            AddGravity();
        }
        ////////////////////////////////////////////////////

        rotation();
    }

    /*����]�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�v���C���[�̌�������]������
    */
    private void rotation()
    {
        float rotateSpeed = 100.0f; // ��]���x

        float turn = 0.0f;

        if (Input.GetKey(KeyCode.A)) turn = -1.0f; // ����]
        if (Input.GetKey(KeyCode.D)) turn = 1.0f;  // �E��]

        if (turn != 0.0f)
        {
            // Y���𒆐S�ɉ�]������
            transform.Rotate(0.0f, turn * rotateSpeed * Time.deltaTime, 0.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die(mainCamera); // �G�l�~�[��������
                AddBoost(m_fBoost);
                m_fRecordedBaseSpeed += m_fBoost;
                AddGravity();
                nEnemyKillCount++; // �L���J�E���g�̑���
            }
        }
    }

    /*�������x�����֐�
   �����Ffloat _boost:��������l
   ��
   �ߒl�F�Ȃ�
   ��
   �T�v:�v���C���[�̑��x��������
   */
    public void AddBoost(float _boost)
    {
        m_fSpeed += _boost;
    }

    /*���d�͑����֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�����x�����ɍ��킹�ďd�͂𑝉�������
    */
    private void AddGravity()
    {
        m_fBaseGravity += m_fAddGravity; // �d�͂̑���
        m_fBaseGravity = Mathf.Min(m_fBaseGravity, 40.0f); // ���(40.0f)�𒴂��Ȃ��悤�ɐݒ�
    }

    /*�����x�����֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v: �v���C���[���ł��ڂ������n�`�Œ��˂�悤�Ɍ����Ă��܂�����h�����߁A
          �v���C���[��Y���W�i�����j�ɏ����݂��āA�n�ʂɂ������悤�Ɉړ�������
    */
    private void ClampPlayerHeight()
    {
        float rayStartOffsetY = 0.1f; // �n�ʂƂ̂߂荞�݂�h�����߁ARaycast�̎n�_��������ɂ��炷
        Vector3 rayOrigin = transform.position + Vector3.up * rayStartOffsetY; // �����ォ��Ray�𔭎�
        RaycastHit hit; // �n�ʂƂ̓����蔻��p

        // �n�ʂɗ����Ă���(�����̒n�ʂ�Ray���q�b�g����)�ꍇ�̂ݏ������s��
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10.0f))
        {
            
            float groundY = hit.point.y; //�n�ʂ̍���
            float maxHeight = groundY + 1.0f;   // ���e����ő�̍����i�����h�~�j

            // �v���C���[���w�肵��������蕂���Ă���ꍇ�͐�����������
            if (transform.position.y > maxHeight)
            {
                Debug.Log("����");
                // Y���W�ɐ������|���č�������������
                Vector3 correctedPos = transform.position;
                correctedPos.y = maxHeight;
                transform.position = correctedPos;

                // �㏸����Y���x��0�ɗ}����
                Vector3 velocity = rb.linearVelocity;
                velocity.y =-3.0f;
                rb.linearVelocity = velocity;
            }
        }

    }

    public void DebugMode(DebugMode _debug)
    {
        _debug.UpdateDebugUI(transform, m_fSpeed, nEnemyKillCount); // �f�o�b�OUI�̍X�V
    }

    /*���p�x�擾�֐�
    �����F�Ȃ�
    ��
    �ߒl�F��̊p�x
    ��
    �T�v: �v���C���[�������Ă����̊p�x���擾����
          ���̕����t���p�x�� ApplySlopeSpeedBoost() ��
          10���P�ʂɊۂ߂��A���x�␳�e�[�u���ɓn�����
    */
    private float GetGroundSlope()
    {
        float rayLength = 2.0f; // Raycast �����i��������p�j
        Vector3 origin = transform.position; // Ray �����ʒu

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
        {
            // �v���C���[�i�s�����ƎΖʕ��������߂�
            Vector3 moveDir = rb.linearVelocity.normalized; // �i�s����
            Vector3 slopeDir = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal).normalized; // �Ζʕ���

            //  �X�Ίp�̐�Βl�����߂�
            float angleAbs = Vector3.Angle(hit.normal, Vector3.up);

            // �o�肩���肩����ςŔ��肵������t�^
            float dot = Vector3.Dot(moveDir, slopeDir);
            float signedAngle = (dot >= 0) ? angleAbs  // moveDir �Ɠ������� �� ����
                                           : -angleAbs; // �t���� �� ���

            return signedAngle; // �����ŕԂ����p�x�� ApplySlopeSpeedBoost�֐��Ŏg�p����
        }
        else
        {
            return -1.0f; // �󒆂ɕ����Ă���A�n�ʂ�������Ȃ��ꍇ��-1��Ԃ�������s��Ȃ�
        }
    }

    /*����̌X�Ίp�ɂ������E���������֐�
   �����F�X�Ίp
   ��
   �ߒl�F�����x�p�����[�^
   ��
   �T�v:��̌X�Ίp�ɉ����ăv���C���[���x�𑝌�������
   */
    private float ApplySlopeSpeedBoost(float _slopeAngle)
    {
        // �n�ʂ����o����Ȃ�����
        if (_slopeAngle == -1.0f) return 0.0f;

        // �p�x���ۂ߂�(���W�b�N�v�Z�p)
        int slopeKey = Mathf.RoundToInt(_slopeAngle / 10.0f) * 10;

        // 30�x�𒴂����X�Ίp��30�x�Ƃ���
        if (slopeKey < -30.0f) slopeKey = -30;
        else if (slopeKey > 30.0f) slopeKey = 30;

        // �X�Ίp�ɉ����ĉ����E��������l�����߂�
        switch (slopeKey)
        {
            case -30:
                return 2.0f;
            case -20:
                return 1.5f;
            case -10:
                return 1.0f;
            case 0:
                return m_fRecordedBaseSpeed - m_fSpeed; // ���n�ɖ߂�ہA���̑��x�ɖ߂�
            case 10:
                return -1.0f;
            case 20:
                return -1.5f;
            case 30:
                return -2.0f;
            default: return 0.0f;

        }
    }

}