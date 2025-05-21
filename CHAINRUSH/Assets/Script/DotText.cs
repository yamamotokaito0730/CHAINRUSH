/*=====
<DotText.cs>
���쐬�ҁFmori

�����e
TextMeshPro�ł�"�E�E�E"�\���̎���

�����ӎ���


���X�V����
Y25         
_M05
__D  
___21:�v���O�����쐬:mori

=====*/

using UnityEngine;
using TMPro;

public class DotText : MonoBehaviour
{
    public TextMeshProUGUI dotText;

    // �\������h�b�g�̃p�^�[���i�����Ȃ� �� 1�� �� 2�� �� 3�� �� ���[�v�j
    string[] dots = { "",".", "..", "..." };
    // ���݂̃h�b�g�C���f�b�N�X
    int index = 0;
    float timer = 0f;
    // �h�b�g��؂�ւ���Ԋu�i�b�j
    public float interval = 0.5f;

    void Update()
    {
        // �o�ߎ��Ԃ����Z
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            // dots�z��̌��݂̃C���f�b�N�X�ɉ������������\��
            dotText.text = dots[index % dots.Length];
            index++;
            timer = 0f;
        }
    }
}
