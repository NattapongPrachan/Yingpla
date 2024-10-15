using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
public class ResourcesLoaderManager : MonoSingleton<ResourcesLoaderManager>
{
    public bool IsMoving = false;
    public GameObject Object;
    public Dictionary<string,Object> ResourcesLoaded = new Dictionary<string, Object>();
    public override void Init()
    {
        base.Init();
        ResourcesLoaded = new Dictionary<string, Object>();
        
    }
    public IEnumerator<Object> GetObject<T>(string resourcePath,Action<Object> callback) where T : Object
    {
        if (ResourcesLoaded.ContainsKey(resourcePath))
        {
            //yield return ResourcesLoaded[resourcePath] as Object;
        }
        else
        {
           var obj =  Create<T>(resourcePath);
        }
        if (callback != null)
        {
            callback(ResourcesLoaded[resourcePath]);
        }
        yield return null;
    }

    public Object Get<T>(string resourcePath) where T : Object
    {
        if (ResourcesLoaded.ContainsKey(resourcePath))
        {
            return ResourcesLoaded[resourcePath] as T;
        }
        else
        {
            var objectLoaded = Create<T>(resourcePath);
        }
        var targetObject = ResourcesLoaded[resourcePath] as T;
        Debug.Log("target " + targetObject);
        return targetObject;
    }
    public Object Create<T>(string resourcePath) where T : Object
    {
       // Resources.Load
        var requestObject = Resources.Load<Object>(resourcePath);
        if (requestObject != null)
        {
            if (!ResourcesLoaded.ContainsKey(resourcePath))
            {
                ResourcesLoaded.Add(resourcePath, requestObject);
                return requestObject;
            }
        }
        return null;
    }
    IEnumerator LoadObject(string resourcePath)
    {
        var requestObject = Resources.Load<Object>(resourcePath);
        Debug.Log("loadComplete " + requestObject);
        if (requestObject != null)
        {
            if (!ResourcesLoaded.ContainsKey(resourcePath))
            {
                ResourcesLoaded.Add(resourcePath, requestObject);
                yield return requestObject;
            }
        }
        yield return null;
    }














    //public IObservable<Object> ToObservable(this UnityEngine.Resources asyncOperation)
    //{
    //    if (asyncOperation == null) throw new ArgumentNullException("asyncOperation");

    //    return Observable.FromCoroutine<Object>((observer, cancellationToken) => RunAsyncOperation(asyncOperation, observer, cancellationToken));
    //}

    //public IEnumerator RunAsyncOperation(UnityEngine.Resources asyncOperation, IObserver<Object> observer, CancellationToken cancellationToken)
    //{
    //    //while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
    //    //{
    //    //    //observer.OnNext(asyncOperation.progress);
    //    //    yield return null;
    //    //}
    //    //if (!cancellationToken.IsCancellationRequested)
    //    //{
    //    //    //observer.OnNext(asyncOperation.progress); 
    //    //    observer.OnCompleted();
    //    //}
    //}
}
