/*=====
<NationalObject.cs> 
└作成者：banno

＞内容
Terrain上に生成されるオブジェクトの情報をまとめて格納したスクリプト

＞注意事項   
ScriptableObjectで生成して扱ってください

＞更新履歴
Y25   
_M06
__D
___13:スクリプトを作成
___16:タグを複数作成、管理方法を追加
=====*/


using UnityEngine;

public enum TypeObject
{
    Tree,
    Rock1,
    Rock2,
    Other,
};


[CreateAssetMenu(fileName = "NationalObject", menuName = "Scriptable Objects/NationalObject")]
public class NationalObject : ScriptableObject
{
    [Header("パーティクル")]
    [SerializeField, Tooltip("表示するパーティクル")] public GameObject[] m_particle;
    [SerializeField, Tooltip("モデルからずらす分の座標")] public Vector3 m_shiftPosition;

    [Header("オブジェクト")]
    [SerializeField, Tooltip("判定を取るオブジェクトタイプ")] public TypeObject type;
    [SerializeField, Tooltip("壊される元モデル")] public GameObject m_breakModel;
    [SerializeField, Tooltip("切断されたプレハブの上部")] public GameObject m_upperDisconnectedPrefab;
    [SerializeField, Tooltip("切断されたプレハブの下部")] public GameObject m_bottomDisconnectedPrefab;
}
