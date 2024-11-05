using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoInstance<ObjectPoolManager>
{
    GameObject _gameobjectRoot,_uiRoot,_soundRoot;
    public Dictionary<string, GameObject> GameObjectCollection = new Dictionary<string, GameObject>();
    public Dictionary<string, IObjectPool<GameObject>> ObjectPoolCollection = new Dictionary<string, IObjectPool<GameObject>>();

    string _objectKey;
    RootName _root = RootName.GameObject;
    public enum PoolType
    {
        Stack,
        LinkedList
    }
    public enum RootName
    {
        GameObject,UI,Sound
    }

    public bool CollectionChecks = true;
    public int MaxPoolSize = 10;
    

    public override void Init()
    {
        base.Init();
        _gameobjectRoot = new GameObject("PoolRoot");
        _gameobjectRoot.transform.position = Vector3.zero;

        _uiRoot = new GameObject("UI");
        _uiRoot.transform.position = Vector3.zero;

        _soundRoot = new GameObject("Sound");
        _soundRoot.transform.position = Vector3.zero;
    }
    public GameObject Get(string objectName , RootName rootName = RootName.GameObject)
    {
        var find = ObjectPoolCollection.FirstOrDefault(o => o.Key == objectName).Value;
        if(find == null)
        {
            Debug.LogError($"Error form name {objectName} not find on ObjectPoolCollection");
            return null;
        }
        _objectKey = objectName;
        _root = rootName;
        return find.Get();
    }
    //public T Get<T>(string objectName,RootName rootName = RootName.GameObject) where T : Object
    //{
    //    var find = ObjectPoolCollection.FirstOrDefault(o => o.Key == objectName).Value;
    //    if (find == null)
    //    {
    //        Debug.LogError($"Error form name {objectName} not find on ObjectPoolCollection");
    //        return null;
    //    }
    //    _objectKey = objectName;
    //    _root = rootName;
    //    return find.Get() as T;
    //}
    public void CreateAssetToPool(GameObject objToPoolObject)
    {
        if (ObjectPoolCollection.ContainsKey(objToPoolObject.name)) return;
        if (GameObjectCollection.ContainsKey(objToPoolObject.name)) return;

        Debug.Log("obj "+objToPoolObject);
        GameObjectCollection.Add(objToPoolObject.name, objToPoolObject);

        IObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(CreatePool, OnTakeFormPool, OnReturnedToPool, OnDestroyPoolObject, CollectionChecks, MaxPoolSize);
        ObjectPoolCollection.Add(objToPoolObject.name, objectPool);

    }
    #region Player command
    public void ReturnToPool(GameObject gameObject)
    {
        if(gameObject.TryGetComponent(out ReturnToPool returnToPool))
        {
            returnToPool.Release();
        }
    }
    #endregion
    #region Pool Operation
    private GameObject CreatePool()
    {
        var gameObject = Instantiate(GameObjectCollection[_objectKey]);
        var pool = ObjectPoolCollection[_objectKey];


        gameObject.AddComponent<ReturnToPool>().Setup(gameObject,pool);
        switch(_root)
        {
            case RootName.GameObject:
                gameObject.transform.SetParent(_gameobjectRoot.transform);
                break;
            case RootName.UI:
                gameObject.transform.SetParent(_uiRoot.transform);
                break;
            case RootName.Sound:
                gameObject.transform.SetParent(_soundRoot.transform);
                break;
        }
        
        return gameObject;
    }
    private void OnTakeFormPool(GameObject @object)
    {
        @object.SetActive(true);
    }
    private void OnReturnedToPool(GameObject @object)
    {
        @object.SetActive(false);
    }
    private void OnDestroyPoolObject(GameObject @object)
    {
        Destroy(@object);
    }
    #endregion


    #region Get ref form ScriptableObject
    public void GetObjectFromObjectPoolSO()
    {
        //ยังไม่ได้ทำ
    }
    #endregion





}
