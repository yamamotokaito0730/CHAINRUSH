/*=====
<ShotWeb.cs>
���쐬�ҁFtooyama

�����e
Enemy�����˂��鎅(�e)���Ǘ�����X�N���v�g

�����ӎ���

���X�V����
Y25   
_M05    
__D     
___23:�v���O�����쐬:tooyama

=====*/
using UnityEngine;


public class ShotWeb : MonoBehaviour
{
    [Header("�e�̃X�e�[�^�X")]
    [SerializeField, Tooltip("�f�X�|�[���b��")] private float m_fLifeTime = 5.0f;

    private Vector3 m_direction; // �����΂��ʒu
    private float m_fShotSpeed;

    /*��Init�֐�
   ����1�F�^�[�Q�b�g�̈ʒu
   ��
   ����2�F�e�̑��x
   ��
   ����3�F�v���C���[���x
   ��
   �ߒl�F�Ȃ�
   ��
   �T�v:���̖ڕW�ʒu����ƃv���C���[���x�����̑��x�ɉ��Z������
   */
    public void Init(Vector3 _Direction,float _fShotSpeed, float _fPlayerSpeed)
    {
        m_direction = _Direction.normalized;
        m_fShotSpeed = _fPlayerSpeed + _fShotSpeed; // ���̑��x���v���C���[���x�ɉ��Z������
        Destroy(gameObject, m_fLifeTime); // ��莞�Ԍ�ɏ��ł�����
    }
    /*��Update�֐�
     �����F�Ȃ�
     ��
     �ߒl�F�Ȃ�
     ��
     �T�v:�����^�[�Q�b�g�Ɍ������Ĕ�΂�
     */
    void Update()
    {
        transform.position += m_direction * m_fShotSpeed * Time.deltaTime;  
    }

    /*��isDestroy�֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�Ă΂ꂽ�玅�����ł�����(�v���C���[�ɒe�������������Ɏg�p)
    */
    public void isDestroy()
    {
        Destroy(gameObject);
    }
}
