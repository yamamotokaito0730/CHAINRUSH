/*=====
<MiniMapIcon.cs>
���쐬�ҁFmori

�����e
�~�j�}�b�v�̎���

�����ӎ���


���X�V����
Y25         
_M05
__D  
___23:�v���O�����쐬:mori

=====*/

using UnityEngine;
using UnityEngine.UI;

public class MiniMapIcon : MonoBehaviour
{
    public RectTransform minimapPanel;  // �~�j�}�b�v��UI�p�l��
    public Transform target;            // �Ώہi�G�j
    public Transform player;            // �v���C���[��Transform
    private float mapScale = 2.0f;         // �~�j�}�b�v��̃X�P�[��
    public float displayRange = 60f;    // �\������ő勗���i���[���h���W��j

    private Image iconImage;

    void Start()
    {
        iconImage = GetComponent<Image>();
    }

    void Update()
    {
        // Destroy���ꂽ�I�u�W�F�N�g�ɂ��Ή�����null�`�F�b�N
        if (target == null || target.Equals(null) || player == null || player.Equals(null))
        {
            iconImage.enabled = false; // �O�̂��ߔ�\����
            return;
        }

        Vector3 offset = target.position - player.position;

        Vector2 offset2D = new Vector2(offset.x, offset.z);
        float distance = offset2D.magnitude;

        if (distance <= displayRange)
        {
            iconImage.enabled = true;
            Vector2 minimapPos = offset2D * mapScale;
            ((RectTransform)transform).anchoredPosition = minimapPos;
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}
