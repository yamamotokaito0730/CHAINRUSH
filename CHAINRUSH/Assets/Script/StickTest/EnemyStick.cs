/*=====
<Enemy.cs>
���쐬�ҁFyamamoto

�����e
Enemy�̋������Ǘ�����X�N���v�g

�����ӎ���

���X�V����
Y25   
_M04    
__D     
___23:�v���O�����쐬:yamamoto

=====*/
using System.Collections;
using UnityEngine;

public class EnemyStick : MonoBehaviour
{

    [Header("�G�t�F�N�g")]
    [SerializeField, Tooltip("�p�[�c�i0:��, 1:����, 2:��, 3:���j")]
    private GameObject[] m_Parts;  // ���E���́E��E����z��ŊǗ�
    [SerializeField, Tooltip("������")] private int m_nPartsNum;    // �I�u�W�F�N�g�̐�����

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*�����Ŋ֐�
    �����F�Ȃ�
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:���̓G�����ł�����
    */
    public void Die(UnityEngine.Camera camera)
    {
        bool die = true; // ���񃋁[�v���̂�Enemy�I�u�W�F�N�g�폜

        for (int i = 0; i < m_nPartsNum; i++)
        {

            GameObject obj = Instantiate(m_Parts[i], transform.position, Quaternion.identity);
            FlyToCamera fly = obj.GetComponent<FlyToCamera>();
            fly.StartFly(camera); // �J�����Ɍ������Ĕ�ԁ�����t�������J�n

            // ���񃋁[�v���̂�Enemy�I�u�W�F�N�g�폜
            if (die)
            {
                die = false;
                Destroy(gameObject);
            }
        }
    }
}
