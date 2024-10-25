using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    public GameObject GameObject;
    public IObjectPool<GameObject> Pool;
    public void Setup(GameObject go ,IObjectPool<GameObject> pool)
    {
        GameObject = go;
        Pool = pool;
    }
    [Button]
    public void Release()
    {
        Pool.Release(GameObject);
    }
}
