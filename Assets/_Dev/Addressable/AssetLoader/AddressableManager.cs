using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Addressable.Manager
{
    public class AddressableManager : MonoSingleton<AddressableManager> //Singleton
    {
        public Action<string> OnFullSizeDownloaded;
        public Action<string, float> OnFileSizeDownloadUpdate;
        public Action<bool> OnLoadCompleted;

        [SerializeField] AssetLabelReference label;
        [SerializeField] CloudKey cloudKey;

        private string awsBucketName;
        private string awsAccessKey;
        private string awsSecretKey;
        private bool initialized;

        public Dictionary<string, object> objectDic = new Dictionary<string, object>();

        private SceneInstance m_CurrentSceneLoaded;
        private AddressableManager() { }

        private void Awake()
        {
            //awsBucketName = cloudKey.BucketName;
            //awsAccessKey = cloudKey.AWSAccessKey;
            //awsSecretKey = cloudKey.AWSSecretKey;
            //Addressables.WebRequestOverride = EditWebRequestURL;
            AsyncOperationHandle<IResourceLocator> initialize = Addressables.InitializeAsync();
            initialize.Completed += AddressableManager_Completed;
            //DontDestroyOnLoad(this);
        }

        public void LoadingAssetFromCloud()
        {
            if (!initialized) return;
            StartCoroutine(DownloadDependencies());

        }

        public void ReleaseAssetInstance(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }
        public void GetAsset<T>(AssetReference assetRef, Action<T> callback)
        {
            AsyncOperationHandle<T> loader = assetRef.LoadAssetAsync<T>();
            loader.Completed += (obj) =>
            {
                callback.Invoke(obj.Result);
            };

            //Addressables.Release(loader);
        }

        public AsyncOperationHandle<T> GetAsset<T>(string key)
        {
            AsyncOperationHandle<T> loader = Addressables.LoadAssetAsync<T>(key);

            return loader;
        }

        public AsyncOperationHandle<T> GetAsset<T>(AssetReference assetRef)
        {
            AsyncOperationHandle<T> loader = assetRef.LoadAssetAsync<T>();

            return loader;
        }


        public void GetAsset<T>(string key, Action<T> callback)
        {
            AsyncOperationHandle<T> loader = Addressables.LoadAssetAsync<T>(key);
            loader.Completed += (obj) =>
            {
                T result = obj.Result;
                callback?.Invoke(result);
                //Addressables.Release(obj);

            };


        }

        public void GetAsset(AssetReference assetRef, Action<GameObject> callback)
        {
            AsyncOperationHandle<GameObject> loader = assetRef.LoadAssetAsync<GameObject>();
            loader.Completed += (handle) =>
            {
                if (loader.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject result = handle.Result;
                    callback?.Invoke(result);
                    
                }
                Addressables.Release(handle);
            };

        }
        public void GetAsset(string key, Action<GameObject> callback)
        {
            print("key " + key);
            print("BuildPath " + Addressables.BuildPath);
            print("PlayerBuildDataPath " + Addressables.PlayerBuildDataPath);
            print("LibraryPath " + Addressables.LibraryPath);
            print("RuntimePath " + Addressables.RuntimePath);
            AsyncOperationHandle<GameObject> loader = Addressables.LoadAssetAsync<GameObject>(key);
            loader.Completed += (handle) =>
            {
                if (loader.Status == AsyncOperationStatus.Succeeded)
                {

                    GameObject result = handle.Result;
                    callback?.Invoke(result);
                    Addressables.Release(handle);

                }
                Addressables.Release(loader);
            };

        }

        public IEnumerator RequestLoadScene(AssetReference assetRef)
        {
            AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(assetRef);

            while (sceneHandle.Status == AsyncOperationStatus.None)
            {
                yield return null;
            }

            m_CurrentSceneLoaded = sceneHandle.Result;
        }

        public IEnumerator RequestLoadScene(string key, Action callback)
        {
            AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (sceneHandle.Status == AsyncOperationStatus.None)
            {
                yield return null;
            }
            callback.Invoke();
            m_CurrentSceneLoaded = sceneHandle.Result;
        }

        public AsyncOperationHandle<SceneInstance> RequestLoadScene(string key, LoadSceneMode mode)
        {
            return Addressables.LoadSceneAsync(key, mode);
        }

        public AsyncOperationHandle<SceneInstance> RequestUnLoadScene(SceneInstance scene)
        {
            return Addressables.UnloadSceneAsync(scene, true);
        }

        public void RequestMultipleAsset<T>(List<AssetReference> assetRefList, Action<List<T>> callback)
        {
            AsyncOperationHandle<IList<T>> loadHandle = Addressables.LoadAssetsAsync<T>(assetRefList, addressable =>
            {

            }, Addressables.MergeMode.Union, false);

            loadHandle.Completed += (obj) =>
            {
                callback.Invoke(obj.Result as List<T>);
            };

        }
        public void RequestMultipleAsset<T>(string keys, Action<List<T>> callback)
        {
            List<string> keysList = new List<string>() { keys };
            AsyncOperationHandle<IList<T>> loadHandle = Addressables.LoadAssetsAsync<T>(keysList, addressable =>
            {

            }, Addressables.MergeMode.Union, false);

            loadHandle.Completed += (obj) =>
            {
                callback.Invoke(obj.Result as List<T>);
            };

        }
        public IEnumerator CheckCatalogs()
        {
            List<string> catalogsToUpdate = new List<string>();
            AsyncOperationHandle<List<string>> checkForUpdateHandle
                = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += op =>
            {
                catalogsToUpdate.AddRange(op.Result);
            };

            yield return checkForUpdateHandle;

            if (catalogsToUpdate.Count > 0)
            {
                AsyncOperationHandle<List<IResourceLocator>> updateHandle
                    = Addressables.UpdateCatalogs(catalogsToUpdate);
                yield return updateHandle;
                Addressables.Release(updateHandle);
            }

            Addressables.Release(checkForUpdateHandle);
        }

        public IEnumerator MultiplePreloadingScene(List<GameObject> assetRef, Action<string, GameObject> callbackPooling)
        {

            AsyncOperationHandle<IList<IResourceLocation>> loadHandle = Addressables.LoadResourceLocationsAsync(assetRef, Addressables.MergeMode.Union, typeof(GameObject));
            yield return loadHandle;

            foreach (IResourceLocation item in loadHandle.Result)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(item);
                yield return handle;

                AsyncOperationHandle<GameObject> assetObject = Addressables.InstantiateAsync(item);
                yield return assetObject;
                callbackPooling.Invoke(item.PrimaryKey, assetObject.Result);
            }

            Addressables.Release(loadHandle);
        }

        private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                //GetAsset("Assets/UIManagerment/Prefab/UIA.prefab", (prefab) => { Debug.Log(prefab); });
                //StartCoroutine(MultipleDownloadCatalogDependencies());
                initialized = true;
            }
        }


        private IEnumerator DownloadDependencies()
        {
            bool m_DownloadhasCompleted = false;

            var loadResourceLocationsAsync = Addressables.LoadResourceLocationsAsync(label);
            yield return loadResourceLocationsAsync;
            var resourceLocations = loadResourceLocationsAsync.Result;

            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(resourceLocations);
            yield return getDownloadSize;

            if (getDownloadSize.Result > 0)
            {
                AsyncOperationHandle downloadWorldDependencies = Addressables.DownloadDependenciesAsync(resourceLocations, false);
                while (downloadWorldDependencies.Status == AsyncOperationStatus.None)
                {
                    OnFileSizeDownloadUpdate?.Invoke(string.Format("{0:0.00} MB. / {1:0.00} MB.", ConvertBytesToMegabytes(downloadWorldDependencies.GetDownloadStatus().DownloadedBytes)
                        , ConvertBytesToMegabytes(downloadWorldDependencies.GetDownloadStatus().TotalBytes))
                        , downloadWorldDependencies.GetDownloadStatus().Percent);

                    yield return null;
                }
                yield return downloadWorldDependencies;

                OnFileSizeDownloadUpdate?.Invoke("Resource Checking...", 1);

                OnFileSizeDownloadUpdate?.Invoke
                (
                    downloadWorldDependencies.Status == AsyncOperationStatus.Succeeded ?
                    $"Patch Loaded" : $"Patch Load Failed"
                    , downloadWorldDependencies.Status == AsyncOperationStatus.Succeeded ? 1 : 0
                );
                m_DownloadhasCompleted = downloadWorldDependencies.Status == AsyncOperationStatus.Succeeded ? true : false;
                Addressables.Release(downloadWorldDependencies);
            }
            else
            {
                //Patch Latest
                m_DownloadhasCompleted = loadResourceLocationsAsync.Status == AsyncOperationStatus.Succeeded ? true : false;
                OnFileSizeDownloadUpdate?.Invoke("Resource Checking...", 1);
            }

            OnFileSizeDownloadUpdate?.Invoke(m_DownloadhasCompleted ? "Load Patch Completed" : "Load Patch Failed"
    , m_DownloadhasCompleted ? 1 : 0);
            OnLoadCompleted?.Invoke(m_DownloadhasCompleted);
        }

        private double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private void EditWebRequestURL(UnityWebRequest request)
        {
            //var headers = new Dictionary<string, string>
            //{
            //    {AWS4SignerBase.X_Amz_Content_SHA256, AWS4SignerBase.EMPTY_BODY_SHA256},
            //    {"content-type", "text/plain"}
            // };

            //var signer = new AWS4SignerForAuthorizationHeader
            //{
            //    EndpointUri = request.uri,
            //    HttpMethod = request.method,
            //    Service = "s3",
            //    Region = "ap-southeast-1"
            //};

            //var authorization = signer.ComputeSignature(headers,
            //request.uri.Query, // no query parameters
            //AWS4SignerBase.EMPTY_BODY_SHA256,
            //awsAccessKey,
            //awsSecretKey);
            //headers.Add("Authorization", authorization);

            //foreach (var header in headers)
            //{
            //    if (header.Key != "Host")
            //    {
            //        try
            //        {
            //            request.SetRequestHeader(header.Key, header.Value);
            //        }
            //        catch (Exception ex)
            //        {
            //            Debug.LogWarning(ex.Message);
            //            throw;
            //        }
            //    }
            //}
        }


        public static async Task<T> LoadObject<T>(string name) where T : UnityEngine.Object
        {
            if (Instance.objectDic.ContainsKey(name))
                return (T)Instance.objectDic[name];
            var handle = Addressables.LoadAssetAsync<T>(name);
            await handle.Task;
            if (handle.Result != null)
            {
                if (!Instance.objectDic.ContainsKey(name))
                    Instance.objectDic.Add(name, handle.Result);
            }

            T result = handle.Result;
            Addressables.Release(handle);
            return result;
        }

        public void GetObjectCount(AssetLabelReference label, Action<int> callback)
        {
            var location = Addressables.LoadResourceLocationsAsync(label, null);

            location.Completed += (g) => callback.Invoke(g.Result.Count);

            //layerList.Completed += (go) => callback.Invoke(go.Result.Count);
        }

        public void ClearDictionaryAsset()
        {
            foreach (var item in objectDic.Values)
            {
                Debug.Log("ClearAsset name " + item);
                Addressables.Release(item);
            }
            objectDic.Clear();
        }
        public void ClearAllAssets()
        {
            ClearDictionaryAsset();
        }

    }
}
