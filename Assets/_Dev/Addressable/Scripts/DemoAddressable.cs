using Addressable.Manager;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;
public class DemoAddressable : MonoBehaviour
{
    [SerializeField] AssetLoaderData _assetLoaderData;
    [SerializeField]AssetLoaderData _soundLoaderData;
    [SerializeField] List<GameObject> _gameObjects = new List<GameObject>();
    [SerializeField]Queue<GameObject> gameObjects = new Queue<GameObject>();
    private void Awake()
    {
       
    }
    [Button]
    public async void LoadObject(string key)
    {
        AssetLoaderHandle.Instance.AddPreloadData(_assetLoaderData);
        await AssetLoaderHandle.Instance.Load();

        
        AssetLoaderHandle.Instance.AssetLoaderLocationCollection["A"].Result.ForEach(location => {

            ObjectPoolManager.Instance.CreateAssetToPool(AssetLoaderHandle.Get<GameObject>(location.PrimaryKey));
            for (int i = 0; i < 1; i++)
            {
                var obj = ObjectPoolManager.Instance.Get(location.PrimaryKey);
                gameObjects.Enqueue(obj);
            }
        });
    }
    //test
    public void TryCallObject()
    {
        var bbb = Observable.Interval(System.TimeSpan.FromSeconds(1)).Subscribe(_ => {
            gameObjects.Enqueue(ObjectPoolManager.Instance.Get(AssetLoaderHandle.Instance.ObjectLoadHandleCollection.ElementAt(0).Key));
        }).AddTo(this);
        var aaa = Observable.Interval(System.TimeSpan.FromSeconds(1.1f)).Subscribe(_ => {
            ObjectPoolManager.Instance.ReturnToPool(gameObjects.Dequeue());
        }).AddTo(this);
    }
    [Button] void ReleaseObject()
    {
        AssetLoaderHandle.Instance.ReleaseAll();
    }
   
}
