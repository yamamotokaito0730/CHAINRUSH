/*=====
<SpriteSheetAnim.cs>
���쐬�ҁFmori

�����e
�X�v���C�g�V�[�g�̃A�j���[�V��������

�����ӎ���


���X�V����
Y25         
_M05
__D  
___21:�v���O�����쐬:mori

=====*/

using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnim : MonoBehaviour
{
    public string spriteSheetPath = "flog"; // �ǂݍ��ރX�v���C�g�V�[�g��
    public Image targetImage;               // UI��Image�ɃA�j���[�V�����\��
    public float frameRate = 0.1f;          // �A�j���[�V�������x

    private Sprite[] sprites;
    private int[] frameIndices = { 16, 17, 18, 19 }; // 17�`20�Ԗځi�z���0�n�܂�j
    private int currentFrame = 0; // ���݂̃t���[�����[�g
    private float timer;

    void Start()
    {
        // �X�v���C�g�����ׂēǂݍ���
        sprites = Resources.LoadAll<Sprite>(spriteSheetPath);
    }

    void Update()
    {
        // �X�v���C�g�z��null�A�܂��͋�̏ꍇ�͉������Ȃ�
        if (sprites == null || sprites.Length == 0) return;

        // �o�ߎ��Ԃ����Z
        timer += Time.deltaTime;

        // ��莞�ԁiframeRate�j���Ƃɏ������s��
        if (timer >= frameRate)
        {
            // �^�C�}�[���Z�b�g
            timer = 0f;
            // ���݂̃t���[���ԍ����X�V�i���[�v����悤�ɂ���j
            currentFrame = (currentFrame + 1) % frameIndices.Length;
            // �Ώۂ�Image�R���|�[�l���g�ɁA�Ή�����X�v���C�g��ݒ�
            targetImage.sprite = sprites[frameIndices[currentFrame]];
        }
    }
}
