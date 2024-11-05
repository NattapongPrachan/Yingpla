using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
[DefaultExecutionOrder(1)]
public class AssetLoaderHandle : MonoSingleton<AssetLoaderHandle>, ILoader
{
    public event Action<float> OnLoadProgress;
    public static event Action<string> OnLoadAssetName;
    public static Subject<string> OnAssetLoaded;
    public event Action OnLoadComplete;
    public event Action OnLoadAssetFromQueueComplete;

    Action ILoader.OnComplete { get => OnLoadComplete; set => OnLoadComplete += value ; }
    Action<float> ILoader.OnProgress { get => OnLoadProgress; set => OnLoadProgress += value; }

    List<AssetLoaderData> _preloaderData;
    public Dictionary<string, AssetLoaderData> LoaderDataCollection;

    public Dictionary<string, UnityEngine.Object> ObjectCollection = new Dictionary<string, UnityEngine.Object>();
    public Dictionary<AsyncOperationHandle<IList<IResourceLocation>>, List<string>> HandleLocationCollection;
    public Dictionary<string, AsyncOperationHandle<UnityEngine.Object>> ObjectLoadHandleCollection;
    public Dictionary<string, AsyncOperationHandle<IList<IResourceLocation>>> AssetLoaderLocationCollection;
    
    
    [SerializeField]List<string>_keyToLoadAsset;
    
    public override void Init()
    {
        base.Init();
        _preloaderData = new List<AssetLoaderData>();
        OnAssetLoaded = new Subject<string>();
        HandleLocationCollection = new Dictionary<AsyncOperationHandle<IList<IResourceLocation>>, List<string>>();
        ObjectLoadHandleCollection = new Dictionary<string, AsyncOperationHandle<UnityEngine.Object>>();
        AssetLoaderLocationCollection = new Dictionary<string, AsyncOperationHandle<IList<IResourceLocation>>>();
        LoaderDataCollection = new Dictionary<string, AssetLoaderData>();
    }
    public async UniTask GetObjectFormLoader(AssetLoaderData assetLoader)
    {
        if (ObjectCollection == null)
            ObjectCollection = new Dictionary<string, UnityEngine.Object>();

        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(assetLoader.labels, Addressables.MergeMode.Union, typeof(UnityEngine.Object));
        float percent = 0;
        int objectloadIndex = 0;
        
        await locations.Task;
        foreach (var location in locations.Result)
        {
            if (ObjectLoadHandleCollection.ContainsKey(location.PrimaryKey)) continue;
            OnLoadAssetName?.Invoke(location.PrimaryKey);
            AsyncOperationHandle<UnityEngine.Object> loader = Addressables.LoadAssetAsync<UnityEngine.Object>(location.PrimaryKey);
            var time = Time.time;
            await loader.Task;
            time = Time.time - time;
            objectloadIndex++;
            percent = (float)(objectloadIndex) / (float)locations.Result.Count;
            OnLoadProgress?.Invoke(percent);
            Debug.Log("objecthandleCollection " + location.PrimaryKey);
            ObjectLoadHandleCollection.Add(location.PrimaryKey, loader);
        }
        AssetLoaderLocationCollection.Add(assetLoader.key, locations);
        OnAssetLoaded?.OnNext(assetLoader.key);
        Depug.Log("Asset Load " + assetLoader.key,Color.green);
        //OnLoadComplete?.Invoke();
    }
    
    public void StartLoadAssetQueue()
    {
        Depug.LogColor("Start loadAssetQueue ",Color.white);
        _keyToLoadAsset = new List<string>();
    }
    void AddKeysToList(List<string> keyList)
    {
        foreach (var key in keyList)
        {
            if(_keyToLoadAsset.Contains(key))
            {
                Debug.LogWarning($"Key {key} has add in KeyToLoadAssets.");
            }else
            {
                _keyToLoadAsset.Add(key);
            }
        }
    }
    public void AddLoaderQueue(AssetLoaderData assetLoader)
    {
        if(!CheckIsStartLoadQueue())return;
        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(assetLoader.labels, Addressables.MergeMode.Union, typeof(UnityEngine.Object));
        AddKeysToList(locations.Result.Select(_ => _.PrimaryKey).ToList());
        AssetLoaderLocationCollection.Add(assetLoader.key, locations);
    }
    public void AddLoadKeyQueue(List<string> dataKeys)
    {
        if(!CheckIsStartLoadQueue())return;
        AddKeysToList(dataKeys);
    }
    //download at runtime load asset then add key to objectPool
    //public async UniTask LoadAssetAtRuntime(ObjectData[] objectDatas)
    //{
    //    Depug.LogColor("LoadAssetAtRuntime count " + objectDatas.Length,Color.green);
    //    StartLoadAssetQueue();
    //    foreach (var data in objectDatas)
    //    {
    //        var assetKey = GrandoraUtils.AddressableAssetName(data);
    //        if(!opObjectLoaded.ContainsKey(assetKey))
    //        {
    //            _keyToLoadAsset.Add(assetKey);
    //        }
    //    }
    //    await LoadAssets(_keyToLoadAsset);
    //    _keyToLoadAsset.Clear();
    //    _keyToLoadAsset = null;
    //    CreativeEvent.InvokeOnLoadFragmentAddressableAssets(objectDatas);
    //}
    public async UniTask StartLoadAssetFromQueue()
    {
        Debug.Log("StartLoadAssetFromQueue "+_keyToLoadAsset.Count);
        float percent = 0;
        int objectLoadIndex = 0;
        foreach (var objectKey in _keyToLoadAsset)
        {
            Debug.Log("objectKey "+objectKey);
            if (ObjectLoadHandleCollection.ContainsKey(objectKey)) continue;
            OnLoadAssetName?.Invoke(objectKey);
            
            AsyncOperationHandle<UnityEngine.Object> loader = Addressables.LoadAssetAsync<UnityEngine.Object>(objectKey);
            var time = Time.time;
            await loader.Task;
            time = Time.time - time;
            objectLoadIndex++;
            percent = (float)(objectLoadIndex) / (float)_keyToLoadAsset.Count;
            OnLoadProgress?.Invoke(percent);
            ObjectLoadHandleCollection.Add(objectKey, loader);
        }
        _keyToLoadAsset.Clear();
        _keyToLoadAsset = null;
        //OnLoadComplete?.Invoke();
        OnLoadAssetFromQueueComplete?.Invoke();
    }
    
    public async UniTask LoadAssets(List<string> objectDataKeys)
    {
        float percent = 0;
        int objectLoadIndex = 0;
        foreach (var objectKey in objectDataKeys)
        {
            Depug.LogColor($"LoadAssets : {objectKey} contain {ObjectLoadHandleCollection.ContainsKey(objectKey)}",Color.white);
            if (ObjectLoadHandleCollection.ContainsKey(objectKey)) continue;
            OnLoadAssetName?.Invoke(objectKey);
            AsyncOperationHandle<UnityEngine.Object> asyncOperationHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(objectKey);
            var time = Time.time;
            await asyncOperationHandle.Task;
            time = Time.time - time;
            objectLoadIndex++;
            percent = (float)(objectLoadIndex) / (float)objectDataKeys.Count;
            OnLoadProgress?.Invoke(percent);
            Depug.LogColor($"AddKey : {objectKey}",Color.white);
            ObjectLoadHandleCollection.Add(objectKey, asyncOperationHandle);
        }
    }

    #region refactor
    public async UniTask StartLoadAssetFromQueue(Action callback)
    {
        Depug.LogColor("StartLoadAssetFromQueue withCallback "+_keyToLoadAsset.Count,Color.white);
        await LoadAssets(_keyToLoadAsset);
        _keyToLoadAsset.Clear();
        _keyToLoadAsset = null;
        callback?.Invoke();
    }
    bool CheckIsStartLoadQueue()
    {
        if (_keyToLoadAsset == null)
        {
            Debug.LogError("You must use method StartLoadAssetQueue Before add loderQueue");
            return false;
        }
        return true;
    }
    #endregion
    #region Get

    public static T Get<T>(string key) where T : UnityEngine.Object
    {
       // Debug.Log("Get key "+key+ "contain "+Instance.ObjectLoadHandleCollection.ContainsKey(key));
        if(!Instance.ObjectLoadHandleCollection.ContainsKey(key))
        {
           Debug.LogWarning($"Key {key} is not conain in opObjectLoaded");
        }
        return (T)Instance.ObjectLoadHandleCollection[key].Result;
    }

    public T TryGet<T>(string key, out bool loaded) where T : UnityEngine.Object
    {
        if (ObjectLoadHandleCollection.ContainsKey(key))
        {
            Debug.Log("containkey " + ObjectLoadHandleCollection[key].Result);
        }

        loaded = ObjectLoadHandleCollection.ContainsKey(key);
        return (T)ObjectLoadHandleCollection[key].Result;
    }
    #endregion

    #region AddLoaderData
    //add loaderData before Load()
    public void AddPreloadData(List<AssetLoaderData> loaderDataList)
    {
        foreach (AssetLoaderData loaderData in loaderDataList)
        {
            AddPreloadData(loaderData);
        }
    }
    public void AddPreloadData(AssetLoaderData loaderData)
    {
        if(!_preloaderData.Contains(loaderData))
        {
            _preloaderData.Add(Instantiate(loaderData));
        }
    }
    
    public async void AddLoaderKey(string key)
    {
        await AddLoaderData(_preloaderData.FirstOrDefault(loader => loader.key == key));
    }
    public async UniTask AddLoaderData(List<AssetLoaderData> types)
    {
        foreach(var type in types)
        {
            await AddLoaderData(type);
        }
    }
    public async UniTask AddLoaderData(AssetLoaderData loaderData)
    {
        if (LoaderDataCollection.ContainsKey(loaderData.key)) return;

        if (!AssetLoaderLocationCollection.ContainsKey(loaderData.key))
        {
            await GetObjectFormLoader(loaderData);
            LoaderDataCollection.Add(loaderData.key, loaderData);
        }
        else
        {
            Debug.Log("ready to add");
        }
    }


    #endregion
    #region Load
    public async UniTask Load()
    {
        await AddLoaderData(_preloaderData);
    }
    #endregion
    public void RemoveLoader(string loaderAssetKey)
    {
        if (!LoaderDataCollection.ContainsKey(loaderAssetKey)) return;
        var loaderInstance = LoaderDataCollection[loaderAssetKey];
        if (AssetLoaderLocationCollection.ContainsKey(loaderAssetKey))
        {
            var location = AssetLoaderLocationCollection[loaderAssetKey];
            foreach (var item in location.Result)
            {
                Addressables.ReleaseInstance(ObjectLoadHandleCollection[item.PrimaryKey]);
                ObjectLoadHandleCollection.Remove(item.PrimaryKey);
            }
            Addressables.Release(location);
            AssetLoaderLocationCollection.Remove(loaderAssetKey);
        }
        LoaderDataCollection.Remove(loaderAssetKey);
    }
    public async void PrepareLoad(bool showPopup)
    {
        if (showPopup == true)
        {
            AddLoadingQueue();
        }
        else
        {
            await LoadStart();
        }

    }
    private void OnApplicationQuit()
    {
        ReleaseAll();
    }

    public async UniTask LoadStart()
    {
        await Load();
    }

    public void AddLoadingQueue()
    {
        //LoadScreenIngameLayer.Instance.DialogHide();
        //LoadScreenIngameLayer.Instance.RegisterLoading(this);
        
    }


    #region Destroy
    private void OnDestroy()
    {
        ReleaseAll();
    }
    public void ReleaseAll()
    {
        ReleaseObjectLoadHandleCollection();
        ReleaseAssetLoaderLocationCollection();
        RemoveLoaderDataCollection();
    }
    void ReleaseObjectLoadHandleCollection()
    {
        foreach (var item in ObjectLoadHandleCollection)
        {
            Addressables.ReleaseInstance(item.Value);
        }
        ObjectLoadHandleCollection.Clear();
    }
    void ReleaseAssetLoaderLocationCollection()
    {
        foreach (AsyncOperationHandle handle in AssetLoaderLocationCollection.Values)
        {
            Addressables.Release(handle);
        }
        AssetLoaderLocationCollection.Clear();
    }
    void RemoveLoaderDataCollection()
    {
        foreach (AssetLoaderData loaderData in LoaderDataCollection.Values)
        {
            Destroy(loaderData);
        }
        LoaderDataCollection.Clear();
    }
    public void ClearDictionary()
    {

        //loaderTypeCollection.Clear();
        //assetLoaderLocations.Clear();

    }
    #endregion
}
