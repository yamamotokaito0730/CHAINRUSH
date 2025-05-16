/*=====
<FlyToCamera.cs>
���쐬�ҁFmori

�����e
�J�����Ɍ������Ĕ��ł��āA��苗���ɂȂ����璣��t��

�����ӎ���


���X�V����
Y25         
_M05
__D  
___14:�v���O�����쐬:mori
___15:�J�����̐U���Ăяo��������t���ꏊ�ύX:yamamoto

=====*/
using System.Collections;
using UnityEngine;

public class FlyToCamera : MonoBehaviour
{
   // private UnityEngine.Camera mainCamera;
    private Camera mainCamera;

    /*��StartFly�֐�
    �����FUnityEngine.Camera camera:���C���̃J����
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�O������Ăяo���ăJ������ݒ肵�A�������J�n
    */
    public void StartFly(UnityEngine.Camera _camera)
    {

        mainCamera = _camera.GetComponent<Camera>();
        
        if (mainCamera != null)
        {
            // �J�����Ɍ������Ĕ�ԁ�����t�������J�n
            StartCoroutine(FlyAndStick(_camera));
        }
        else
        {
            Debug.LogError("�J�������ݒ肳��Ă��܂���");
        }
    }

    /*��FlyAndStick�֐�
    �����FUnityEngine.Camera camera:���C���̃J����
    ��
    �ߒl�F�Ȃ�
    ��
    �T�v:�J�����Ɍ������Ĕ�ԁ�����t������
    */
    private IEnumerator FlyAndStick(UnityEngine.Camera camera)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) yield break;

        // ������~���e�q�t��
        rb.isKinematic = true;
        transform.SetParent(camera.transform);

        Vector3 startLocalPos = camera.transform.InverseTransformPoint(transform.position);

        // �����_���ȃ|�W�V�����ݒ�
        //int randX = Random.Range(0, 2) * 2 - 1;
        //int randY = Random.Range(0, 2) * 2 - 1;
        float randX;
        float randY;
        do
        {
            randX = Random.Range(-1.0f, 1.0f);
            randY = Random.Range(-1.0f, 1.0f);
        } while (Mathf.Abs(randX) < 0.5f || Mathf.Abs(randY) < 0.5f); // ���S�ɋ߂�����ꍇ�͂�蒼��

        Vector3 targetLocalPos = new Vector3(randX, randY, 2); // �J�����O���̃��[�J���ʒu

        float duration = 0.3f;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;
            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }
       mainCamera.ShakeCamera(0.1f, 0.3f);
        // ����t�����܂�1�b�\��
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
