/*=====
<EnemyPattern.cs>
���쐬�ҁFtooyama

�����e
Enemy�̍��G�E�U�����s���X�N���v�g

�����ӎ���

���X�V����
Y25   
_M05    
__D     
___23:�v���O�����쐬:tooyama

=====*/
using System.Collections;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    [Header("�X�e�[�^�X")]
    [SerializeField, Tooltip("���G�͈�(���a)")] private float m_fSearchRange = 15.0f;    // �v���C���[���N��������U������͈�
    [SerializeField, Tooltip("���G�͈̓I�u�W�F�N�g�i�q�I�u�W�F�N�g�j")] private GameObject m_SearchRangeObject;

    [Header("�U���֌W")]
    [SerializeField, Tooltip("���̃v���n�u")] private GameObject m_BulletPrefab;
    [SerializeField, Tooltip("�U���Ԋu(�b)")] private float m_fShotInterval = 2.0f;
    [SerializeField, Tooltip("�e�̑��x")] private float m_fShotSpeed = 3.0f;

    private Transform m_targetPlayer; // �U���Ώ�

    private Coroutine m_attackCoroutine; // �R���[�`��

    private bool m_bIsAttacking = false; // �U������

    private SphereCollider m_SearchCollider; // ���G�Ɏg�p����X�t�B�A�R���C�_�[

    /*��Start�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:������
    */
    void Start()
    {
        m_SearchCollider = m_SearchRangeObject.GetComponent<SphereCollider>();
        // ���a��ݒ� �����G�͈͂�100����1�Ɋ����Ă���̂̓X�t�B�A�R���C�_�[��radius�ɍ��킹�邽��
        m_SearchCollider.radius = m_fSearchRange / 100.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�����G�͈͂ɐN��������
        if (other.gameObject.CompareTag("Player") && !m_bIsAttacking)
        {
            // �N�����̈ʒu���L�^
            m_targetPlayer = other.transform; 
            // 2�b���Ƃɒe�𔭎˂���R���[�`���J�n
            m_attackCoroutine = StartCoroutine(ShootWebPeriodically());
            // �U�����t���O���I����
            m_bIsAttacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �v���C���[�����G�͈͂��痣�ꂽ��
        if (other.CompareTag("Player") && m_attackCoroutine != null)
        {
            // �U���R���[�`�����~ 
            StopCoroutine(m_attackCoroutine);
            // �R���[�`���̎Q�Ƃ��N���A
            m_attackCoroutine = null;
            // �^�[�Q�b�g�����Z�b�g
            m_targetPlayer = null;
            // �U�����t���O���I�t��
            m_bIsAttacking = false;
        }
    }

    /*�����G�͈͕`��֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�V�[�����ō��G�͈͂�`�悳����
    */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_fSearchRange);
    }
    /*���U���֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�v���C���[�Ɍ����Ď��𔭎˂���
    */
    private void Attack()
    {
        // null�`�F�b�N
        if (!m_targetPlayer || !m_BulletPrefab) return;
        // ���̐����ʒu�����߂�
        Vector3 spawnPos = transform.position;
        // �����΂��ʒu�����߂�
        Vector3 direction = (m_targetPlayer.position - spawnPos).normalized;

        // �v���C���[�̑��x���擾
        Player player = m_targetPlayer.GetComponent<Player>();
        if (!player) return;

        float f_PlayerSpeed = player.PlayerSpeed;

        GameObject ShotWeb = Instantiate(m_BulletPrefab, spawnPos, Quaternion.LookRotation(direction));

        // ShotWeb�X�N���v�g���擾���A������
        ShotWeb webScript = ShotWeb.GetComponent<ShotWeb>();
        if (webScript)
        {
            webScript.Init(direction, m_fShotSpeed, f_PlayerSpeed);
        }
    }

    /*�������˃R���[�`��
    �����F�Ȃ�
    ��
    �ߒl�F�U���p�x(m_fShotInterval)�̕b��
    ��
    �T�v:�ݒ肵���b�����Ɏ��𔭎˂�����
    */
    private IEnumerator ShootWebPeriodically()
    {
        while (true)
        {
            if (m_targetPlayer != null) Attack();

            yield return new WaitForSeconds(m_fShotInterval);
        }
    }
}
