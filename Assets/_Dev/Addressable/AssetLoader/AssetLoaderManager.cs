using Addressable.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetLoaderManager : MonoSingleton<AssetLoaderManager>
{
    public static event Action OnLoadComplete;
    public static Action<string> OnLoadAssetName;
    public static event Action<float> OnLoadProgress;
    Dictionary<string, object> assetPaths = new Dictionary<string, object>();
    Dictionary<string, object> assetClass = new Dictionary<string, object>();
    Dictionary<string, UnityEngine.Object> assetObjects = new Dictionary<string, UnityEngine.Object>();
    public Dictionary<string, UnityEngine.Object> operationDictionary = new Dictionary<string, UnityEngine.Object>();
    //
    List<AssetLoader> loaders = new List<AssetLoader>();

    public Action OnLoaded { get => OnLoadComplete; set => OnLoaded += value; }
    public Action<float> OnProgress { get => OnProgress; set => OnProgress += value; }

    private void Start()
    {
        OnLoadProgress += ViewDownloadProgress;
    }
    public void AddAsset(string key, string path)
    {
        if (assetPaths.ContainsKey(key)) return;
        assetPaths.Add(key, path);
    }
    public void AddAsset(Dictionary<string, object> collectionPathKey)
    {
        assetPaths.Clear();
        assetPaths = collectionPathKey.ToDictionary(entry => entry.Key, entry => entry.Value);
        collectionPathKey.Clear();
    }
    public static void Add<T>(string name, T obj)
    {
        if (Instance.assetClass.ContainsKey(name)) return;
        Instance.assetClass.Add(name, obj);
    }
    public static T GetReference<T>(string name)
    {
        return (T)Convert.ChangeType(Instance.assetClass[name], typeof(T));
    }
    public static T Get<T>(string name)
    {

        return (T)Convert.ChangeType(Instance.assetObjects[name], typeof(T));
    }
    public bool TryGetObject<T>(string key, out T component)
    {
        if (Instance.assetObjects.ContainsKey(key))
        {
            component = Get<T>(key);
            return true;
        }
        component = default;
        return false;
    }
    public async void Load()
    {
        float percent = 0;
        int objectloadIndex = 0;
        foreach (var assetToLoad in assetPaths)
        {
            var targetObject = await AddressableManager.LoadObject<UnityEngine.Object>(assetToLoad.Value.ToString());
            OnLoadAssetName?.Invoke(assetToLoad.Value.ToString());
            if (!Instance.assetObjects.ContainsKey(assetToLoad.Key))
            {
                Instance.assetObjects.Add(assetToLoad.Key, targetObject);
            }
            objectloadIndex++;
            percent = (objectloadIndex * 100f) / assetPaths.Count;
            Debug.Log("asset : " + assetToLoad.Key + "percent" + percent);
            OnLoadProgress(percent);
            OnProgress.Invoke(percent);
        }
        OnLoadComplete();
    }

    public void ClearData()
    {
        AddressableManager.Instance.ClearAllAssets();
        Instance.assetObjects.Clear();
        Instance.assetPaths.Clear();
        assetClass.Clear();
    }
    private void ViewDownloadProgress(float obj)
    {

    }
    public int GetAssetLength() => assetObjects.Count;


}