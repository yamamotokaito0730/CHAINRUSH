// ����1
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

=====*/

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerStick : MonoBehaviour
{
    // �ϐ��錾
    [Header("�X�e�[�^�X")]
    [SerializeField, Tooltip("�ړ����x")] private float m_fSpeed;
    [SerializeField, Tooltip("������")] private float m_fBoost;

    [Header("�f�o�b�O")]
    [SerializeField, Tooltip("�f�o�b�O�\��")] private bool m_bDebugView = false;
    [SerializeField, Tooltip("�f�o�b�O�v���n�u�擾")] private GameObject debugPrefab;

    [Header("�d�͊֌W")]
    [SerializeField, Tooltip("�x�[�X�̏d��")] private float m_fBaseGravity = 9.81f;

    [SerializeField, Tooltip("�d�͂̑�����")] private float m_fAddGravity = 3.0f;


    private UnityEngine.Camera mainCamera;
    private Rigidbody rb; // �v���C���[�̕��������𐧌䂷�邽�߂�Rigidbody
    private DebugMode debugModeInstance; // �f�o�b�OUI�i���x�E�X�΂Ȃǁj�̕\���Ǘ��p�C���X�^���X
    private int nEnemyKillCount = 0; // �|�����G�̐�

    //===============================
    ///��̊p�x�ɂ��������p����
    //private Vector3 moveDir = Vector3.forward; // ���݂̐i�s������ێ�����ׂ̕ϐ�
    //
    //// �X�Ίp�ɂ�鑬�x�ω�
    //private Dictionary<int, float> slopeSpeedTable = new Dictionary<int, float>()
    //{
    //    {-30, 2.0f}, {-20, 1.5f}, {-10, 1.0f}, {0, 0.0f}, {10, -1.0f}, {20, -1.5f}, {30, -2.0f}
    //};
    //private int lastSlopeKey = int.MinValue;
    //===============================



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

        // ������ԂŃf�o�b�O�\��ON�Ȃ�AUI�𐶐����Ă���
        if (m_bDebugView && debugModeInstance == null)
        {
            GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity); // �f�o�b�OUI�̐���
            debugModeInstance = obj.GetComponent<DebugMode>(); // DebugMode�̎擾
        }
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

        //        UpdateSlopeSpeed();
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
            AddGravity();
        }
        // �f�o�b�OUI�\��
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            m_bDebugView = !m_bDebugView; // UI�̕\����\���؂�ւ�
            if (m_bDebugView && debugModeInstance == null)
            {
                // �v���n�u����C���X�^���X�𐶐����ADebugMode���擾
                GameObject obj = Instantiate(debugPrefab, Vector3.zero, Quaternion.identity); // ���W�E��]�̓v���n�u���Őݒ�d
                debugModeInstance = obj.GetComponent<DebugMode>();
            }
            else if (!m_bDebugView && debugModeInstance != null)
            {
                Destroy(debugModeInstance.gameObject); // UI���\��(�폜)����
                debugModeInstance = null;
            }
        }

        if (debugModeInstance != null)
            debugModeInstance.UpdateDebugUI(transform, m_fSpeed, nEnemyKillCount); // �f�o�b�OUI�̍X�V

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
            EnemyStick enemy = collision.gameObject.GetComponent<EnemyStick>();
            if (enemy != null)
            {
                enemy.Die(mainCamera); // �G�l�~�[��������
                AddBoost(m_fBoost);
                AddGravity();
                nEnemyKillCount++; // �L���J�E���g�̑���
                UnityEngine.Debug.Log("atari");
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
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2.0f))
        {
            float groundY = hit.point.y; //�n�ʂ̍���
            float maxHeight = groundY + 1.1f;   // ���e����ő�̍����i�����h�~�j

            // �v���C���[���w�肵��������蕂���Ă���ꍇ�͐�����������
            if (transform.position.y > maxHeight)
            {
                // Y���W�ɐ������|���č�������������
                Vector3 correctedPos = transform.position;
                correctedPos.y = maxHeight;
                transform.position = correctedPos;

                // �㏸����Y���x��0�ɗ}����
                Vector3 velocity = rb.linearVelocity;
                velocity.y = 0.0f;
                rb.linearVelocity = velocity;
            }
        }

    }

    ////////////////////////////
    //���x�ω�
    /////*���p�x�擾�֐�
    ////�����F�Ȃ�
    ////��
    ////�ߒl�F��̊p�x
    ////��
    ////�T�v:�v���C���[�������Ă����̊p�x���擾����
    ////*/
    //private float GetGroundSlope()
    //{
    //    float rayLength = 2.0f;
    //    Vector3 origin = transform.position;

    //    RaycastHit hit;
    //    if (Physics.Raycast(origin, Vector3.down, out hit, rayLength))
    //    {
    //        // �o�� or ����̌������l�����ČX�Ίp�ɕ�����t����
    //        Vector3 moveDir = rb.linearVelocity.normalized;
    //        Vector3 slopeDir = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal).normalized;
    //        float dot = Vector3.Dot(moveDir, slopeDir);
    //        float angle = Vector3.Angle(hit.normal, Vector3.up);
    //        return dot >= 0 ? angle : -angle;
    //    }
    //    else
    //    {
    //        return -1f; // �n�ʂ�������Ȃ�����
    //    }
    //}

    ///*�����x�ω��֐�
    //�����F�Ȃ�
    //��
    //�ߒl�F�Ȃ�
    //��
    //�T�v:��̊p�x�ɂ���ăv���C���[���x�𑝌�������
    //*/
    //private void UpdateSlopeSpeed()
    //{
    //    float slope = GetGroundSlope();
    //    if (slope == -1.0f || slope < -30 || slope > 30) return;

    //    int rounded = Mathf.RoundToInt(slope / 10.0f) * 10;
    //    if (rounded != lastSlopeKey && slopeSpeedTable.ContainsKey(rounded))
    //    {
    //        float boost = slopeSpeedTable[rounded];
    //        m_fSpeed += boost;
    //        lastSlopeKey = rounded;
    //        Debug.Log($"�X��: {rounded}�� �� ���x�ω� {boost}�i���ݑ��x: {m_fSpeed}�j");
    //    }
    //}
    /////////////////////////////////
}