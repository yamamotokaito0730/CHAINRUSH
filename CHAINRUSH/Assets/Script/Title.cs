/*=====
<Title.cs>
���쐬�ҁFmori

�����e
�^�C�g���̎���

�����ӎ���


���X�V����
Y25         
_M05
__D  
___22:�v���O�����쐬:mori

=====*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // ���[�h�V�[���֑J��
            SceneManager.LoadScene("LoadScene");
        }
    }
}
