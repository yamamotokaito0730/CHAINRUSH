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

using UnityEngine;

public class EnemyStick : MonoBehaviour
{

    [Header("�G�t�F�N�g")]
    [Header("�G�t�F�N�gCube�̃v���n�u")]
    [SerializeField] private GameObject cubePrefab;

    [Header("��������Cube�̐�")]
    [SerializeField] private int cubeCount;

    [Header("Cube�̏o���͈�")]
    [SerializeField] private float spawnRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GenerateEffectCubes();
            Destroy(gameObject); // �G���g��j��
            Debug.Log("�v���C���[�ɓ��������I");
        }
    }

    /*���G�t�F�N�g�pCube�����֐�
   �����F�Ȃ�
   ��
   �ߒl�F�Ȃ�
   ��
   �T�v:�w�肵�����a���̃����_���Ȉʒu�ɃG�t�F�N�g�p�̃L���[�u�𕡐���������
   */
    public void GenerateEffectCubes()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            // �w�肳�ꂽ���a���̃����_���Ȉʒu���Z�o
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPos = transform.position + randomOffset;

            // �L���[�u�𐶐�
            GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

            // Rigidbody ���t���Ă��Ȃ��ꍇ�͒ǉ��i�O�̂��߁j
            if (!cube.TryGetComponent(out Rigidbody rb))
            {
                rb = cube.AddComponent<Rigidbody>();
            }
        }
    }
}
