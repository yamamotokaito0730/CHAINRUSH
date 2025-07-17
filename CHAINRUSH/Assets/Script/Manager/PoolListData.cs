using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolListData", menuName = "Scriptable Objects/PoolListData")]
public class PoolListData : ScriptableObject
{
    public List<ObjectPoolManager.Pool> pools;
}
