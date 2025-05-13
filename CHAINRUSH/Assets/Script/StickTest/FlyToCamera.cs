// ����1
/*=====
<FlyToCamera.cs>
���쐬�ҁFmori

�����e
�J�����Ɍ�������EffectCube���΂�
��苗���߂Â����璣��t������

�����ӎ���


���X�V����
Y25   
_M05
___14:�v���O�����쐬:mori

=====*/

using UnityEngine;
using System.Collections;

public class FlyToCamera : MonoBehaviour
{
    private UnityEngine.Camera mainCamera;
    private Transform cameraTransform;
    private bool hasReachedTarget = false; // �Ώۈʒu�ɓ��B�������ǂ����̃t���O�i����t�������̐؂�ւ��Ɏg�p�j
    private Vector3 localOffset;           // �J�����ɑ΂��郍�[�J���ȃI�t�Z�b�g�ʒu

    [Header("�J������O�̋���")]
    [SerializeField] private float stopDistanceFromCamera = 2f;

    [Header("���ł���ő�X�s�[�h")]
    [SerializeField] private float maxFlySpeed = 40f;

    [Header("��������")]
    [SerializeField] private float accelerationTime = 0.3f;

    [Header("��~���锻�苗��")]
    [SerializeField] private float stopThreshold = 0.1f;

    [Header("����t���Ƃ��̈ʒu�����_�����a")]
    [SerializeField] private float attachRandomRadius = 0.5f;

    void Start()
    {
        mainCamera = UnityEngine.Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        cameraTransform = mainCamera.transform;

        // ����t���ʒu�ɑ΂��郉���_���ȃ��[�J���I�t�Z�b�g�����
        Vector2 randomCircle = Random.insideUnitCircle * attachRandomRadius;
        localOffset = cameraTransform.forward * stopDistanceFromCamera
                    + cameraTransform.right * randomCircle.x
                    + cameraTransform.up * randomCircle.y;

        // �J�����֌������������J�n
        StartCoroutine(FlyToTargetCoroutine());
    }


    /*���J�����Ɍ������Ĕ�΂��֐�
   �����F�Ȃ�
   ��
   �ߒl�F�Ȃ�
   ��
   �T�v:�J�����Ɍ������ď��X�ɉ������Ȃ���ڋ߂���R���[�`���B
        ��苗���܂Őڋ߂������~���A�J������O�Œ���t���B
   */
    private IEnumerator FlyToTargetCoroutine()
    {
        float elapsedTime = 0f;

        while (true)
        {
            if (mainCamera == null) yield break;

            // ����t���ڕW�̃��[���h���W
            Vector3 worldTarget = cameraTransform.position + localOffset;

            // ���݈ʒu����ڕW�ʒu�ւ̕����Ƌ������v�Z
            Vector3 direction = worldTarget - transform.position;
            float distance = direction.magnitude;

            // ��~���鋗���ɓ��������~
            if (distance <= stopThreshold)
                break;

            direction.Normalize();

            // ���Ԍo�߂ɉ����đ��x������
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / accelerationTime);
            float currentSpeed = Mathf.Lerp(0f, maxFlySpeed, t);

            // �J�����Ɍ������Đi��
            transform.position += direction * currentSpeed * Time.deltaTime;

            yield return null;
        }

        // ����t����ԂɈڍs
        hasReachedTarget = true;
        transform.rotation = cameraTransform.rotation;
        transform.position = cameraTransform.position + localOffset;

        // Rigidbody ������� isKinematic �� true �ɂ��ĕ����������~
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // �����\���𑱂��Ă���I�u�W�F�N�g���폜
        Destroy(gameObject, 0.5f);
    }

    void Update()
    {
        if (hasReachedTarget && mainCamera != null)
        {
            // ����t������Ԃ��ێ��i�J�����̑O�ɌŒ�\���j
            // �J�����ɑ΂��郍�[�J���ʒu���ێ�����i�����ڂ̓s�^�~�܂�j
            transform.position = cameraTransform.position + localOffset;
            transform.rotation = cameraTransform.rotation;
        }
    }
}
