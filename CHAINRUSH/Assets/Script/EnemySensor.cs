using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [SerializeField]
    private EnemyPattern owner;  // ← Inspector で本体をドラッグして参照渡し

    private void OnTriggerEnter(Collider other)
    {
        owner.HandleSensorEnter(other);   // 本体側の処理を呼ぶ
    }

    private void OnTriggerExit(Collider other)
    {
        owner.HandleSensorExit(other);    // 本体側の処理を呼ぶ
    }
}
