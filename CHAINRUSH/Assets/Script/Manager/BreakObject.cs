/*=====
<BreakObject.cs>
└作成者：banno

＞内容
壊れるオブジェクトの破片やパーティクルをプールに戻すための非同期処理用スクリプト

＞更新履歴
Y25   
_M06    
__D
___14:スクリプトを作成:banno
___16:パーティクルも対応:banno
___19:パーティクルの非同期処理を変更:banno
=====*/
using System.Collections;
using UnityEngine;

public enum Type
{
    BreakObject,
    Particle,
};

public class BreakObject : MonoBehaviour
{
    [SerializeField,Tooltip("オブジェクトのタイプ")]public Type type;

    /*＞OnEnable関数
       引数：なし
       ｘ
       戻値：なし
       ｘ
       概要：非表示状態から、表示状態になったら実行される関数
       */
    private void OnEnable()
    {
        switch(type)
        {
            // オブジェクト破片の処理
            case Type.BreakObject:
                StartCoroutine(DeleateObject());
                break;

            // パーティクルの処理
            case Type.Particle:
                StartCoroutine(WaitForParticleEnd());
                break;
        }
    }

    // オブジェクトの破片用の非同期処理
    private IEnumerator DeleateObject()
    {
        yield return new WaitForSeconds(3);
        // nameのCloneを削除しておく
        string cleanName = NameUtility.GetCleanName(gameObject);
        ObjectPoolManager.Instance.ReturnToPool(cleanName, gameObject);
    }

    // パーティクル用の非同期処理
    private IEnumerator WaitForParticleEnd()
    {
        yield return new WaitForSeconds(1.5f);

        string cleanName = NameUtility.GetCleanName(gameObject);
        ObjectPoolManager.Instance.ReturnToPool(cleanName, gameObject);
    }

}
