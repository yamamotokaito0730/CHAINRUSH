/*=====
<LoadScene.cs>
���쐬�ҁFmori

�����e
���[�h��ʂł̃Q�[���V�[���̔񓯊��ǂݍ���

�����ӎ���


���X�V����
Y25         
_M05
__D  
___22:�v���O�����쐬:mori

=====*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneNameToLoad = "NatureRenderer";  // �ǂݍ��ރQ�[���V�[����
    [SerializeField] private float minLoadingTime = 1.0f;  // �Œ�\�����ԁi�b�j

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        float timer = 0f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            // �i����90���ȏ�ŁA���Œ᎞�Ԃ��o�߂�����؂�ւ��\�ɂ���
            if (asyncLoad.progress >= 0.9f && timer >= minLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
