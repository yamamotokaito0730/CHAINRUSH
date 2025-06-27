/*=====
<EnemySerachSensor.cs>
└作成者：tooyama

＞内容
敵の索敵範囲（SphereCollider）にプレイヤーが侵入・離脱したことを検知し、  
EnemyPattern 本体の処理(HandleSensorEnter/Exit)を呼び出す補助スクリプト

＞注意事項
このスクリプトをアタッチする GameObject に SphereCollider（isTrigger=true）が必要
＞更新履歴
Y25   
_M05    
__D     
___26:プログラム作成:tooyama

=====*/
using UnityEngine;

public class EnemySerachSensor : MonoBehaviour
{
    [SerializeField, Tooltip("親のオブジェクト(spider)")]  private EnemyPattern owner;  // ← Inspector で本体をドラッグして参照渡し

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;
        owner.HandleSensorEnter(other);   // 本体側の処理を呼ぶ
    }

    private void OnTriggerExit(Collider other)
    {
        if (owner == null) return;
        owner.HandleSensorExit(other);    // 本体側の処理を呼ぶ
    }
}
