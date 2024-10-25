using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new ObjectToPool",menuName = "ScriptableObject/ObjectToPool",order = 10)]
public class ObjectPoolSO : MonoBehaviour
{
    public Object ObjectToCreatePool;
    public bool collectionChecks = true;
    public int maxPoolSize = 10;
}
